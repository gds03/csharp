public class AutoCallbackServiceAgent
    {
        private readonly ConcurrentDictionary<int, bool>  _processingManager;
        private readonly Control                          _callbackTarget;







        public AutoCallbackServiceAgent(Control callbackTarget) {

            if ( callbackTarget == null )
                throw new ArgumentNullException("callbackTarget cannot be null");

            // Store reference to the callback target object.
            _callbackTarget = callbackTarget;

            // Initialize dictionary that controls the creation of threads peer method
            _processingManager = new ConcurrentDictionary<int, bool>();
        }




        void InvokeAutoCallback(Delegate method, params object[] methodParameters) 
        {
            if ( _callbackTarget.InvokeRequired ) {

                //
                // Thread that called this method is not the UI Thread.
                // Schedule a new work item to dispatch UI message queue.

                _callbackTarget.Invoke(method, methodParameters);
            }

            else {

                // Thread that called this method is the UI Thread.
                // Execute immediatelly
                method.DynamicInvoke(methodParameters);
            }
        }



        protected void ExecuteParallel(Action<int> parallelFunction) 
        {
            bool isProcessing;
            int syncToken = new StackTrace().GetFrame(1).GetMethod().Name.GetHashCode();

            // If key don't exist, add a new one and jump to the end
            if ( _processingManager.TryAdd(syncToken, true) )
                goto procede;

            // If we are here, means that key exists. 
            // We want procede only if the value is false (not proceding), and we set true (procede). Otherwise (another thread is processing) we stop.
            if ( !_processingManager.TryUpdate(syncToken, true, false) )
                return;

        procede:

            // Enqueue to TP
            ThreadPool.QueueUserWorkItem(x => parallelFunction(syncToken));
        }





        protected void FinishAsyncProcess(int synchronizationToken, Delegate method, params object[] methodParameters)
        {
            // Indicate that finished processing for that method
            _processingManager.AddOrUpdate(synchronizationToken, false, (a, b) => false);

            // Invoke callback
            InvokeAutoCallback(method, methodParameters);
        }
    }