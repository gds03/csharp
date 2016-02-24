using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomComponents.Core.Types.Collections
{
    public sealed class TimerConcurrentDictionary<TKey, TValue> : ConcurrentDictionary<TKey, TValue>, IDisposable
    {
        public const int MINIMUM_DELAY = 10000;     // 10 seconds

        public int GlobalRefreshMilis { get; set; }
        public System.Timers.Timer Timer { get; private set; }
        private bool Disposed { get; set; }

        public TimerConcurrentDictionary(int concurrencyLevel, int capacity)
            : base(concurrencyLevel, capacity)
        {

        }


        public TimerConcurrentDictionary(int concurrencyLevel, int capacity, int globalRefreshMilis)
            : this(concurrencyLevel, capacity)
        {
            if (globalRefreshMilis >= MINIMUM_DELAY)
            {
                GlobalRefreshMilis = globalRefreshMilis;

                Timer = new System.Timers.Timer(globalRefreshMilis);
                Timer.Elapsed += (o, e) => Runner();
                Timer.Start();
            }

            else
            {
                // normal dictionary behavior without timer.
            }
        }


        ~TimerConcurrentDictionary()
        {
            InternalDispose();
        }

        public void Dispose()
        {
            InternalDispose();
        }

        private void Runner()
        {
            Clear();
        }



        private void InternalDispose()
        {
            if (!Disposed)
            {
                Disposed = true;

                if (Timer != null)
                {
                    Timer.Stop();
                    Timer.Dispose();
                }
            }
        }
    }
}
