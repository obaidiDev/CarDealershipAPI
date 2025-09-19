using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using cdms.Data;
using cdms.Models;
using Microsoft.EntityFrameworkCore;

namespace cdms.Services
{
    public class UserService
    {
        private readonly CarDealershipDbContext _db;

        public UserService(CarDealershipDbContext db)
        {
            _db = db;

            // Seed admin if not exists
            if (!_db.Users.Any(u => u.Role == "Admin"))
            {
                var admin = new User
                {
                    Email = "admin@dealer.com",
                    PasswordHash = HashPassword("Admin123!"),
                    Role = "Admin",
                    FullName = "Administrator"
                };
                _db.Users.Add(admin);
                _db.SaveChanges();
            }
        }

        public static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        public async Task<User?> GetByEmailAsync(string email) =>
            await _db.Users.FirstOrDefaultAsync(u => u.Email == email);

        public async Task<User?> GetByIdAsync(Guid id) =>
            await _db.Users.FindAsync(id);

        public async Task AddAsync(User user)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> VerifyPasswordAsync(User user, string password)
            => await Task.Run(() => user.PasswordHash == HashPassword(password));

        public IQueryable<User> GetAllCustomers()
            => _db.Users.Where(u => u.Role == "Customer");
    }
}
