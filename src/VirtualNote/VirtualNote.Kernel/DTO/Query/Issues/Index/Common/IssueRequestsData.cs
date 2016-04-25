using System.Collections.Generic;
using VirtualNote.Kernel.DTO.Query.Issues.Index.Tuple;

namespace VirtualNote.Kernel.DTO.Query.Issues.Index.Common
{
    public sealed class IssueRequestsData<T> where T : IIssueQueryTyple
    {
        public IEnumerable<T>           Requests { get; set; }
        public IssueRequestsInfo        RequestsInfo { get; set; }
        
    }
}
