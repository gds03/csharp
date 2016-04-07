using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomComponents.Core.Types
{
    public class EqualityComparer<T> : IEqualityComparer<T>
    {
        readonly Func<T, T, bool> m_comparer;



        public EqualityComparer(Func<T, T, bool> comparer)
        {
            if (comparer == null)
                throw new ArgumentNullException("comparer");

            m_comparer = comparer;
        }

        public bool Equals(T x, T y)
        {
            return m_comparer(x, y);
        }

        public int GetHashCode(T obj)
        {
            return obj.ToString().ToLower().GetHashCode();
        }
    }
}
