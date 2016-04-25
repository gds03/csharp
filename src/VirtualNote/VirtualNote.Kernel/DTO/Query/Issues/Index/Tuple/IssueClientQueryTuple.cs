using System;

namespace VirtualNote.Kernel.DTO.Query.Issues.Index.Tuple
{
    // Representa um issue a mostrar ao cliente
    public class IssueClientQueryTuple : IIssueQueryTyple
    {
        public int IssueId { get; set; }
        public DateTime CreatedAt { get; set; }
        public String ShortDescription { get; set; }
        public TypeEnum Type { get; set; }
        public PriorityEnum Priority { get; set; }
        public StateEnum State { get; set; }
        public int NumComments { get; set; }

        public int ProjectId { get; set; }
        public String ProjectName { get; set; }
        public String MemberSolvingName { get; set; }
    }
}
