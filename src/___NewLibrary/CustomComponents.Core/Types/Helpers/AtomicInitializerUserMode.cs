using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CustomComponents.Core.Types.Helpers
{
    public static class AtomicInitializerUserMode
    {

        public static T ThreadSafe<T>(ref T refObjectToInitialize, Func<T> factoryValue)
            where T : class
        {
            SpinWait sWait = new SpinWait();
            do
            {
                if (refObjectToInitialize != null)
                    return refObjectToInitialize;

                if (refObjectToInitialize == null && Interlocked.CompareExchange(ref refObjectToInitialize, factoryValue(), null) == null)
                    return refObjectToInitialize;

                sWait.SpinOnce();
            }
            while (true);
        }


        public static T ThreadSafe<T>(ref T refObjectToInitialize, ref bool processingInitFlag, Func<T> factoryValue)
            where T : class
        {
            SpinWait sWait = new SpinWait();
            do
            {
                if (refObjectToInitialize != null)
                    return refObjectToInitialize;

                if (refObjectToInitialize == null && (!processingInitFlag) && (processingInitFlag = true) && Interlocked.CompareExchange(ref refObjectToInitialize, factoryValue(), null) == null)
                {
                    processingInitFlag = false;
                    return refObjectToInitialize;
                }

                sWait.SpinOnce();
            }
            while (true);
        }
    }
}
