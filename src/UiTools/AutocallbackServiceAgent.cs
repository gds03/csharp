
using System.Collections.Concurrent;
using System.Threading;
using System;
using System.Windows.Forms;
using System.Diagnostics;

namespace UiTools
{
    public class AutoCallbackServiceAgent
    {
        private readonly ConcurrentDictionary<String, bool> _ProcessingManager;
        private readonly object _callbackTarget;

        public AutoCallbackServiceAgent(object callbackTarget) {

            // Store reference to the callback target object.
            _callbackTarget = callbackTarget;

            // Initialize dictionary that controls the creation of threads peer method
            _ProcessingManager = new ConcurrentDictionary<string, bool>();
        }




        private void InvokeAutoCallback(Delegate del, params object[] parameters) {

            // If the target is a control, make sure we invoke it on the correct thread.
            Control targetCtrl = _callbackTarget as Control;

            if ( targetCtrl != null && targetCtrl.InvokeRequired ) {

                //
                // Thread that called this method is not the UI Thread.
                // Schedule a new work item to dispatch UI message queue.

                targetCtrl.Invoke(del, parameters);
            }

            else {

                // Thread that called this method is the UI Thread.
                // Execute immediatelly
                del.DynamicInvoke(parameters);
            }
        }



        protected void ExecuteParallel(Action<string> funcPtr) {
            bool isProcessing;
            string methodName = new StackTrace().GetFrame(1).GetMethod().Name;

            // If key don't exist, add a new one and jump to the end
            if ( _ProcessingManager.TryAdd(methodName, true) )
                goto procede;

            // If we are here, means that key exists. 
            // We want procede only if the value is false (not proceding), and we set true (procede). Otherwise (another thread is processing) we stop.
            if ( !_ProcessingManager.TryUpdate(methodName, true, false) )
                return;

        procede:

            // Enqueue to TP
            ThreadPool.QueueUserWorkItem(x => funcPtr(methodName));
        }





        protected void FinishAsyncProcess(Delegate del, string methodName, params object[] parameters) {
            // Indicate that finished processing for that method
            _ProcessingManager.AddOrUpdate(methodName, false, (a, b) => false);

            // Invoke callback
            InvokeAutoCallback(del, parameters);
        }
    }

}