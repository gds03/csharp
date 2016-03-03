using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomComponents.Algorithms.Collections.Generic
{
    public class HashTable<T> : IEnumerable<T>, ICollection<T>
        where T : class, IComparable<T>
        
    {
        class Node<T>
            where T : IComparable<T>
        {
            public T val { get; internal protected set; }
            public Node<T> next { get; internal protected set; }
            public int hash { get; internal protected set; } 

            public Node(T item)
            {
                val = item;
                next = null;
                hash = 0;
            }
            public Node(T item, int code)
            {
                val = item;
                next = null;
                hash = code;
            }
        }




        class HashTableIterator : IEnumerator<T>
        {
            private readonly HashTable<T> m_hashTable;
            private Node<T> m_prev, m_current;
            private int m_currentIdx;

            public HashTableIterator(HashTable<T> hashTable)
            {
                if (hashTable == null)
                    throw new ArgumentNullException("hashTable");

                this.m_hashTable = hashTable;
                Reset();
            }

            public bool MoveNext()
            {
                if (m_current != null && m_current.next != null)
                    m_current = m_current.next; // try to advance before going down in the array.
                
                else
                {
                    m_current = null;

                    // iterate over the array down
                    while (m_current == null)
                    {
                        m_currentIdx++;
                        
                        // invariant check - we are at the end
                        if (m_currentIdx >= m_hashTable.m_hashArray.Length)
                            return false;

                        m_current = this.m_hashTable.m_hashArray[m_currentIdx];
                    }
                }

               
               

                return true;
            }

            public T Current
            {
                get {
                    if (m_current == null)
                        return null;

                    return m_current.val;
                }
            }

            public void Dispose()
            {
                
            }

            object System.Collections.IEnumerator.Current
            {
                get { return this; }
            }

            

            public void Reset()
            {
                this.m_prev = this.m_current = null;
                this.m_currentIdx = -1;
            }
        }


        const int DEFAULT_SIZE = 11;
        
        Node<T>[] m_hashArray;
	    int m_bucketSize, m_elems;

        


        #region ctor 



        public HashTable()
        {
            m_hashArray = new Node<T>[DEFAULT_SIZE];
            m_bucketSize = DEFAULT_SIZE;
            m_elems = 0;
        }
	
	    public HashTable( int dim )
        {
            if (dim <= 0)
                throw new ArgumentException("dim <= 0");

		    m_hashArray = new Node<T>[dim];
		    m_bucketSize=dim; m_elems=0;
	    }



        #endregion

        // 
        // public methods


        public int Count {
            get
            {
                return m_elems;
            }
        }

        public T Search(T e)
        {
            if (m_elems == 0)
                return null;

            Node<T> corr = m_hashArray[Index(e)];
            while (corr != null)
            {
                if (corr.val.CompareTo(e) == 0)
                    return corr.val;

                else corr = corr.next;
            }
            return null;
        }


        public void Add(T item)
        {
            Node<T> n = new Node<T>(item, item.GetHashCode());
            if (m_elems + 1 > m_bucketSize) 
                Grow();

            int bucket = ReadAndMask(n, m_bucketSize);

            n.next = m_hashArray[bucket];
            m_hashArray[bucket] = n;
            m_elems++;
        }

        public bool Remove(T item)
        {
            if (m_elems == 0)
                return false;

            int i = Index(item);

            Node<T> prev = null;
            Node<T> corr = m_hashArray[i];
            
            while (corr != null)
            {
                if (corr.val.CompareTo(item) == 0 )
                {
                    if (prev == null) m_hashArray[i] = corr.next;
                    else prev.next = corr.next;

                    m_elems--;
                    return true;
                }
                // advance
                prev = corr;
                corr = corr.next;
            } 
            return false;
        }


        public IEnumerator<T> GetEnumerator()
        {
            return new HashTableIterator(this);
        }


        public void Clear()
        {
            m_hashArray = new Node<T>[m_hashArray.Length];
        }

        public bool Contains(T item)
        {
            return Search(item) != null;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (arrayIndex < 0)
                throw new ArgumentException("arrayIndex < 0");

            foreach(var item in this)
            {
                if (arrayIndex < array.Length)
                    array[arrayIndex++] = item;
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }



        


        //
        // private methods

        private int Index(T value) { return (value.GetHashCode() & 0x7FFFFFFF) % m_bucketSize; }

        private int ReadAndMask(Node<T> n, int size) { return (n.hash & 0x7FFFFFFF) % size; }

        private void Grow()
        {
            int new_size = 2 * m_bucketSize + 1;
            Node<T>[] newArray = new Node<T>[new_size];
            for (int i = 0; i < m_bucketSize; i++)
            {
                Node<T> iter = m_hashArray[i];
                while (iter != null)
                {
                    int idx = ReadAndMask(iter, new_size);      // recalculate idx
                    Node<T> aux = iter.next;
                    iter.next = newArray[idx];                  // invert the order
                    newArray[idx] = iter;
                    iter = aux;
                }
            }
            m_bucketSize = new_size;
            m_hashArray = newArray;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }


}
