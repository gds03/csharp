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

        private static void MergeOperation<T>(T[] src, int low, int middle, int high) where T : IComparable<T>
        {
            // copy current portion to backup portion size array
            T[] bck = new T[high - low + 1];
            for (int idx = low, bckIdx; (bckIdx = (idx - low)) < bck.Length; idx++)
            {
                bck[bckIdx] = src[idx];
            }

            int i1, i1Stop, i2, i2Stop, iSrc;

            i1 = 0;         // starts to compare always on the start
            i1Stop = (bck.Length - 1) / 2;

            i2 = i1Stop + 1;
            i2Stop = bck.Length - 1;
            iSrc = low;

            while (i1 <= i1Stop && i2 <= i2Stop)
            {
                src[iSrc++] = (bck[i1].CompareTo(bck[i2]) <= 0) ? bck[i1++]
                                                                : bck[i2++];
            }

            // we are in this situation if right idx arrived at end (and so, left part must be copied is the greatest part in terms of values)
            while (i1 <= i1Stop)
            {
                src[iSrc++] = bck[i1++];
            }
        }
    }
}
