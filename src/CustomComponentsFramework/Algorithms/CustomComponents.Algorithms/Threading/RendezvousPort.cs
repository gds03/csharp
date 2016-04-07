using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CustomComponents.Core.ExtensionMethods;

namespace CustomComponents.Algorithms.Threading
{
    public class RendezvousPort<TRequest, TResponse>
    {
        private class WaitBlock<TReq, TResp>
        {
            readonly TReq m_request;
            TResp m_response;
            bool m_completed;

            public WaitBlock(TReq request)
            {
                m_request = request;
            }

            public void SetResponse(TResp r) { m_response = r; m_completed = true; }

            public bool Completed { get { return m_completed; } }
            public TReq Request { get { return m_request; } }
            public TResp Response { get { return m_response; } }
        }



        private readonly LinkedList<WaitBlock<TRequest, TResponse>> m_clientsReq = new LinkedList<WaitBlock<TRequest, TResponse>>();
        private readonly LinkedList<bool> m_serverReq = new LinkedList<bool>();

        // Public API
        public bool RequestService(TRequest request, int timeout, out TResponse response)
        {
            lock(m_clientsReq)
            {
                response = default(TResponse);
                bool interrupted = false;

                LinkedListNode<WaitBlock<TRequest, TResponse>> wb = m_clientsReq.AddLast(new WaitBlock<TRequest, TResponse>(request));
                NotifyFirstServerThread();  

                int lastTime = (timeout != Timeout.Infinite) ? timeout : 0;

                do
                {
                    try { SyncUtils.Wait(m_clientsReq, wb, timeout); }
                    catch(ThreadInterruptedException)
                    {
                        // predicate evaluation after timeout 
                        if( wb.Value.Completed)
                        {
                            response = wb.Value.Response;
                            Thread.CurrentThread.Interrupt();
                            return true;
                        }

                        if( wb.List == m_clientsReq )
                        {
                            // still waiting - nothing to do just fix stuff and throw
                            m_clientsReq.Remove(wb);
                            NotifyFirstServerThread();
                            throw;
                        }

                        // not on clientsReq, => being processed by server threads.
                        interrupted = true;
                        timeout = Timeout.Infinite;
                    }

                    // predicate 
                    if( wb.Value.Completed )
                    {
                        response = wb.Value.Response;

                        if (!interrupted)
                            return true;

                        Thread.CurrentThread.Interrupt();
                        return true;
                    }

                    // adjust timeout
                    if( !interrupted && SyncUtils.AdjustTimeout(ref lastTime, ref timeout) == 0 )
                    {
                        if (wb.List == m_clientsReq)
                        {
                            m_clientsReq.Remove(wb);
                            NotifyFirstServerThread();
                            return false;
                        }

                        // in process by the server, put timeout infinite.
                        timeout = Timeout.Infinite;
                    }
                } 
                while (true);
            }
        }


        public object AcceptService(int timeout, out TRequest request)
        {
            lock(m_clientsReq)
            {
                request = default(TRequest);

                do
                {
                    if (m_clientsReq.Count > 0)
                    {
                        WaitBlock<TRequest, TResponse> firstRequest = m_clientsReq.GetFirstAndRemove();

                        request = firstRequest.Request;
                        object token = firstRequest;
                        return token;
                    }

                    // wait
                    int lastTime = (timeout != Timeout.Infinite) ? timeout : 0;
                    LinkedListNode<bool> threadblock = m_serverReq.AddLast(false);

                    SyncUtils.Wait(m_clientsReq, threadblock, timeout);

                    // adjust timeout
                    if (SyncUtils.AdjustTimeout(ref lastTime, ref timeout) == 0)
                    {
                        return null;
                    }
                }
                while (true);
            }
        }

        public void CompleteService(object token, TResponse response)
        {
            lock(m_clientsReq)
            {
                LinkedListNode<WaitBlock<TRequest, TResponse>> wb = null;

                if (token != null && (wb = token as LinkedListNode<WaitBlock<TRequest, TResponse>>) != null )
                {
                    wb.Value.SetResponse(response);
                    NotifyFirstClientThread();
                }
            }
        }



        // Private methods

        private void NotifyFirstClientThread()
        {
            if (m_clientsReq.Count > 0)
                SyncUtils.Notify(m_clientsReq, m_clientsReq.First);
        }

        private void NotifyFirstServerThread()
        {
            if (m_serverReq.Count > 0)
                SyncUtils.Notify(m_clientsReq, m_serverReq.First);
        }
    }
}
