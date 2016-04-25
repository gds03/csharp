using System.ComponentModel.DataAnnotations;

namespace VirtualNote.MVC.Models
{
    public sealed class LogOnModel
    {
        [Required(ErrorMessage = "Field is required")]
        [StringLength(20, ErrorMessage = "Username must have between 3 and 20 characters", MinimumLength = 3)]
        [RegularExpression(@"^[\S]*$", ErrorMessage = "Username cannot contain spaces")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Field is required")]
        [StringLength(32, ErrorMessage = "Password must have between 3 and 32 characters", MinimumLength = 3)]
        [RegularExpression(@"^[\S]*$", ErrorMessage = "Password cannot contain spaces")]
        public string Password { get; set; }
    }
}
