using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace EnhancedLibrary.ExternalTypes.Business
{
    public class SingletonInitializer
    {
        volatile static int s_start = 0;
        volatile static int s_end = 0;


        /// <summary>
        ///     Only one thread can initialize func function.
        /// </summary>
        /// <param name="func"></param>
        /// <returns>False if wasn't initialized and is not initialized and true if it was already initialized. </returns>
        public bool Once(Action func)
        {
            do
            {
                // break condition
                if ( s_start == 1 && s_end == 1 )
                    return true;

                #pragma warning disable 420

                if ( s_start == 0 && Interlocked.CompareExchange(ref s_start, 1, 0) == 0 )
                {
                    // Only one thread can be here..
                    func();
                    Interlocked.Exchange(ref s_end, 1);
                    return false;
                }

                if ( Environment.ProcessorCount == 1 )
                    Thread.Yield();     // Let the other thread to finish initializing progress..

                #pragma warning restore 420
            }

            while ( true );
        }
    }
}
