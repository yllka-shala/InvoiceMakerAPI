using System.ComponentModel.DataAnnotations;

namespace InvoiceMakerAPI.DTOs
{
    public class ChangePasswordDTO
    {
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string CurrentPassword { get; set; } = string.Empty;
        [Required]
        public string NewPassword { get; set; } = string.Empty;
    }
}
