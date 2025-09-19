using System.ComponentModel.DataAnnotations;
namespace cdms.DTOs
{
    public class RegisterDto
    {
        [Required]
        [EmailAddress] // ✅ ensures valid email format
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Full name must be between 2 and 100 characters.")]
        public string FullName { get; set; } = string.Empty;

        [Phone] // ✅ ensures valid phone number format
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^(Admin|Customer)$", ErrorMessage = "Role must be either 'Admin' or 'Customer'.")]
        public string Role { get; set; } = "User";
    }
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public record VerifyDto(string Email, string Code);
}
