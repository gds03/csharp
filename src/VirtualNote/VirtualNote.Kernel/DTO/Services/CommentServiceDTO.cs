using System;
using System.ComponentModel.DataAnnotations;

namespace VirtualNote.Kernel.DTO
{
    public sealed class CommentServiceDto : IServiceDTO
    {
        public int IssueID { get; set; }
        public int CommentID { get; set; }

        [Required(ErrorMessage = "Field is required")]
        [StringLength(2000, ErrorMessage = "Description must be between 5 and 2000 characters", MinimumLength = 5)]
        public String Description { get; set; }
    }
}
