using System.Collections.Generic;

namespace VirtualNote.Kernel.DTO.Query.Issues.Index.Common
{
    public sealed class IssueProjectsData
    {
        public IEnumerable<KeyIdValueString>       Projects { get; set; }
        public int                        ProjectSelectedId { get; set; }
        public IEnumerable<KeyIdValueString>        Filters { get; set; }
    }
}
