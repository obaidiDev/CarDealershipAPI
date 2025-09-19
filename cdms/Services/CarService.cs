using System;
using System.Linq;
using System.Threading.Tasks;
using cdms.Data;
using cdms.Models;
using Microsoft.EntityFrameworkCore;

namespace cdms.Services
{
    public class CarService
    {
        private readonly CarDealershipDbContext _db;

        public CarService(CarDealershipDbContext db)
        {
            _db = db;

            // Seed 10 sample vehicles if empty
            if (!_db.Cars.Any())
            {
                for (int i = 1; i <= 10; i++)
                {
                    var car = new Car
                    {
                        Make = "Make" + i,
                        Model = "Model" + i,
                        Year = 2015 + (i % 8),
                        TrimLevel = "Standard",
                        MarketRegion = "US",
                        Color = i % 2 == 0 ? "Red" : "Blue",
                        LicensePlate = $"ABC{i}23",
                        PricePerDay = 50 + i * 10,
                        Mileage = 10000 + i * 1000
                    };
                    _db.Cars.Add(car);
                }
                _db.SaveChanges();
            }
        }

        public IQueryable<Car> GetAll() => _db.Cars.AsQueryable();

        public async Task<Car?> GetByIdAsync(Guid id) => await _db.Cars.FindAsync(id);

        public async Task AddOrUpdateAsync(Car newCar)
        {
            var existing = await _db.Cars.FirstOrDefaultAsync(c =>
                c.Make == newCar.Make &&
                c.Model == newCar.Model &&
                c.Year == newCar.Year &&
                c.TrimLevel == newCar.TrimLevel &&
                c.MarketRegion == newCar.MarketRegion
            );

            if (existing != null)
            {
                // Reuse existing attributes except Color, LicensePlate, IsAvailable
                newCar.PricePerDay = existing.PricePerDay;
                newCar.Mileage = existing.Mileage;
                newCar.TransmissionType = existing.TransmissionType;
                newCar.FuelType = existing.FuelType;
                newCar.NumberOfSeats = existing.NumberOfSeats;
                newCar.Description = existing.Description;
                newCar.Features = existing.Features;
                newCar.ImageUrl = existing.ImageUrl;
            }

            _db.Cars.Add(newCar);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Car car)
        {
            _db.Cars.Update(car);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var car = await _db.Cars.FindAsync(id);
            if (car != null)
            {
                _db.Cars.Remove(car);
                await _db.SaveChangesAsync();
            }
        }
    }
}
