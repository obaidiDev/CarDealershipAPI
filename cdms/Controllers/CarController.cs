using cdms.DTOs;
using cdms.Models;
using cdms.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace cdms.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarsController : ControllerBase
    {
        private readonly CarService _cars;
        private readonly OtpService _otps;

        public CarsController(CarService cars, OtpService otps)
        {
            _cars = cars;
            _otps = otps;
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] string? make, [FromQuery] string? model)
        {
            var query = _cars.GetAll();
            if (!string.IsNullOrEmpty(make)) query = query.Where(c => c.Make.Contains(make, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(model)) query = query.Where(c => c.Model.Contains(model, StringComparison.OrdinalIgnoreCase));
            return Ok(query);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var car = await _cars.GetByIdAsync(id);
            if (car == null) return NotFound(new { error = "Car not found" });
            return Ok(car);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CarDto dto)
        {
            var otp = Request.Headers["X-OTP-Code"].ToString();
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr))
                return BadRequest(new { error = "Missing user identifier" });
            var userId = Guid.Parse(userIdStr);
            if (string.IsNullOrEmpty(otp) || !_otps.Validate(userId, otp, "update"))
                return BadRequest(new { error = "Missing or invalid OTP" });

            var car = new Car
            {
                Make = dto.Make,
                Model = dto.Model,
                Year = dto.Year,
                TrimLevel = dto.TrimLevel,
                MarketRegion = dto.MarketRegion,
                Color = dto.Color,
                LicensePlate = dto.LicensePlate,
                IsAvailable = dto.IsAvailable,
                PricePerDay = dto.PricePerDay,
                Mileage = dto.Mileage,
                TransmissionType = dto.TransmissionType,
                FuelType = dto.FuelType,
                NumberOfSeats = dto.NumberOfSeats,
                Description = dto.Description,
                Features = dto.Features,
                ImageUrl = dto.ImageUrl
            };

            await _cars.AddOrUpdateAsync(car);
            return CreatedAtAction(nameof(Get), new { id = car.Id }, car);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CarDto dto)
        {
            var otp = Request.Headers["X-OTP-Code"].ToString();
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr))
                return BadRequest(new { error = "Missing user identifier" });
            var userId = Guid.Parse(userIdStr);
            if (string.IsNullOrEmpty(otp) || !_otps.Validate(userId, otp, "update"))
                return BadRequest(new { error = "Missing or invalid OTP" });

            var existing = await _cars.GetByIdAsync(id);
            if (existing == null) return NotFound(new { error = "Car not found" });

            existing.Make = dto.Make;
            existing.Model = dto.Model;
            existing.Year = dto.Year;
            existing.TrimLevel = dto.TrimLevel;
            existing.MarketRegion = dto.MarketRegion;
            existing.Color = dto.Color;
            existing.LicensePlate = dto.LicensePlate;
            existing.IsAvailable = dto.IsAvailable;
            existing.PricePerDay = dto.PricePerDay;
            existing.Mileage = dto.Mileage;
            existing.TransmissionType = dto.TransmissionType;
            existing.FuelType = dto.FuelType;
            existing.NumberOfSeats = dto.NumberOfSeats;
            existing.Description = dto.Description;
            existing.Features = dto.Features;
            existing.ImageUrl = dto.ImageUrl;

            await _cars.UpdateAsync(existing);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _cars.DeleteAsync(id);
            return NoContent();
        }
    }
}
