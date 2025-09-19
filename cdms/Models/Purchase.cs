namespace cdms.Models
{
    public class Purchase
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public Guid CarId { get; set; }
        public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public Guid PaymentMethod { get; set; }
        public string Status { get; set; } = "Pending"; // e.g., Pending, Completed, Failed
        public DateTime DoneAt { get; set; } = DateTime.UtcNow;
    }
}