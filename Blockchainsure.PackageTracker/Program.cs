using Blockchainsure.PackageTracker.Application;
using Blockchainsure.PackageTracker.Configuration;
using Blockchainsure.PackageTracker.Infrastructure;
using Blockchainsure.Shared.Configurations.Integration;
using Blockchainsure.Shared.Configurations.Services.Tracking;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;

namespace Blockchainsure.PackageTracker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    ConfigureServices(services, hostContext.Configuration);
                    services.AddHostedService<Worker>();
                });

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            var appConfiguration = configuration.GetSection(nameof(AppConfiguration)).Get<AppConfiguration>();
            services.AddSingleton(appConfiguration);
            AddTrackingServicesConfiguration(services, configuration);

            var messageQueueConfiguration = configuration.GetSection(nameof(MessageQueueConfiguration)).Get<MessageQueueConfiguration>();
            services.AddSingleton(messageQueueConfiguration);

            services.AddInfrastructure(configuration);
            services.AddApplication(configuration);
        }

        private static void AddTrackingServicesConfiguration(IServiceCollection services, IConfiguration configuration)
        {
            var trackingServiceSConfiguration = new TrackingServicesConfiguration();
            trackingServiceSConfiguration.Configurations = new Dictionary<string, TrackingServiceConfiguration>();
            trackingServiceSConfiguration.Configurations.Add(TrackingServicesConfiguration.SHIPPO,
                configuration.GetSection(nameof(TrackingServicesConfiguration))
                    .GetSection(nameof(TrackingServicesConfiguration.SHIPPO))
                    .Get<TrackingServiceConfiguration>());
            services.AddSingleton(trackingServiceSConfiguration);
        }
    }
}
