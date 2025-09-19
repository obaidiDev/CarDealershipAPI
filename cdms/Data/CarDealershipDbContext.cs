using cdms.Models;
using Microsoft.EntityFrameworkCore;

namespace cdms.Data
{
    public class CarDealershipDbContext : DbContext
    {
        public CarDealershipDbContext(DbContextOptions<CarDealershipDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
    }
}
