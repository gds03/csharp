using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CustomComponents.Core.ExtensionMethods;

namespace CustomComponents.Algorithms.Threading
{
    public class SynchronousChannel<T>
    {
        private readonly LinkedList<T> queue = new LinkedList<T>();

        public void Send(T message)
        {
            lock(queue)
            {
                LinkedListNode<T> wb = queue.AddLast(message);

                try { SyncUtils.Wait(queue, wb, Timeout.Infinite); }        // granular control over the condition variable (threads parked)
                catch(ThreadInterruptedException)
                {
                    if (queue.Count > 0 && wb.List == queue)
                        queue.Remove(wb);

                    throw;
                }
            }
        }

        public T Receive()
        {
            lock(queue)
            {
                if (queue.Count == 0)
                    return default(T);

                SyncUtils.Notify(queue, queue.First);                       // granular control over the condition variable (threads unpark)
                return queue.GetFirstAndRemove();
            }
        }
    }
}
