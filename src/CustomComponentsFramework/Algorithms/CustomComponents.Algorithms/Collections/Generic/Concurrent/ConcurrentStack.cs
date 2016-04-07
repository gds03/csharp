using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CustomComponents.Algorithms.Collections.Generic.Concurrent
{
    public sealed class ConcurrentStack<T> // LIFO
    {
        private class Node
        {
            public Node next;
            public T item;

            public Node(T item)
            {
                this.item = item;
                next = null;
            }
        }

        private volatile Node m_top;

        public void Push(T item)
        {
            Node node = null;

            do
            {
                Node t = m_top;         // aquire (read barrier) and provides a fastest way to check if t is yet the same as the top later in if checking

                if (node == null) node = new Node(item);
                node.next = t;

                if (m_top == t && Interlocked.CompareExchange(ref m_top, node, t) == t)
                    return;
            }

            while (true);
        }

        public bool TryPop(out T item)
        {
            do
            {
                Node t = m_top;         // aquire (read barrier)
                if (t == null)
                {
                    item = default(T);
                    return false;
                }

                if (m_top == t && Interlocked.CompareExchange(ref m_top, t.next, t) == t)
                {
                    item = t.item;
                    return true;
                }
            }
            while (true);
        }
    }
}
