using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomComponents.Algorithms.Sorting
{
    public partial class Sort
    {
        private enum OPERATION { DIVIDE, MERGE };


        private class SortItem<T>
        {
            public int l { get; internal protected set; }
            public int r { get; internal protected set; }
            public OPERATION op { get; internal protected set; }

            public SortItem(int left, int right, OPERATION o)
            {
                l = left;
                r = right;
                op = o;
            }
        }




        


        // Considere a dimensão de v igual 2^N, para se obterem metades iguais
        public static void MergeSortIterative<T>(T[] v, int left, int right)
            where T : IComparable<T>
        {
            Stack<SortItem<T>> stack = new Stack<SortItem<T>>();
            stack.Push(new SortItem<T>(left, right, OPERATION.DIVIDE));
            int l, r, length;

            while (stack.Count != 0)
            {
                SortItem<T> item = stack.Pop();
                l = item.l;
                r = item.r;
                length = r - l + 1;

                Console.WriteLine("Sorting array from l=" + item.l + " to r=" + item.r + "; opr=" + item.op);

                // Divide
                if (item.op == OPERATION.DIVIDE)
                {
                    if (length < 2)
                    { 
                        // Cannot divide more, push to merge

                        // Pop next item to process
                        SortItem<T> nextitem = stack.Pop();
                        // Push array to merge
                        stack.Push(new SortItem<T>(l, r, OPERATION.MERGE));
                        // Push next item to process
                        stack.Push(nextitem);
                        continue;
                    }

                    int m = (l + r) / 2;
                    // Push two halves for dividing
                    stack.Push(new SortItem<T>(m + 1, r, OPERATION.DIVIDE));
                    stack.Push(new SortItem<T>(l, m, OPERATION.DIVIDE));
                }
                else
                {
                    // Merge two items

                    // Pop next item to process
                    SortItem<T> nextitem = stack.Pop();

                    //Console.WriteLine("Merge: l=" + l + "; m=" + r + "; r=" + nextitem.r);

                    // Merge   		
                    MergeOperation(v, l, r, nextitem.r);

                    /*for (int i = l; i <= nextitem.r; ++i) 
                        //Console.Write(v[i] + " ");
                    //Console.WriteLine();*/

                    // Pop next item to process
                    if (stack.Count == 0)
                        break;

                    SortItem<T> nextitemProcess = stack.Pop();
                    // Push merged array
                    stack.Push(new SortItem<T>(l, nextitem.r, OPERATION.MERGE));
                    // Push next item to process
                    stack.Push(nextitemProcess);
                }
            }
        }
    }
}
