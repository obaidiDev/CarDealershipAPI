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
    public class PurchasesController : ControllerBase
    {
        private readonly PurchaseService _purchases;
        private readonly CarService _cars;
        private readonly OtpService _otps;

        public PurchasesController(PurchaseService purchases, CarService cars, OtpService otps)
        {
            _purchases = purchases;
            _cars = cars;
            _otps = otps;
        }

        [Authorize(Roles = "Customer")]
        [HttpPost("request")]
        public async Task<IActionResult> RequestPurchase([FromBody] PurchaseRequestDto dto)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
                return Unauthorized(new { error = "Invalid or missing user identifier" });
            var otp = Request.Headers["X-OTP-Code"].ToString();
            if (string.IsNullOrEmpty(otp) || !_otps.Validate(userId, otp, "purchase"))
                return BadRequest(new { error = "Missing or invalid OTP" });

            var car = await _cars.GetByIdAsync(dto.CarId);
            if (car == null) return NotFound(new { error = "Car not found" });
            if (!car.IsAvailable) return BadRequest(new { error = "Car not available" });

            var purchase = new Purchase
            {
                CarId = car.Id,
                UserId = userId,
                Amount = dto.Amount,
                PaymentMethod = dto.PaymentMethod,
                Status = "Completed",
                DoneAt = DateTime.UtcNow
            };

            await _purchases.AddAsync(purchase);
            car.IsAvailable = false;
            await _cars.UpdateAsync(car);

            return Ok(purchase);
        }

        [Authorize(Roles = "Customer")]
        [HttpGet("history")]
        public IActionResult History()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
                return Unauthorized(new { error = "Invalid or missing user identifier" });
            var purchases = _purchases.GetByUser(userId);
            return Ok(purchases);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        public IActionResult All() => Ok(_purchases.GetAll());
    }
}
