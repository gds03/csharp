using System;

namespace VirtualNote.Kernel.DTO.Query.Comments
{
    public sealed class CommentQueryDetails : IQueryDTO
    {
        public int CommentId { get; set; }
        public DateTime CreatedAt { get; set; }
        public String Description { get; set; }

        public String ReportedBy { get; set; }
        public bool CanEdit { get; set; }
    }
}
