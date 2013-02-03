using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnhancedLibrary.ExternalTypes.Business
{
    public class EqComparer<T> : IEqualityComparer<T>
    {
        readonly Func<T, T, bool> m_comparer;



        public EqComparer(Func<T, T, bool> comparer)
        {
            if ( comparer == null )
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
