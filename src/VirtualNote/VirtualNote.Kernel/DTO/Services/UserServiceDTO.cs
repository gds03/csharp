using System;
using System.ComponentModel.DataAnnotations;
using VirtualNote.Kernel.Validation.DataAnnotations;

namespace VirtualNote.Kernel.DTO
{
    public abstract class UserServiceDTO : IServiceDTO
    {
        public int UserID { get; set; }

        [Required(ErrorMessage = "Field is required")]
        [StringLength(20, ErrorMessage = "Name must have between 3 and 20 characters", MinimumLength = 3)]
        [RegularExpression(@"^[\S]*$", ErrorMessage = "Name cannot contain spaces")]
        public String Name { get; set; }

        [Required(ErrorMessage = "Field is required")]
        [StringLength(30, ErrorMessage = "Password must have between 5 and 30 characters", MinimumLength = 5)]
        [RegularExpression(@"^[\S]*$", ErrorMessage = "Password cannot contain spaces")]
        public String Password { get; set; }

        public bool Enabled { get; set; }

        [StringLength(100, ErrorMessage = "Email must have between 8 and 100 characters", MinimumLength = 8)]
        [EmailValidation(ErrorMessage = "Email is not valid")]
        public String Email { get; set; }

        [StringLength(30, ErrorMessage = "Phone must have between 5 and 30 characters", MinimumLength = 5)]        
        public String Phone { get; set; }
    }
}
