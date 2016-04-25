using System;

namespace VirtualNote.Kernel.Contracts.Exceptions
{
    public sealed class IssueWasAlreadyTakedByAnotherMember : Exception
    {
        public IssueWasAlreadyTakedByAnotherMember(){
            
        }
        public IssueWasAlreadyTakedByAnotherMember(string message)
            : base(message) {

        }
    }
}
