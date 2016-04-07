using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomComponents.Algorithms.Collections.Generic
{
    /// <summary>
    ///     Represents a circular List where the lastNode points to the firstNode.
    /// </summary>
    public class CircularList<TValue> : IEnumerable<TValue>
    {
        class SimpleNode
        {
            public SimpleNode Next { get; internal protected set; }

            public TValue Data { get; internal protected set; }

            public SimpleNode(TValue data)
            {
                Data = data;
            }
        }

        class CircularListEnumerator : IEnumerator<TValue>
        {
            private readonly CircularList<TValue> m_list;
            private SimpleNode m_iterNode;

            public CircularListEnumerator(CircularList<TValue> list)
            {
                if (list == null)
                    throw new ArgumentNullException("list");

                this.m_list = list;
                Reset();                
            }

            public void Reset()
            {
                this.m_iterNode = null;
            }


            public TValue Current
            {
                get
                {
                    if (this.m_iterNode == null)
                        return default(TValue);

                    return this.m_iterNode.Data;
                }
            }

            public void Dispose() { }

            object System.Collections.IEnumerator.Current
            {
                get { return this; }
            }

            public bool MoveNext()
            {
                if (m_list.IsEmpty())
                    return false;

                if (m_iterNode == null)
                {
                    m_iterNode = m_list.m_tail.Next;
                }
                else
                {
                    if (m_iterNode.Next == m_list.m_tail.Next)
                        return false;

                    m_iterNode = m_iterNode.Next;
                }

                return true;
            }           
        }


        private SimpleNode m_tail;
        private int m_count;



        #region Constructors


        public CircularList()
        {
            m_count = 0;
        }


        public CircularList(IEnumerable<TValue> enumerable) : this()
        {
            if (enumerable == null)
                throw new ArgumentNullException("enumerable");

            // add in FIFO order
            foreach (TValue data in enumerable)
                AddLast(data);
        }

        #endregion



        //  
        // public methods

        /// <summary>
        ///     Add a specific data to the start of the circular list - O(1)
        /// </summary>
        public void AddFirst(TValue data)
        {
            SimpleNode newNode = new SimpleNode(data);

            if (IsEmpty())
            {
                newNode.Next = newNode;     // points to himself <circular>
                m_tail = newNode;
            }
            else
            {
                newNode.Next = m_tail.Next;
                m_tail.Next = newNode;
            }

            m_count++;
        }



        /// <summary>
        ///     Add specific data to the end of the circular list - O(1)
        ///     Similiar with push operation.
        /// </summary>
        public void AddLast(TValue data)
        {
            SimpleNode newNode = new SimpleNode(data);

            if (IsEmpty())
            {
                newNode.Next = newNode;     // points to himself <circular>
            }
            else
            {
                newNode.Next = m_tail.Next; // keep reference of the first element.
                m_tail.Next = newNode;
            }

            m_tail = newNode;   // update tail node
            m_count++;
        }


        /// <summary>
        ///     Remove the first element in the collection. 
        /// </summary>
        public bool RemoveFirst(out TValue data)
        {
            data = default(TValue);
            if (IsEmpty())
                return false;

            data = m_tail.Next.Data;

            if (Count == 1)
            {
                // is the tail so remove tail
                m_tail = null;
            }
            else
            {
                // disconnect
                m_tail.Next = m_tail.Next.Next;
            }

            return true;
        }


        /// <summary>
        ///     Remove the last element in the list - O(N).
        ///     Similiar with Pop() operation.
        /// </summary>
        public bool RemoveLast(out TValue data)
        {
            data = default(TValue);
            if (IsEmpty())
                return false;

            // disconnect the node - O(N) because we need to disconnect prev than m_tail.
            SimpleNode iter = m_tail;
            while ( iter.Next != m_tail )        // go around
                iter = iter.Next;

            // data to be removed is on tail
            data = m_tail.Data;

            if (Count == 1)
            {
                // is the tail so remove tail
                m_tail = null;
            }
            else
            {
                // disconect
                iter.Next = iter.Next.Next;

                // update tail
                m_tail = iter;
            }

            m_count--;
            return true;
        }



        public IEnumerator<TValue> GetEnumerator()
        {
            return new CircularListEnumerator(this);
        }

        

        /// <summary>
        ///     Returns the number of elements within the collection
        /// </summary>
        public int Count
        {
            get { return m_count; }
        }




        public void Reverse()
        {
            if( Count <= 1 )
                return;

            SimpleNode head;
            SimpleNode prev = null;
            SimpleNode iter = head = m_tail.Next;

            for (int i = 0; i < Count; i++ )
            {
                // save next since we will change it.
                SimpleNode next = iter.Next;

                // change - next is previous item.
                iter.Next = prev;

                prev = iter;
                iter = next;
            }


            m_tail = head;          // tail is transformed in "head"           
            m_tail.Next = prev;     // circular is guaranteed with this pointing to the last element.
        }



       

        //
        // private methods
        private bool IsEmpty()
        {
            return m_tail == null;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }


      
    }
}
