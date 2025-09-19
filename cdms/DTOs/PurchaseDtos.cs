// DTOs/PurchaseDtos.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace cdms.DTOs
{
    // Sent to generate (initiate) a purchase OTP
    public class PurchaseInitiateDto
    {
        [Required]
        public Guid CarId { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be > 0")]
        public decimal Amount { get; set; }

        public Guid PaymentMethod { get; set; } // optional (can validate existence later)
    }

    // Sent to confirm/complete the purchase (includes OTP code)
    public class ConfirmPurchaseDto
    {
        [Required]
        public Guid CarId { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be > 0")]
        public decimal Amount { get; set; }

        public Guid PaymentMethod { get; set; }

        [Required]
        [StringLength(6, MinimumLength = 4, ErrorMessage = "OTP Code should be 4–6 characters.")]
        public string Code { get; set; } = string.Empty;
    }
}
