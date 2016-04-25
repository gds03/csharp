using System;

namespace VirtualNote.Kernel.Contracts.Exceptions
{
    public sealed class ServiceAccessDeniedException : Exception
    {
        public ServiceAccessDeniedException(String message) : base(message)
        {

        }
    }
}
