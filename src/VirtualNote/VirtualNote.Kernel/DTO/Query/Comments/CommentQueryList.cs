using System;
using VirtualNote.Kernel.Types;

namespace VirtualNote.Kernel.DTO.Query.Comments
{
    public sealed class CommentQueryList : IQueryDTO
    {
        public int IssueId { get; set; }
        public string IssueShortDescription { get; set; }
        public string IssueReportedBy { get; set; }
        public DateTime IssueCreatedAt { get; set; }
        public string IssueMemberSolving { get; set; }

        public PaginatedList<CommentQueryDetails> Data { get; set; }
    }
}
