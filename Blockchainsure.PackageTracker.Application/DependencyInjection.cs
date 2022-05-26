using Blockchainsure.PackageTracker.Application.Integration.Consumers;
using Blockchainsure.PackageTracker.Application.Integration.Producers;
using Blockchainsure.PackageTracker.Application.Mapping;
using Blockchainsure.Shared.Services.Tracking.Shippo;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Blockchainsure.PackageTracker.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient<IShippoTrackingService, ShippoTrackingService>();

            services.AddMappings();
            services.AddMediatR(typeof(DependencyInjection));

            AddMassTransit(services, configuration);
            return services;
        }


        private static void AddMassTransit(IServiceCollection services, IConfiguration configuration)
        {
            var sp = services.BuildServiceProvider();

            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(configuration.GetSection("MessageQueueConfiguration")["Host"], configurator =>
                    {
                        configurator.Username(configuration.GetSection("MessageQueueConfiguration")["Username"]);
                        configurator.Password(configuration.GetSection("MessageQueueConfiguration")["Password"]);
                    });
                    cfg.ReceiveEndpoint(configuration.GetSection("MessageQueueConfiguration")["CreatePackageQueueName"], e =>
                    {
                        var serviceScopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
                        e.Consumer(() => new CreatePackageConsumer(serviceScopeFactory), config => config.UseConcurrentMessageLimit(1));
                    });
                });
            });
            services.AddMassTransitHostedService();
            services.AddScoped<IUpdatePackageStatusProducer, UpdatePackageStatusProducer>();
        }
    }
}
