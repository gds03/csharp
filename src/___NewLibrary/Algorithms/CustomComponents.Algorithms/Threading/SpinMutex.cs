using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CustomComponents.Algorithms.Threading
{
    public sealed class SpinMutex 
    {
        private static bool isMultiProc = Environment.ProcessorCount > 1;



        private volatile int owner;
        private int count;

        public void Enter()
        {
            int tid = Thread.CurrentThread.ManagedThreadId; // always > 0
            if (owner == tid) { count++; return; }

            do
            {
                if (owner == 0 && Interlocked.CompareExchange(ref owner, tid, 0) == 0)
                    return;

                if (isMultiProc) { Thread.SpinWait(10); }
                else { Thread.Yield(); }
            }
            while (true);
        }

        public void Exit()
        {
            int tid = Thread.CurrentThread.ManagedThreadId;

            if (owner == tid)
            {
                if (count != 0) { 
                    count--; 
                    return; 
                }

                owner = 0;
            }
        }

    }
}
