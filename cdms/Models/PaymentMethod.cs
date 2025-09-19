namespace cdms.Models
{
    public class PaymentMethod
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string StripeCustomerId { get; set; } = string.Empty;
        public string StripeToken { get; set; } = string.Empty;
    }
}