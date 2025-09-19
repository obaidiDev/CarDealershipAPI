namespace cdms.DTOs
{
    public record PurchaseRequestDto(
        Guid CarId,
        decimal Amount,
        Guid PaymentMethod,
        string? Otp
    );
}
