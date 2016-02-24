using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomComponents.Algorithms.Collections
{
    public class LinkedList<TValue> : IEnumerable<TValue>
    {
        class LinkedNode
        {
            public LinkedNode Previous { get; internal protected set; }
            public LinkedNode Next { get; internal protected set; }
            public TValue Data { get; private set; }

            public LinkedNode(TValue data)
            {
                Data = data;
            }
        }


        class LinkedListEnumerator : IEnumerator<TValue>
        {
            private readonly LinkedList<TValue> m_list;
            private LinkedNode m_current;

            public LinkedListEnumerator(LinkedList<TValue> list)
            {
                m_list = list;
                Reset();
            }

            public void Reset()
            {
                m_current = m_list.m_head_sentinel;
            }

            public TValue Current
            {
                get { return m_current.Data; }
            }

            public void Dispose()
            {
                
            }

            object System.Collections.IEnumerator.Current
            {
                get { return this; }
            }

            public bool MoveNext()
            {
                if (m_current.Next != m_list.m_head_sentinel)
                {
                    m_current = m_current.Next;
                    return true;

                } 
                
                return false;
            }
        }


        //
        // private fields
        
        private readonly LinkedNode m_head_sentinel;
        private int m_count;


        #region ctor

        public LinkedList()
        {
            m_head_sentinel = new LinkedNode(default(TValue));
            m_head_sentinel.Next = m_head_sentinel.Previous = m_head_sentinel;
            m_count = 0;
        }


        public LinkedList(IEnumerable<TValue> enumerable) : this()
        {
            if (enumerable == null)
                throw new ArgumentNullException("enumerable");

            foreach(TValue data in enumerable)
            {
                AddLast(data);
            }
        }

        #endregion


        //
        // public methods

        public int Count
        {
            get { return m_count; }
        }

        public TValue First
        {
            get { return IsEmpty() ? default(TValue) : m_head_sentinel.Next.Data; }
        }

        public TValue Last
        {
            get { return IsEmpty() ? default(TValue) : m_head_sentinel.Previous.Data; }
        }

        public void AddFirst(TValue data)
        {

            LinkedNode newNode = new LinkedNode(data);
            m_count++;

            // connect - first the node refs.
            newNode.Next = m_head_sentinel.Next;
            newNode.Next.Previous = newNode;
            newNode.Previous = m_head_sentinel;

            // change sentinel head
            m_head_sentinel.Next = newNode;             
        }

        /// <summary>
        ///     Add the specified data at the end of the collection.
        /// </summary>
        public void AddLast(TValue data)
        {
            LinkedNode newNode = new LinkedNode(data);
            m_count++;

            // connect - first the node refs.
            newNode.Next = m_head_sentinel.Previous.Next;
            newNode.Previous = m_head_sentinel.Previous;
            m_head_sentinel.Previous.Next = newNode;

            // change sentinel tail
            m_head_sentinel.Previous = newNode;         
        }

        /// <summary>
        ///     Remove the first element in the collection.
        /// </summary>
        /// <returns>If collection is empty return default(TValue), otherwise remove the first element and return the data within.</returns>
        public TValue RemoveFirst()
        {
            if (IsEmpty())
                return default(TValue);

            TValue data = m_head_sentinel.Next.Data;
            m_count--;

            // disconnect - go over the node
            LinkedNode firstNode = m_head_sentinel.Next;
            firstNode.Next.Previous = m_head_sentinel;

            // change sentinel head
            m_head_sentinel.Next = firstNode.Next;

            return data;
        }

        /// <summary>
        ///     Remove the last element in the collection
        /// </summary>
        /// <returns>If collection is empty return default(TValue), otherwise remove the last element and return the data within.</returns>
        public TValue RemoveLast()
        {
            if (IsEmpty())
                return default(TValue);

            TValue data = m_head_sentinel.Previous.Data;
            m_count--;

            // disconnect - go over the node
            LinkedNode penultimate = m_head_sentinel.Previous.Previous;
            penultimate.Next = m_head_sentinel;

            // change sentinel head
            m_head_sentinel.Previous = penultimate;

            return data;
        }

        /// <summary>
        ///     Remove the element at index position.
        /// </summary>
        /// <returns>If collection is empty return default(TValue), otherwise remove the Nth element and return the data within.</returns>
        public TValue RemoveAt(int index)
        {
            if (index < 0)
                throw new ArgumentException("index < 0");

            if (index >= m_count)
                throw new InvalidOperationException("index is higher than total count of elements present in the collection");

            if (IsEmpty())
                return default(TValue);

            LinkedNode iter = m_head_sentinel.Next;
            for (int i = 0; i < index; iter = iter.Next, i++) ;

            TValue data = iter.Data;
            m_count--;

            // disconnect - iter
            iter.Next.Previous = iter.Previous;
            iter.Previous.Next = iter.Next;

            return data;
        }

        /// <summary>
        ///     Reverses the current linked list.
        /// </summary>
        public void Reverse()
        {
            if (IsEmpty() || Count == 1)
                return;

            LinkedNode n = m_head_sentinel;
            for (int i = 0; i <= Count; i++)        // equals because sentinel is included
            {
                // swap references
                LinkedNode next = n.Next;
                n.Next = n.Previous;
                n.Previous = next;

                n = next;
            }
        }


        public IEnumerator<TValue> GetEnumerator()
        {
            return new LinkedListEnumerator(this);
        }


        //
        // private methods
        private bool IsEmpty()
        {
            return m_count == 0;    // || m_head.next == null
        }

              

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
