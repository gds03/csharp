using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomComponents.Algorithms.Recursion
{
    public static class ArrayRecursive
    {
        public static T[] MakeArrayOfInverted<T>(T[] original)
        {
            // invariants checking
            if (original == null)
                return null;

            if (original.Length < 2)
                return original;

            // create an empty array to insert inverted data
            T[] inverted = new T[original.Length];
            return MakeArrayOfInvertedR(original, inverted, 0);

        }



        private static T[] MakeArrayOfInvertedR<T>(T[] original, T[] inverted, int idx)
        {
            if (idx == original.Length)
                return inverted;

            inverted[idx] = original[original.Length - 1 - idx];
            return MakeArrayOfInvertedR(original, inverted, ++idx);
        }
    }
}
