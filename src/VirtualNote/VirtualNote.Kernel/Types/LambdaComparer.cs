using System;
using System.Collections.Generic;

namespace VirtualNote.Kernel.Types
{
    class LambdaComparer<T> : IEqualityComparer<T>
    {
        private readonly Func<T, T, bool> _comparer;

        public LambdaComparer(Func<T, T, bool> comparer) {
            if (comparer == null)
                throw new ArgumentNullException("comparer");

            _comparer = comparer;
        }

        public bool Equals(T x, T y) {
            return _comparer(x, y);
        }

        public int GetHashCode(T obj) {
            return obj.ToString().ToLower().GetHashCode();
        }
    }

}
