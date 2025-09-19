// Controllers/PurchasesController.cs
using cdms.DTOs;
using cdms.Models;
using cdms.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Linq;

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

        // STEP 1: Generate OTP for a purchase (customer must call this first)
        [Authorize(Roles = "Customer")]
        [HttpPost("request/generate")]
        public async Task<IActionResult> GeneratePurchaseOtp([FromBody] PurchaseInitiateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
                return Unauthorized(new { error = "Invalid or missing user identifier" });

            var car = await _cars.GetByIdAsync(dto.CarId);
            if (car == null) return NotFound(new { error = "Car not found" });
            if (!car.IsAvailable) return BadRequest(new { error = "Car not available" });

            // Generate OTP tied to this user and purpose "purchase"
            var otpEntry = _otps.Generate(userId, "purchase");
            // NOTE: For demo we print OTP to console inside OtpService.Generate.
            // Do NOT return the code in production. Returning expiresAt only.
            return Ok(new
            {
                message = "OTP generated for purchase. Check console for code.",
                expiresAt = otpEntry.ExpiresAt
            });
        }

        // STEP 2: Confirm with OTP and complete the purchase
        [Authorize(Roles = "Customer")]
        [HttpPost("request/confirm")]
        public async Task<IActionResult> ConfirmPurchase([FromBody] ConfirmPurchaseDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
                return Unauthorized(new { error = "Invalid or missing user identifier" });

            // Validate OTP
            if (!_otps.Validate(userId, dto.Code, "purchase"))
                return BadRequest(new { error = "Invalid or expired OTP" });

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

        // Customer: view their purchase history
        [Authorize(Roles = "Customer")]
        [HttpGet("history")]
        public IActionResult History()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
                return Unauthorized(new { error = "Invalid or missing user identifier" });

            var purchases = _purchases.GetByUser(userId).ToList();
            return Ok(purchases);
        }

        // Admin: view all purchases
        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        public IActionResult All()
        {
            var all = _purchases.GetAll().ToList();
            return Ok(all);
        }
    }
}
