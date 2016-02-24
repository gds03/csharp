using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CustomComponents.Core.ExtensionMethods;

namespace CustomComponents.Algorithms.Threading
{
    public sealed class SimpleThreadPool
    {
        #region Private Class


        private class Request
        {
            public Action<object> Method { get; private set; }
            public object Arg { get; private set; }

            public Request(Action<object> method, object arg)
            {
                if (method == null)
                    throw new ArgumentNullException("method");

                Method = method;
                Arg = arg;
            }
        }


        #endregion



        // Fields
        private readonly int m_maxNumWorkerThreads;
        private volatile bool m_shutDown;
        private int m_backgroundThreads;

        private readonly HashSet<Thread> m_threadsWaitingForUserWork;
        private readonly HashSet<Thread> m_threadsPerformingUserWork;
        private readonly LinkedList<Request> m_workQueue;

        private readonly object sync = new object();        // monitor and condition variable (Wait queue).


        // Ctor
        public SimpleThreadPool() : this(Environment.ProcessorCount * 2)
        {

        }

        public SimpleThreadPool(int maxNumThreads)
        {
            if (maxNumThreads <= 0)
                throw new ArgumentException("maxNumThreads <= 0");

            m_maxNumWorkerThreads = maxNumThreads;
            m_backgroundThreads = 0;

            m_threadsWaitingForUserWork = new HashSet<Thread>();
            m_threadsPerformingUserWork = new HashSet<Thread>();
            m_workQueue = new LinkedList<Request>();
        }


        // Public methods
        public void Shutdown()
        {
            m_shutDown = true;  // write barrier.
        }

        public int BusyThreads
        {
            get
            {
                lock (sync) { return m_threadsPerformingUserWork.Count; }
            }
        }

        public int SuspendedThreads
        {
            get
            {
                lock (sync) { return m_threadsWaitingForUserWork.Count; }
            }
        }

        public bool QueueWorkItem(Action method)
        {
            return this.QueueWorkItem(a => method(), null);
        }

        public bool QueueWorkItem(Action<object> method, object arg)
        {
            if (m_shutDown)
                return false;

            Request wb = new Request(method, arg);

            lock(sync)
            {
                // put this work to be done at the end of requests queue.
                m_workQueue.AddLast(wb);

                if (m_backgroundThreads < m_maxNumWorkerThreads)
                {
                    // no available threads. Create one.
                    CreateBackgroundThreadAndStart();
                }

                // notify one thread that queue changed - remove it from sync wait queue, and put it on ready queue of the CPU to be evaluated.
                Monitor.Pulse(sync);
            }

            return true;
        }



        // Private methods
        private void CreateBackgroundThreadAndStart()
        {
            // called from QueueWorkItem - in the pose of the lock.
            Thread runner = new Thread(DispatchQueueMethod);
            runner.IsBackground = true;
            m_backgroundThreads++;

            // add created thread to the suspended threads list.
            m_threadsWaitingForUserWork.Add(runner);

            // go to dispatch queue
            runner.Start();
        }

        private void DispatchQueueMethod()
        {
            while(!m_shutDown)
            {
                Request wb = null;
                lock (sync)
                {
                    while (true)
                    {
                        // work to do?
                        if (m_workQueue.Count > 0)
                        {
                            // pick up first item of work to perform
                            wb = m_workQueue.GetFirstAndRemove();

                            // remove me from suspended and put me busy;
                            if (!m_threadsWaitingForUserWork.Remove(Thread.CurrentThread))
                                throw new InvalidOperationException("This thread is not suspended. Program error");

                            // put me busy
                            m_threadsPerformingUserWork.Add(Thread.CurrentThread);
                            
                            // Signal that i've changed user work items.
                            Monitor.Pulse(sync);
                            break;
                        }

                        Monitor.Wait(sync);
                    }
                }

                // perform the work without the lock
                if (wb == null)
                    throw new InvalidOperationException("Wait block item should be != null. Program error");

                // execute method
                wb.Method(wb.Arg);

                // after execute, remove me from busy and put me waiting again.
                lock (sync)
                {
                    m_threadsPerformingUserWork.Remove(Thread.CurrentThread);
                    m_threadsWaitingForUserWork.Add(Thread.CurrentThread);
                }
            }
        }
    }
}
