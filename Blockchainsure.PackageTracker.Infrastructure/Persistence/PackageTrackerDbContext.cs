using Blockchainsure.PackageTracker.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Blockchainsure.PackageTracker.Infrastructure.Persistence
{
    public class PackageTrackerDbContext : DbContext
    {
        public PackageTrackerDbContext(DbContextOptions<PackageTrackerDbContext> options)
            : base(options)
        {
        }

        public DbSet<Package> Packages { get; set; }
        public DbSet<PackageActivity> PackageActivities { get; set; }
        public DbSet<ShippingAddress> ShippingAddresses { get; set; }
    }
}
