using System;

namespace VirtualNote.Kernel.Contracts.Exceptions
{
    public sealed class ProjectDisabledException : Exception
    {
        public ProjectDisabledException(string message)
            : base(message)
        {

        }
    }
}
