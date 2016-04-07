using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomComponents.Algorithms.Collections.Generic
{
    public struct KeyValuePair<TKey, TValue>
    {
        public TKey Key { get; set; }
        public TValue Value { get; set; }

        public KeyValuePair(TKey k, TValue v) : this()
        {
            Key = k;
            Value = v;
        }
    }


    public class Dictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>, ICollection<KeyValuePair<TKey, TValue>>
        where TValue : IComparable<TValue>
    {
        private const int DEFAULT_SIZE = 11;

        private readonly int m_initialSize;

        private Node[] m_hashArray;
        private int m_items, m_growthTimes;




        #region ctor



        public Dictionary()
            : this(0)
        {

        }

        public Dictionary(int initialCapacity)
        {
            if (initialCapacity < 0)
                throw new ArgumentException("initialCapacity <= 0");

            m_hashArray = new Node[m_initialSize = initialCapacity];
        }



        #endregion







        // 
        // public methods

        /// <summary>
        ///     Return the number of values stored in the dictionary. This method is an O(1) Operation.
        /// </summary>
        public int Count { get { return m_items; } }

        /// <summary>
        ///     Return the number of times the internal array grow. This method is an O(1) Operation.
        /// </summary>
        public int GrowthTimes { get { return m_growthTimes; } }

        /// <summary>
        ///     Return Miliseconds taken to grew the internal array. This method is an O(1) Operation.
        /// </summary>
        public uint GrowthOperationMiliseconds { get; private set; }


        /// <summary>
        ///     Add the specific KeyValuePair into the dictionary. This method is an O(1) Operation.
        /// </summary>
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        /// <summary>
        ///     Add the specific key and value into the dictionary. This method is approximatelly O(1) Operation.
        /// </summary>
        public void Add(TKey key, TValue item)
        {
            if (m_items == m_hashArray.Length * 4) { this.Grow(); }
           
            Node newNode = new Node(key, item);
            int bucket = GetBucketForKey(key);

            Node iter = m_hashArray[bucket];

            while (iter != null)
            {
                // linear search validation
                if (iter.InitialKeyHashCode == newNode.InitialKeyHashCode)
                    throw new InvalidOperationException("Dictionary does not allow duplicated keys");

                iter = iter.NextNode;
            }


            // unique, insert in the beggining of the list

            newNode.NextNode = m_hashArray[bucket];
            m_hashArray[bucket] = newNode;
            m_items++;
        }



        /// <summary>
        ///     Gets or sets the specific value into the key.
        ///     When setting, if the key is not present add a new entry, otherwise update existing entry.
        ///     This method is approximatelly O(1) Operation.
        /// </summary>
        /// <param name="key">The key where value will be stored</param>
        /// <returns>THe value associated to the key</returns>
        public TValue this[TKey key]
        {
            get
            {
                Node n = this.InternalSearch(key);
                if (n == null)
                    return default(TValue);

                return n.Data.Value;
            }

            set
            {
                Node n = this.InternalSearch(key);

                if (n == null)
                {
                    Add(key, value);
                }
                else
                {
                    n.Data.Value = value;
                }
            }
        }


        /// <summary>
        ///     Indicate if contains the specific item.key value.
        ///     This method is approximatelly O(1) Operation.
        /// </summary>
        /// <param name="item">Key value pair that contains at least the key.</param>
        /// <returns>THe value to the specific key</returns>
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            Node n = this.InternalSearch(item.Key);
            return n != null;
        }



        /// <summary>
        ///     Indicate if contains the specific key. 
        ///     This method is approximatelly O(1) Operation.
        /// </summary>
        /// <param name="key">The key to search for.</param>
        /// <returns>true if contains the key, otherwise false</returns>
        public bool ContainsKey(TKey key)
        {
            return this.InternalSearch(key) != null;
        }



        /// <summary>
        ///     Indicate if contains the specific value.
        ///     This method is O(N) Operation where N is Count.
        /// </summary>
        /// <param name="value">The value to search for.</param>
        /// <returns>true if contains the value, otherwise false</returns>
        public bool ContainsValue(TValue value)
        {
            foreach (var kvp in this)
            {
                if (kvp.Value.CompareTo(value) == 0)
                    return true;
            }
            return false;
        }





        /// <summary>
        ///     Remove the entry associated to the key.
        ///     This method is approximatelly O(1) Operation.
        /// </summary>
        /// <param name="key">key to remove</param>
        /// <returns>true if sucessfully removed, otherwise false</returns>
        public bool Remove(TKey key)
        {
            return this.InternalRemove(key) != null;
        }


        /// <summary>
        ///     Remove the entry associated to the KeyValuePair
        ///     This method is approximatelly O(1) Operation.
        /// </summary>
        /// <param name="key">item that holds the key.</param>
        /// <returns>true if sucessfully removed, otherwise false</returns>
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return this.Remove(item.Key);
        }


        /// <summary>
        ///     Get the enumerator to iterate over this collection.
        /// </summary>
        /// <returns>The enumerator</returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return new HashTableIterator(this);
        }


        /// <summary>
        ///     Clear all data.
        /// </summary>
        public void Clear()
        {
            m_hashArray = new Node[m_initialSize];
            m_items = m_growthTimes = 0;
            GrowthOperationMiliseconds = 0;
        }

       
        /// <summary>
        ///   Indicates if this collection is readonly.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }



      
        /// <summary>
        ///     Copies the elements of the ICollection<T> to an array of type KeyValuePair<TKey, TValue>, starting at the specified array index.
        /// </summary>
        /// <param name="array">Array that data will be copy to.</param>
        /// <param name="arrayIndex">Start at this index.</param>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            var enumerator = this.GetEnumerator();
            while( arrayIndex < array.Length && enumerator.MoveNext() )
            {
                KeyValuePair<TKey, TValue> kvp = enumerator.Current;
                array[arrayIndex++] = kvp;
            }
        }

       





        //
        // private methods

        private int GetBucketForKey(TKey k) { return (k.GetHashCode() & 0x7FFFFFFF) % m_hashArray.Length; }

        private int GetBucketForNewArray(Node n, int newArraySize) { return (n.InitialKeyHashCode & 0x7FFFFFFF) % newArraySize; }

        private void Grow()
        {
            Stopwatch counter = Stopwatch.StartNew();
            counter.Restart();
            int size = (m_hashArray.Length == 0) ? DEFAULT_SIZE : (2 * m_hashArray.Length + 1);
            Node[] newArray = new Node[size];
            for (int i = 0; i < m_hashArray.Length; i++)
            {
                Node iter = m_hashArray[i];
                while (iter != null)
                {
                    int idx = GetBucketForNewArray(iter, newArray.Length);      // recalculate idx
                    Node aux = iter.NextNode;
                    iter.NextNode = newArray[idx];                              // invert the order
                    newArray[idx] = iter;
                    iter = aux;
                }
            }
            m_hashArray = newArray;
            m_growthTimes++;
            counter.Stop();
            GrowthOperationMiliseconds += (uint)counter.ElapsedMilliseconds;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private Node InternalSearch(TKey k)
        {
            if (m_items == 0)
                return null;

            Node curr = m_hashArray[GetBucketForKey(k)];
            int kHash = k.GetHashCode();
            while (curr != null)
            {
                if (curr.InitialKeyHashCode == kHash)
                    return curr;

                else curr = curr.NextNode;
            }
            return null;
        }

        private Node InternalRemove(TKey k)
        {
            if (m_items == 0)
                return null;

            int bucket = GetBucketForKey(k);
            int kHash = k.GetHashCode();

            Node prev = null;
            Node curr = m_hashArray[bucket];

            while (curr != null)
            {
                if (curr.InitialKeyHashCode == kHash)
                {
                    if (prev == null) m_hashArray[bucket] = curr.NextNode;
                    else prev.NextNode = curr.NextNode;

                    m_items--;
                    return curr;
                }
                // advance
                prev = curr;
                curr = curr.NextNode;
            }
            return null;
        }






        #region Helper Classes


        class Node
        {
            internal readonly int InitialKeyHashCode;
            internal KeyValuePair<TKey, TValue> Data;
            internal Node NextNode;

            public Node(TKey key, TValue item)
            {
                Data = new KeyValuePair<TKey, TValue>(key, item);
                NextNode = null;
                InitialKeyHashCode = key.GetHashCode();
            }
        }




        class HashTableIterator : IEnumerator<KeyValuePair<TKey, TValue>>
        {
            private readonly Dictionary<TKey, TValue> m_hashTable;
            private Node m_prev, m_current;
            private int m_currentIdx;

            public HashTableIterator(Dictionary<TKey, TValue> hashTable)
            {
                if (hashTable == null)
                    throw new ArgumentNullException("hashTable");

                this.m_hashTable = hashTable;
                Reset();
            }

            public bool MoveNext()
            {
                if (m_current != null && m_current.NextNode != null)
                    m_current = m_current.NextNode; // try to advance before going down in the array.

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

            public KeyValuePair<TKey, TValue> Current
            {
                get
                {
                    if (m_current == null)
                        throw new NotSupportedException();

                    return m_current.Data;
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


        #endregion


    }


}
