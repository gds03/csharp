using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomComponents.Algorithms.Recursion
{
    public static class LinkedListRecursive
    {
        public static LinkedListNode<T> FindNode<T>(LinkedList<T> list, T value) 
            where T : IComparable<T>, IComparable
        {
            if (list == null)
                throw new ArgumentNullException("list");

            if (value == null)
                throw new ArgumentNullException("node");

            if (list.Count == 0)
                return null;

            // it is not the first
            return FindNodeR(list, list.First, value);
        }


        private static LinkedListNode<T> FindNodeR<T>(LinkedList<T> list, LinkedListNode<T> node, T value)
            where T : IComparable<T>, IComparable
        {
            if (node.Value.CompareTo(value) == 0 )
                return node;

            if (node.Next == null)
                return null;    // we did one turn around            

            return FindNodeR(list, node.Next, value);
        }






        //public static bool RemoveNode<T>(LinkedList<T> list, T value)
        //    where T : IComparable<T>, IComparable
        //{
        //    var node = FindNode(list, value);
        //    if (node == null)
        //        return false;

        //    node.Previous.Next = node.Next;
        //    node.Next.Previous = node.Previous;
        //    node = null;
        //    return true;
        //}
    }
}
