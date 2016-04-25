using VirtualNote.Kernel.DTO.Query.Issues.Index.Common;
using VirtualNote.Kernel.DTO.Query.Issues.Index.Tuple;

namespace VirtualNote.Kernel.DTO.Query.Issues.Index
{
    public sealed class IssueClientQueryList : IQueryDTO
    {
        public IssueProjectsData                    ProjectsData { get; set; }
        public IssueRequestsData<IssueClientQueryTuple> Requests { get; set; }
    }
}
