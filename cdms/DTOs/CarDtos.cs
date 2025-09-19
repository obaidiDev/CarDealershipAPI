using System.ComponentModel.DataAnnotations;

namespace cdms.DTOs
{
    public class CarDto
    {
        [Required(ErrorMessage = "Make is required.")]
        public string Make { get; set; } = string.Empty;

        [Required(ErrorMessage = "Model is required.")]
        public string Model { get; set; } = string.Empty;

        [Range(1886, 2100, ErrorMessage = "Year must be valid.")]
        public int Year { get; set; }

        [StringLength(50, ErrorMessage = "Trim level can't exceed 50 characters.")]
        public string TrimLevel { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Market region can't exceed 50 characters.")]
        public string MarketRegion { get; set; } = string.Empty;

        [Required(ErrorMessage = "Color is required.")]
        public string Color { get; set; } = string.Empty;

        [Required(ErrorMessage = "License plate is required.")]
        public string LicensePlate { get; set; } = string.Empty;

        public bool IsAvailable { get; set; } = true;

        [Range(0, double.MaxValue, ErrorMessage = "Price must be non-negative.")]
        public decimal PricePerDay { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Mileage must be non-negative.")]
        public int Mileage { get; set; }

        [Required(ErrorMessage = "Transmission type is required.")]
        public string TransmissionType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Fuel type is required.")]
        public string FuelType { get; set; } = string.Empty;

        [Range(1, 20, ErrorMessage = "Seats must be between 1 and 20.")]
        public int NumberOfSeats { get; set; }

        [StringLength(500, ErrorMessage = "Description can't exceed 500 characters.")]
        public string Description { get; set; } = string.Empty;

        public String[] Features { get; set; } = Array.Empty<string>();

        [Url(ErrorMessage = "ImageUrl must be a valid URL.")]
        public string ImageUrl { get; set; } = string.Empty;
    }
}
