using System;

namespace VirtualNote.Kernel.DTO.Query.Issues.Index.Tuple
{
    public sealed class IssueMemberQueryTuple : IssueClientQueryTuple
    {
        public String ReportedByClient { get; set; }
    }
}
