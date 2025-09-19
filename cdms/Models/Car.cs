namespace cdms.Models
{
    public class Car
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
        public string TrimLevel { get; set; } = string.Empty;
        public string MarketRegion { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string LicensePlate { get; set; } = string.Empty;
        public bool IsAvailable { get; set; } = true;
        public decimal PricePerDay { get; set; }
        public int Mileage { get; set; }
        public string TransmissionType { get; set; } = "Automatic"; // e.g., Automatic, Manual
        public string FuelType { get; set; } = "Gasoline"; // e.g., Gasoline, Diesel, Electric
        public int NumberOfSeats { get; set; } = 5;
        public string Description { get; set; } = string.Empty;
        public string[] Features { get; set; } = Array.Empty<string>(); // e.g., GPS, Bluetooth
        public string ImageUrl { get; set; } = string.Empty;
    }
}