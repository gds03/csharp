using System;
using System.ComponentModel.DataAnnotations;
using VirtualNote.Kernel.Validation.DataAnnotations;

namespace VirtualNote.Kernel.DTO
{
    public sealed class IssueServiceClientDTO : IServiceDTO
    {
        public int IssueId { get; set; }
        public int ProjectId { get; set; }

        [Required(ErrorMessage = "Field is required")]
        [StringLength(100, ErrorMessage = "Subject must have between 5 and 100 characters", MinimumLength = 5)]
        public String ShortDescription { get; set; }

        [Required(ErrorMessage = "Field is required")]
        [StringLength(1000, ErrorMessage = "Description must have between 5 and 1000 characters", MinimumLength = 5)]
        public String LongDescription { get; set; }

        public PriorityEnum Priority { get; set; }
        public TypeEnum Type { get; set; }
    }

    public sealed class IssueServiceMemberDTO : IServiceDTO
    {
        public int IssueId { get; set; }
        public StateEnum State { get; set; }
    }
}
