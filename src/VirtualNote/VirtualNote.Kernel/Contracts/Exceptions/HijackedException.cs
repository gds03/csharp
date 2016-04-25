using System;

namespace VirtualNote.Kernel.Contracts.Exceptions
{
    public sealed class HijackedException : Exception
    {
        public HijackedException(string message) : base(message)
        {

        }
    }
}
