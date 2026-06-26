using System.ComponentModel.DataAnnotations;

namespace InvoiceMakerAPI.DTOs
{
    public class RegisterDTO
    {
        [Required]
        public string FullName { get; set; } = default!;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = default!;
        [Required]
        public string Password { get; set; } = default!;
        [Required]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = default!;
    }
}
