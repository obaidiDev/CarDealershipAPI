namespace cdms.DTOs
{
    public record CarDto(
        string Make,
        string Model,
        int Year,
        string TrimLevel,
        string MarketRegion,
        string Color,
        string LicensePlate,
        bool IsAvailable,
        decimal PricePerDay,
        int Mileage,
        string TransmissionType,
        string FuelType,
        int NumberOfSeats,
        string Description,
        string[] Features,
        string ImageUrl
    );
}
