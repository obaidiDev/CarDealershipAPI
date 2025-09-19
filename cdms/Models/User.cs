namespace cdms.Models
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        // Other user properties
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
        public string Role { get; set; } = "Customer";
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public bool EmailVerified { get; set; } = false;
        public bool PhoneVerified { get; set; } = false;
        public string Address { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
        public string ProfilePictureUrl { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public string[] Interests { get; set; } = Array.Empty<string>();
        public string[] SocialLinks { get; set; } = Array.Empty<string>();
        public DateTime LastLoginAt { get; set; } = DateTime.UtcNow;
        public int LoginCount { get; set; } = 0;
        public string TimeZone { get; set; } = "UTC";
        public Guid[] PaymentMethods { get; set; } = Array.Empty<Guid>();
    }
}