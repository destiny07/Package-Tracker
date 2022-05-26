using Blockchainsure.PackageTracker.Domain.Repositories;
using Blockchainsure.PackageTracker.Infrastructure.Domain;
using Blockchainsure.PackageTracker.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Blockchainsure.PackageTracker.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<PackageTrackerDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("Default")));
            services.AddScoped<IPackageRepository, PackageRepository>();

            return services;
        }
    }
}
