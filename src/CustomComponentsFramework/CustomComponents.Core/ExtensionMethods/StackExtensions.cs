using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomComponents.Core.ExtensionMethods
{
    public static class StackExtensions
    {
        public static void CopyFromLast<T>(this Stack<T> stack,
            ICollection<T> collection)
        {
            if (stack.Count == 0)
                return;

            Stack<T> temp = new Stack<T>(stack);
            T element;

            while ((element = temp.Pop()) != null)
            {
                collection.Add(element);
            }
        }
    }
}
