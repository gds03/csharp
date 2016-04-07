using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CustomComponents.Algorithms.Threading
{
    public class RefCountHolder<T> where T : class
    {
        private readonly T m_value;
        private volatile int m_refCounter;


        public RefCountHolder(T @object)
        {
            if (@object == null)
                throw new ArgumentNullException("object == null");

            m_value = @object;
            m_refCounter = 1;
        }


        public void AddReference()
        {
            int r;

            do
            {
                if (m_refCounter == 0)
                    throw new ObjectDisposedException("@object");

                r = m_refCounter;
                if (m_refCounter != 0 && Interlocked.CompareExchange(ref m_refCounter, r + 1, r) == r)
                    return;
            }
            while (true);
        }

        public void ReleaseReference()
        {
            int r;

            do
            {
                if (m_refCounter == 0)
                    throw new InvalidOperationException();

                r = m_refCounter;
                if (Interlocked.CompareExchange(ref m_refCounter, r - 1, r) == r)
                {
                    if (r - 1 == 0)
                    {
                        IDisposable disposableObject = m_value as IDisposable;
                        if (disposableObject != null)
                            disposableObject.Dispose();
                    }
                    return;
                }
            }
            while (true);
        }
    }
}
