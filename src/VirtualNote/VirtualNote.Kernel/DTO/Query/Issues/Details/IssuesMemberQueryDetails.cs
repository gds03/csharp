using System;

namespace VirtualNote.Kernel.DTO.Query.Issues.Details
{
    public sealed class IssuesMemberQueryDetails : IssuesClientQueryDetails
    {
        public String ClientName { get; set; }  // Faz parte de initial data para os membros
        public bool ShowDeleteButton { get; set; }
    }
}
