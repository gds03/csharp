using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using SCG = System.Collections.Generic;

namespace CustomComponents.Algorithms.Collections.Generic
{
    public class LinkedNode<TValue>
        where TValue : IComparable<TValue>, IComparable
    {
        public LinkedNode<TValue> Previous { get; internal protected set; }
        public LinkedNode<TValue> Next { get; internal protected set; }
        public LinkedList<TValue> List { get; internal protected set; }

        public TValue Data { get; private set; }

        public LinkedNode(LinkedList<TValue> list, TValue data)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            List = list;
            Data = data;
        }
    }

    [Serializable]
    public class LinkedList<TValue> : SCG.IEnumerable<TValue>, SCG.ICollection<TValue>, SCG.IReadOnlyCollection<TValue>, ISerializable
        where TValue : IComparable<TValue>, IComparable
    {
        class LinkedListEnumerator<TValue> : SCG.IEnumerator<TValue>
            where TValue : IComparable<TValue>, IComparable
        {
            private readonly LinkedList<TValue> m_list;
            private LinkedNode<TValue> m_current;

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

        private readonly LinkedNode<TValue> m_head_sentinel;
        private int m_count;
        private const string ITEMS_SERIALIZATION_KEY = "items";


        #region ctor

        public LinkedList()
        {
            m_head_sentinel = new LinkedNode<TValue>(this, default(TValue));
            this.ResetItems();
        }




        public LinkedList(SCG.IEnumerable<TValue> enumerable)
            : this()
        {
            if (enumerable == null)
                throw new ArgumentNullException("enumerable");

            foreach(TValue data in enumerable)
            {
                AddLast(data);
            }
        }


        protected LinkedList(SerializationInfo info, StreamingContext context) 
            : this(( TValue[] ) info.GetValue(ITEMS_SERIALIZATION_KEY, typeof(TValue[])))
        {
          
        }

        #endregion


        //
        // public methods

        /// <summary>
        ///     Return how many items are in the collection
        /// </summary>
        public int Count
        {
            get { return m_count; }
        }

        /// <summary>
        ///     Return the first node in the list or null if is none.
        /// </summary>
        public LinkedNode<TValue> First
        {
            get { return IsEmpty() ? null : m_head_sentinel.Next; }
        }

        /// <summary>
        ///     Return the last node in the list or null if is none.
        /// </summary>
        public LinkedNode<TValue> Last
        {
            get { return IsEmpty() ? null : m_head_sentinel.Previous; }
        }


        /// <summary>
        ///     Add the specified data at the beginning of the collection.
        /// </summary>
        public LinkedNode<TValue> AddFirst(TValue data)
        {
            LinkedNode<TValue> newNode = new LinkedNode<TValue>(this, data);
            this.AddFirstInternal(newNode);
            return newNode;
        }


        /// <summary>
        ///     Add the specified data at the beginning of the collection.
        /// </summary>
        public LinkedNode<TValue> AddFirst(LinkedNode<TValue> node)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            if (node.List == this)
                throw new InvalidOperationException("node already on this list");

            node.List = this;
            this.AddFirstInternal(node);
            return node;
        }


        

        /// <summary>
        ///     Add the specified data at the end of the collection.
        /// </summary>
        public LinkedNode<TValue> AddLast(TValue data)
        {
            LinkedNode<TValue> newNode = new LinkedNode<TValue>(this, data);
            this.AddLastInternal(newNode);
            return newNode;
        }


        /// <summary>
        ///     Add the specified data at the end of the collection.
        /// </summary>
        public LinkedNode<TValue> AddLast(LinkedNode<TValue> node)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            if (node.List == this)
                throw new InvalidOperationException("node already on this list");

            node.List = this;
            this.AddLastInternal(node);
            return node;
        }

        /// <summary>
        ///     Add the nodeToAdd after node.
        /// </summary>
        public void AddAfter(LinkedNode<TValue> node, LinkedNode<TValue> nodeToAdd)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            if (node.List != this)
                throw new InvalidOperationException("node does not belong in this list");

            if (nodeToAdd == null)
                throw new ArgumentNullException("nodeToAdd");

            if (nodeToAdd.List == this)
                throw new InvalidOperationException("nodeToAdd already belongs to this list");

            m_count++;

            // connect - node
            nodeToAdd.Next = node.Next;
            nodeToAdd.Previous = node;

            // connect - outsiders.
            node.Next.Previous = nodeToAdd;            
            node.Next = nodeToAdd;
            nodeToAdd.List = this;
        }

        /// <summary>
        ///     Add the item after node.
        /// </summary>
        public LinkedNode<TValue> AddAfter(LinkedNode<TValue> node, TValue item)
        {
            LinkedNode<TValue> newNode = new LinkedNode<TValue>(this, item);
            AddAfter(node, newNode);
            return newNode;
        }


        /// <summary>
        ///     Add the nodeToAdd before node.
        /// </summary>
        public void AddBefore(LinkedNode<TValue> node, LinkedNode<TValue> nodeToAdd)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            if (node.List != this)
                throw new InvalidOperationException("node does not belong in this list");

            if (nodeToAdd == null)
                throw new ArgumentNullException("nodeToAdd");

            if (nodeToAdd.List == this)
                throw new InvalidOperationException("nodeToAdd already belongs to this list");

            m_count++;

            // connect - node
            nodeToAdd.Previous = node.Previous;
            nodeToAdd.Next = node;

            // connect - outsiders
            node.Previous.Next = nodeToAdd;
            node.Previous = nodeToAdd;

            nodeToAdd.List = this;
        }


        /// <summary>
        ///     Add the item before node.
        /// </summary>
        public LinkedNode<TValue> AddBefore(LinkedNode<TValue> node, TValue item)
        {
            LinkedNode<TValue> newNode = new LinkedNode<TValue>(this, item);
            AddBefore(node, newNode);
            return newNode;
        }

        
        /// <summary>
        ///     Remove the first element in the collection.
        /// </summary>
        /// <returns>If collection is empty return default(TValue), otherwise remove the first element and return the data within.</returns>
        public LinkedNode<TValue> RemoveFirst()
        {
            if (IsEmpty())
                return null;

            LinkedNode<TValue> node = m_head_sentinel.Next;
            node.List = null;
            m_count--;

            // disconnect - go over the node
            LinkedNode<TValue> firstNode = m_head_sentinel.Next;
            firstNode.Next.Previous = m_head_sentinel;

            // change sentinel head
            m_head_sentinel.Next = firstNode.Next;
            return node;
        }

        /// <summary>
        ///     Remove the last element in the collection
        /// </summary>
        /// <returns>If collection is empty return default(TValue), otherwise remove the last element and return the data within.</returns>
        public LinkedNode<TValue> RemoveLast()
        {
            if (IsEmpty())
                return null;

            LinkedNode<TValue> node = m_head_sentinel.Previous;
            node.List = null;
            m_count--;

            // disconnect - go over the node
            LinkedNode<TValue> penultimate = m_head_sentinel.Previous.Previous;
            penultimate.Next = m_head_sentinel;

            // change sentinel head
            m_head_sentinel.Previous = penultimate;
            return node;
        }

        /// <summary>
        ///     Remove the element at index position.
        /// </summary>
        /// <returns>If collection is empty return default(TValue), otherwise remove the Nth element and return the data within.</returns>
        public LinkedNode<TValue> RemoveAt(int index)
        {
            if (index < 0)
                throw new ArgumentException("index < 0");

            if (index >= m_count)
                throw new InvalidOperationException("index is higher than total count of elements present in the collection");

            if (IsEmpty())
                return null;

            LinkedNode<TValue> iter = m_head_sentinel.Next;
            for (int i = 0; i < index; iter = iter.Next, i++) ;

            iter.List = null;
            m_count--;

            // disconnect - iter
            iter.Next.Previous = iter.Previous;
            iter.Previous.Next = iter.Next;

            return iter;
        }

        /// <summary>
        ///     Remove the specific node from this list.
        /// </summary>
        public void Remove(LinkedNode<TValue> node)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            if (node.List != this)
                throw new InvalidOperationException("node does not belong to this list");

            node.Previous.Next = node.Next;
            node.Next.Previous = node.Previous;
            node.List = null;
        }

        /// <summary>
        ///     Finds the first node that holds item data.
        /// </summary>
        public LinkedNode<TValue> FindFirst(TValue item)
        {
            LinkedNode<TValue> node = GetNode(item);
            if (node != null)
                return node;

            return null;
        }

        /// <summary>
        ///     Finds the last node that holds item data.
        /// </summary>
        public LinkedNode<TValue> FindLast(TValue item)
        {
            // iterate backwards
            LinkedNode<TValue> iter = m_head_sentinel.Previous;
            for (; iter.Previous != m_head_sentinel; iter = iter.Previous)
                if (iter.Data.CompareTo(item) == 0)
                    return iter;

            return null;
        }


        /// <summary>
        ///     Reverses the current linked list.
        /// </summary>
        public LinkedList<TValue> Reverse()
        {
            if (IsEmpty() || Count == 1)
                return this;

            LinkedNode<TValue> n = m_head_sentinel;
            for (int i = 0; i <= Count; i++)        // equals because sentinel is included
            {
                // swap references
                LinkedNode<TValue> next = n.Next;
                n.Next = n.Previous;
                n.Previous = next;

                n = next;
            }

            return this;
        }

        /// <summary>
        ///     Provides the enumerator to iterate over the collection.
        /// </summary>
        /// <returns></returns>
        public SCG.IEnumerator<TValue> GetEnumerator()
        {
            return new LinkedListEnumerator<TValue>(this);
        }

        /// <summary>
        ///     Adds an item at the end of the list. Same as AddLast
        /// </summary>
        public void Add(TValue item)
        {
            this.AddLast(item);
        }

        /// <summary>
        ///     Discarts all previosly elements.
        /// </summary>
        public void Clear()
        {
            this.ResetItems();
        }

        /// <summary>
        ///     Returns indicating if the specific item is on the collection.
        /// </summary>
        public bool Contains(TValue item)
        {
            return this.GetNode(item) != null;
        }

        /// <summary>
        ///     Copy all elements in the collection to array from arrayIndex position.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(TValue[] array, int arrayIndex)
        {
            int idx = 0;
            foreach(var item in this)
            {
                array[arrayIndex++] = item;
                idx++;
            }
        }

        /// <summary>
        ///     Return indicating if is a readonly structure.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        ///     Remove the given item from the collection and return if the operation was sucessful.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(TValue item)
        {
            LinkedNode<TValue> node = GetNode(item);
            if (node == null) return false;
            Remove(node);
            return true;
        }



        /// <summary>
        ///     Get the LinkedNode that holds the data for item.
        /// </summary>
        public LinkedNode<TValue> GetNode(TValue item)
        {
            LinkedNode<TValue> iter = m_head_sentinel.Next;
            for (; iter.Next != m_head_sentinel; iter = iter.Next)
                if (iter.Data.CompareTo(item) == 0)
                    return iter;

            return null;
        }


        /// <summary>
        ///     Return the LinkedNode that holds the item.
        /// </summary>
        public LinkedNode<TValue> this[TValue item]
        {
            get
            {
                return GetNode(item);
            }
        }




        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            TValue[] data = new TValue[m_count];
            CopyTo(data, 0);
            info.AddValue(ITEMS_SERIALIZATION_KEY, data, typeof(TValue[]));
        }




        //
        // private methods      


        private void AddFirstInternal(LinkedNode<TValue> node)
        {
            m_count++;

            // connect - first the node refs.
            node.Next = m_head_sentinel.Next;
            node.Next.Previous = node;
            node.Previous = m_head_sentinel;

            // change sentinel head
            m_head_sentinel.Next = node;
        }



        private LinkedNode<TValue> AddLastInternal(LinkedNode<TValue> node)
        {
            m_count++;

            // connect - first the node refs.
            node.Next = m_head_sentinel.Previous.Next;
            node.Previous = m_head_sentinel.Previous;
            m_head_sentinel.Previous.Next = node;

            // change sentinel tail
            m_head_sentinel.Previous = node;
            return node;
        }


        private void ResetItems()
        {
            m_head_sentinel.Next = m_head_sentinel.Previous = m_head_sentinel;
            m_count = 0;
        }


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
