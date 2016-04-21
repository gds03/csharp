using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace OMapper.Types.Helpers
{

    public static class UsermodeInitializator
    {
        /// <summary>
        ///     Initializes at user-mode while possible the initializationObject
        /// </summary>
        /// <param name="initializationObject">Object to initialize. > Must be volatile</param>
        /// <param name="initialized">Flag that indicates if was already initialized. > Must be volatile</param>
        /// <param name="initializing">Flag that indicates if is initializing or not. > Must be volatile </param>
        /// <param name="factoryValue">Func that returns the instance to be set on the reference of initializationObject</param>
        /// <returns></returns>
        public static T ThreadSafe<T>(ref T initializationObject, ref int initialized, ref int initializing, Func<T> factoryValue)
            where T : class
        {
            SpinWait sWait = new SpinWait();
            do
            {
                if (initializing == 1)
                    return initializationObject;

                if (initializing == 0 && Interlocked.CompareExchange(ref initializing, 1, 0) == 0)
                {
                    initializationObject = factoryValue();
                    initializing = 0;
                    initialized = 1;
                    return initializationObject;
                }

                sWait.SpinOnce();
            }
            while (true);
        }
    }
}
