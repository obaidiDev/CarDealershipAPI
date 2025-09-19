using System;
using System.Linq;
using System.Threading.Tasks;
using cdms.Data;
using cdms.Models;
using Microsoft.EntityFrameworkCore;

namespace cdms.Services
{
    public class PurchaseService
    {
        private readonly CarDealershipDbContext _db;

        public PurchaseService(CarDealershipDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(Purchase purchase)
        {
            _db.Purchases.Add(purchase);
            await _db.SaveChangesAsync();
        }

        public IQueryable<Purchase> GetAll() => _db.Purchases.AsQueryable();

        public IQueryable<Purchase> GetByUser(Guid userId) =>
            _db.Purchases.Where(p => p.UserId == userId);
    }
}
