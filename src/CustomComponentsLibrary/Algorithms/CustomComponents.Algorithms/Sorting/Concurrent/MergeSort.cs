using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomComponents.Algorithms.Sorting
{



    public partial class Sort
    {
        /// <summary>
        ///     Applies mergesort algorithm over the array.
        /// </summary>
        public static void Merge<T>(T[] arr) where T : IComparable<T>
        {
            MergeR(arr, 0, arr.Length - 1);
        }

        /// <summary>
        ///     Applies mergesort algorithm over the array in a parallel way.
        /// </summary>
        /// <param name="depthParallel">Specifies how many levels (sub arrays within main array) are executed in parallel.</param>
        public static void MergeParallel<T>(T[] arr, int depthParallel = 1) where T : IComparable<T>
        {
            MergeParallelR(arr, 0, arr.Length - 1, 0, depthParallel);
        }


        private static void MergeParallelR<T>(T[] arr, int low, int high, int currentDepth, int depthParallel) where T : IComparable<T>
        {
            // check if low is smaller then high, if not then the array is sorted
            if (low < high)
            {
                Task tl = null, tr = null;

                // Get the index of the element which is in the middle
                int middle = low + (high - low) / 2;

                // Sort the left side of the array
                Action leftFunc = () => MergeParallelR(arr, low, middle, currentDepth + 1, depthParallel);
                if (currentDepth <= depthParallel)
                    tl = Task.Factory.StartNew(leftFunc);

                else leftFunc();

                // Sort the right side of the array
                Action rightFunc = () => MergeParallelR(arr, middle + 1, high, currentDepth + 1, depthParallel);
                if (currentDepth <= depthParallel)
                    tr = Task.Factory.StartNew(rightFunc);

                else rightFunc();

                if (currentDepth <= depthParallel)
                    Task.WaitAll(tl, tr);

                // Combine them both
                MergeOperation(arr, low, middle, high);
            }
        }


        private static void MergeR<T>(T[] src, int low, int high) where T : IComparable<T>
        {
            // check if low is smaller then high, if not then the array is sorted
            if (low < high)
            {
                // Get the index of the element which is in the middle
                int middle = low + (high - low) / 2;

                // Sort the left side of the array
                MergeR(src, low, middle);

                // Sort the right side of the array
                MergeR(src, middle + 1, high);

                // Combine them both
                MergeOperation(src, low, middle, high);

            }
        }


        private static void MergeOperation<T>(T[] v, int l, int m, int r)
            where T : IComparable<T>
        {
            int size2 = r - m;
            T[] v2 = new T[size2];
            System.Array.Copy(v, m + 1, v2, 0, size2);      // copy the right array.
            int put = r;
            int i1 = m, i2 = size2 - 1;

            while (i1 >= l && i2 >= 0)
            {
                if (v2[i2].CompareTo(v[i1]) >= 0)
                    v[put] = v2[i2--];
                else
                    v[put] = v[i1--];
                --put;
            }
            while (i2 >= 0)
            {
                v[put--] = v2[i2--];
            }
        }
    }
}
