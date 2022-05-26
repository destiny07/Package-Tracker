using Blockchainsure.PackageTracker.Domain.Models;
using Blockchainsure.PackageTracker.Domain.Repositories;
using Blockchainsure.PackageTracker.Infrastructure.Persistence;
using Blockchainsure.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockchainsure.PackageTracker.Infrastructure.Domain
{
    public class PackageRepository : IPackageRepository
    {
        private readonly PackageTrackerDbContext _dbContext;

        public PackageRepository(PackageTrackerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Package> CreateAsync(Package package)
        {
            var now = DateTime.UtcNow;
            package.Created = now;
            package.Modified = now;

            var addedPackage = await _dbContext.AddAsync(package);
            return addedPackage.Entity;
        }

        public async Task DeleteAsync(Guid id)
        {
            var package = await _dbContext.Packages.FindAsync(id);
            _dbContext.Packages.Remove(package);
        }

        public async Task<Package> GetPackageById(Guid id)
        {
            return await _dbContext.Packages.FindAsync(id);
        }

        public async Task<Package> GetPackageByTrackingNumberAsync(string trackingNumber)
        {
            return await _dbContext.Packages.FirstOrDefaultAsync(x => x.TrackingNumber == trackingNumber);
        }

        public async Task<List<Package>> GetPackagesAsync()
        {
            return await _dbContext.Packages.AsNoTracking().ToListAsync();
        }

        public async Task<List<Package>> GetPackagesNotInFinalState(int statusLastUpdatedHours)
        {
            return await _dbContext.Packages
                .AsNoTracking()
                .Include(x => x.PackageActivities)
                .Where(x => x.UpdatesLastFetched.AddHours(statusLastUpdatedHours) < DateTime.UtcNow &&
                    x.PackageActivities.Any() && 
                    x.PackageActivities.OrderByDescending(x1 => x1.Modified).FirstOrDefault().Status != PackageStatusConstants.Delivered &&
                    x.PackageActivities.OrderByDescending(x1 => x1.Modified).FirstOrDefault().Status != PackageStatusConstants.Failure)
                .ToListAsync();
        }

        public async Task<List<Package>> GetPackagesInTransitAsync()
        {
            return await _dbContext.Packages.AsNoTracking().Where(x => 
                x.PackageActivities.Any() &&
                (x.PackageActivities.OrderByDescending(x1 => x1.Modified).FirstOrDefault().Status == "pretransit" ||
                x.PackageActivities.OrderByDescending(x1 => x1.Modified).FirstOrDefault().Status == "transit")).ToListAsync();
        }

        public void UpdateAsync(Package package)
        {
            package.Modified = DateTime.UtcNow;
            _dbContext.Packages.Update(package);
        }

        public async Task<PackageActivity> AddPackageActivity(PackageActivity packageActivity)
        {
            var now = DateTime.UtcNow;
            packageActivity.Created = now;
            packageActivity.Modified = now;

            var entry = await _dbContext.PackageActivities.AddAsync(packageActivity);
            return entry.Entity;
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
