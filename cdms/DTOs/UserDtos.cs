namespace cdms.DTOs
{
    public record RegisterDto(string Email, string Password, string? Role, string FullName, string PhoneNumber);
    public record LoginDto(string Email, string Password);
    public record VerifyDto(string Email, string Code);
}
