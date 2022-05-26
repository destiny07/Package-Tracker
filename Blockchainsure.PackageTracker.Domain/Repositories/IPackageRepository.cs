using Blockchainsure.PackageTracker.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blockchainsure.PackageTracker.Domain.Repositories
{
    public interface IPackageRepository
    {
        Task<List<Package>> GetPackagesAsync();

        /// <summary>
        /// Returns Packages with status equal to PreTransit or Transit
        /// </summary>
        /// <returns></returns>
        Task<List<Package>> GetPackagesInTransitAsync();

        /// <summary>
        /// Returns Packages with status not equal to Delivered or Failure
        /// </summary>
        /// <returns></returns>
        Task<List<Package>> GetPackagesNotInFinalState(int statusLastUpdatedDays);

        Task<Package> GetPackageByTrackingNumberAsync(string trackingNumber);

        Task<Package> GetPackageById(Guid id);

        Task<Package> CreateAsync(Package package);

        void UpdateAsync(Package package);

        Task DeleteAsync(Guid id);

        Task<PackageActivity> AddPackageActivity(PackageActivity packageActivity);
        Task SaveChangesAsync();
    }
}
