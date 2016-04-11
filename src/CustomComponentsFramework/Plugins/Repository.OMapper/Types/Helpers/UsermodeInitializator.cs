using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Repository.OMapper.Types.Helpers
{

    public static class UsermodeInitializator
    {
        /// <summary>
        ///     Initializes at user-mode while possible the initializationObject
        /// </summary>
        /// <param name="initializationObject">Object to initialize. > Must be volatile</param>
        /// <param name="initializing">Flag that indicates if is initializing or not. > Must be volatile </param>
        /// <param name="factoryValue">Func that returns the instance to be set on the reference of initializationObject</param>
        /// <returns></returns>
        public static T ThreadSafe<T>(ref T initializationObject, ref int initializing, Func<T> factoryValue)
            where T : class
        {
            SpinWait sWait = new SpinWait();
            do
            {
                if (initializationObject != null)
                    return initializationObject;

                if (initializing == 0 && Interlocked.CompareExchange(ref initializing, 1, 0) == 0)
                {
                    initializationObject = factoryValue();
                    initializing = 0;
                    return initializationObject;
                }

                sWait.SpinOnce();
            }
            while (true);
        }
    }
}
