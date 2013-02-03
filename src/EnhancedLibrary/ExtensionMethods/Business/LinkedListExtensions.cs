using System;
using System.Collections.Generic;

namespace EnhancedLibrary.ExtensionMethods.Business
{
    public static class LinkedListExtensions
    {
        /// <summary>
        ///     Get the index node within the list.
        ///     This method has a O(N) performance, where N is the index.
        ///     
        /// </summary>
        /// <exception cref="InvalidOperationException">When Count of the list is 0</exception>
        /// <exception cref="IndexOutOfRangeException">Index >= list.Count</exception>
        public static T GetIndex<T>(this LinkedList<T> list, int index)
        {
            if ( list == null )
                throw new ArgumentNullException("list");

            if ( list.Count == 0 )
                throw new InvalidOperationException("list doesn't contain any elements");

            if ( index >= list.Count )
                throw new IndexOutOfRangeException();

            LinkedListNode<T> node = list.First;

            while ( index-- > 0 )
            {
                node = node.Next;
            }

            return node.Value;
        }
    }
}
