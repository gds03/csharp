using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CustomComponents.Algorithms.Collections.Generic.Concurrent

{

    public sealed class ConcurrentQueue<T> // FIFO
    {
        private class Node
        {
            public volatile Node next;
            public T item;

            public Node(T item)
            {
                this.item = item;
                this.next = null;
            }

            /// <summary>
            ///     Compares if the current next field is equal to oldNode, and if it is, changes it to the new node.
            /// </summary>
            public bool CasNext(Node oldNode, Node newNode)
            {
                return this.next == oldNode && (Interlocked.CompareExchange(ref this.next, newNode, oldNode) == oldNode);
            }
        }

        private volatile Node m_head;
        private volatile Node m_tail;

        public ConcurrentQueue()
        {
            m_head = m_tail = new Node(default(T));
        }


        private bool SetHead(Node h /* head */, Node nh /* new head */)
        {
            return (m_head == h && Interlocked.CompareExchange(ref m_head, nh, h) == h);
        }

        private bool SetTail(Node t /* tail*/, Node nt /* new tail */)
        {
            return (m_tail == t && Interlocked.CompareExchange(ref m_tail, nt, t) == t);
        }

        public void Enqueue(T item)
        {
            Node n = new Node(item);

            do
            {
                Node t = m_tail;
                Node tn = t.next;

                // tn is not null, and this means that 2 steps are required (connect node to last node, and setup new tail)
                if (tn != null)
                {
                    SetTail(t, tn);         // update tail 
                    continue;
                }

                // tn is null and so one step is needed (connect tail to node through atomic way)
                if (t.CasNext(null, n))
                {
                    SetTail(t, n);
                    return;
                }
            }

            while (true);
        }

        public bool TryDequeue(out T item)
        {
            do
            {
                Node h = m_head;
                Node t = m_tail;

                Node hn = h.next;

                if (h == t)
                {
                    if (hn == null)
                    {
                        // queue is empty
                        item = default(T);
                        return false;
                    }
                    // some enqueue occurrs, we must adjust the tail
                    SetTail(t, hn);
                    continue;       // make another check
                }

                if (h != m_head || t != m_tail || hn == null)
                    continue;       // make another check

                if (SetHead(h, hn))
                {
                    item = h.item;
                    return true;
                }
            }
            while (true);
        }
    }
}
