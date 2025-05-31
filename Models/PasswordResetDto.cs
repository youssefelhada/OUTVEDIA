using System.ComponentModel.DataAnnotations;

namespace final_project_depi.Models
{
    public class PasswordResetDto
    {
        [Required, EmailAddress, MaxLength(100)]
        public string Email { get; set; } = "";
        [Required, MaxLength(100)]

        public string Password { get; set; } = "";

        [Required(ErrorMessage = "The Confirm Password field is required")]
        [Compare("Password", ErrorMessage = "Confirm Password and Password do not match")]
        public string ConfirmPassword { get; set; } = "";
    }
}
