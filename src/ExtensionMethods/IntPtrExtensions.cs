using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace ExtensionMethods
{
    public static class IntPtrExtensions
    {
        [DllImport("user32.dll", SetLastError=true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);


        /// <summary>
        ///     Return the processId that is associated with the thread that created the hWnd (handle of the window)
        /// </summary>
        public static uint GetProcessId(this IntPtr hWnd)
        {
            uint pId;
            GetWindowThreadProcessId(hWnd, out pId);

            if ( pId <= 0 )
                throw new InvalidOperationException();

            return pId;
        }
    }
}
