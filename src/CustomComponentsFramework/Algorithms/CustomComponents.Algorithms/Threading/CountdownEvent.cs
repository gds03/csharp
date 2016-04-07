using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CustomComponents.Algorithms.Threading
{
    public class CountdownEvent
    {
        private readonly int m_initialCount;
        private readonly ManualResetEvent m_resetEvent = new ManualResetEvent(false);
        private readonly object sync = new object();

        private int m_currentCount;

        public CountdownEvent(int initialCount) 
        {
            if(initialCount > 0)
            {
                m_initialCount = initialCount;
                m_currentCount = initialCount;
            }
        }

        public bool Wait(int timeout)
        {
            lock (sync)
            {
                if (m_currentCount == 0)
                    return true;

                int lastTime = (timeout == Timeout.Infinite ? timeout : 0);

                do
                {
                    try { m_resetEvent.WaitOne(timeout); }
                    catch (ThreadInterruptedException tex)
                    {
                        if (m_currentCount == 0)
                        {
                            Thread.CurrentThread.Interrupt();
                            return true;
                        }
                        throw;
                    }

                    if (m_currentCount == 0)
                        return true;

                    if (SyncUtils.AdjustTimeout(ref lastTime, ref timeout) == 0)
                        return false;
                }
                while (true);
            }
        }

        public void Signal()
        {
            lock (sync)
            {
                if (m_currentCount > 0)
                {
                    if (m_currentCount-- == 0)
                        m_resetEvent.Set();
                }
            }
        }

        public bool TryAdd()
        {
            lock (sync)
            {
                if (m_currentCount == 0)
                    return false;

                m_currentCount++;
                return true;
            }
        }

        public void Reset()
        {
            lock(sync)
            {
                m_resetEvent.Reset();
                m_currentCount = m_initialCount;
            }
        }
    }
}
