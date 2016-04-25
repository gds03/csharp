using System;
using System.ComponentModel.DataAnnotations;
using VirtualNote.Kernel.DTO.Query.Issues.Details.InitialData;

namespace VirtualNote.Kernel.DTO.Query.Issues.Details
{
    public class IssuesClientQueryDetails : IQueryDTO
    {
        public IssueQueryInitialData InitialData { get; set; }

        public int IssueId { get; set; }
        public int ProjectId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUpdateAt { get; set; }

        [Required(ErrorMessage = "Field is required")]
        [StringLength(1000, ErrorMessage = "Description must have between 5 and 1000 characters", MinimumLength = 5)]
        public String LongDescription { get; set; }

        [Required(ErrorMessage = "Field is required")]
        [StringLength(100, ErrorMessage = "Subject must have between 5 and 100 characters", MinimumLength = 5)]
        public String ShortDescription { get; set; }
        public TypeEnum Type { get; set; }
        public PriorityEnum Priority { get; set; }
        public StateEnum State { get; set; }
        public int NumComments { get; set; }

        public String MemberSolving { get; set; } // can be null
    }
}
