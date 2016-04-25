using System;
using System.Linq;

namespace VirtualNote.Common.ExtensionMethods
{
    public static class ByteArrayExtensions
    {
        public static bool AllBytesAreEqual(this byte[] array, byte[] otherarray)
        {
            if(array == null)
                throw new ArgumentNullException("array");
            if(otherarray == null)
                throw new ArgumentNullException("otherarray");

            if (array.Length == 0 || otherarray.Length == 0)
                throw new InvalidOperationException();

            if (array.Length != otherarray.Length)
                return false;

            return !array.Where((b, i) => b != otherarray[i]).Any();
        }
    }
}
