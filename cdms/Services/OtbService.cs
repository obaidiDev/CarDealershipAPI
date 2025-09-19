using System;
using System.Collections.Generic;
using System.Linq;
using cdms.Models;

namespace cdms.Services
{
    public class OtpService
    {
        private readonly List<OtpEntry> _otps = new();
        private readonly Random _rand = new();

        public OtpEntry Generate(Guid userId, string purpose)
        {
            var code = _rand.Next(100000, 999999).ToString();
            var otp = new OtpEntry
            {
                Code = code,
                Purpose = purpose,
                UserId = userId,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5)
            };
            _otps.Add(otp);
            Console.WriteLine($"OTP for {purpose}: {code}");
            return otp;
        }

        public bool Validate(Guid userId, string code, string purpose)
        {
            var otp = _otps.FirstOrDefault(o => o.UserId == userId && o.Code == code && o.Purpose == purpose);
            if (otp == null || otp.ExpiresAt < DateTime.UtcNow) return false;
            _otps.Remove(otp);
            return true;
        }
    }

    public class OtpEntry
    {
        public string Code { get; set; } = string.Empty;
        public string Purpose { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
