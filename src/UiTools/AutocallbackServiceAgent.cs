using System;
using System.Collections.Concurrent;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;

namespace ExtensionMethods.Utilities
{
    /// <summary>
    ///     This class provides the ability to execute some background work and after the work has been
    ///     finished, execute a function in the context of the UI Thread.
    /// </summary>
    public class AutoCallbackServiceAgent
    {
        private readonly ConcurrentDictionary<int, bool>  m_methods_manager;
        private readonly Control                          m_targetcontrol;







        public AutoCallbackServiceAgent(Control control) {

            if ( control == null )
                throw new ArgumentNullException("control");

            // Store reference to the callback target object.
            m_targetcontrol = control;

            // Initialize dictionary that controls the creation of threads peer method
            m_methods_manager = new ConcurrentDictionary<int, bool>();
        }




        void InvokeAutoCallback(Delegate method, params object[] methodParameters) 
        {
            if ( m_targetcontrol.InvokeRequired ) {

                //
                // Thread that called this method is not the UI Thread.
                // Schedule a new work item to dispatch UI message queue.

                m_targetcontrol.Invoke(method, methodParameters);
            }

            else {

                // Thread that called this method is the UI Thread.
                // Execute immediatelly
                method.DynamicInvoke(methodParameters);
            }
        }








        /// <summary>
        ///     Performs the task (parallelFunction) if no task is 
        ///     currently executing that method.
        /// </summary>
        /// <param name="parallelFunction">A delegate that receives a token that must be passed to the FinishAsyncProcess when finished.</param>
        protected void ExecuteParallel(Action<int> parallelFunction) 
        {
            int syncToken = new StackTrace().GetFrame(1).GetMethod().Name.GetHashCode();

            // If key don't exist, add a new one and jump to the end
            if ( m_methods_manager.TryAdd(syncToken, true) )
                goto procede;

            // If we are here, means that key exists. 
            // We want procede only if the value is false (not proceding), and we set true (procede). Otherwise (another thread is processing) we stop.
            if ( !m_methods_manager.TryUpdate(syncToken, true, false) )
                return;

        procede:

            // Enqueue to TP
            ThreadPool.QueueUserWorkItem(x => parallelFunction(syncToken));
        }




        /// <summary>
        ///     Indicates that task was finished.
        /// </summary>
        /// <param name="synchronizationToken">The token from ExecuteParallel method </param>
        /// <param name="method">The callback method that must be called with the owner of the control</param>
        /// <param name="methodParameters">The parameters that callback method expect</param>
        protected void FinishAsyncProcess(int synchronizationToken, Delegate method, params object[] methodParameters)
        {
            // Indicate that finished processing for that method
            m_methods_manager.AddOrUpdate(synchronizationToken, false, (a, b) => false);

            // Invoke callback
            InvokeAutoCallback(method, methodParameters);
        }
    }
}
