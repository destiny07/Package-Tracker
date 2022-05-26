using Blockchainsure.PackageTracker.Application.Packages.FetchUpdatePackages;
using Blockchainsure.PackageTracker.Configuration;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Blockchainsure.PackageTracker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceScopeFactory _factory;
        private readonly AppConfiguration _appConfiguration;

        public Worker(ILogger<Worker> logger, IServiceScopeFactory factory, AppConfiguration appConfiguration)
        {
            _logger = logger;
            _factory = factory;
            _appConfiguration = appConfiguration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("PackageTracker running at: {time}", DateTimeOffset.Now);

                using var scope = _factory.CreateScope();
                var mediator = scope.ServiceProvider.GetService<IMediator>();

                var response = await mediator.Send(new FetchUpdatePackagesCommand(_appConfiguration.PackageUpdateIntervalHours));

                await Task.Delay(TimeSpan.FromMinutes(_appConfiguration.IntervalMinutes), stoppingToken);
            }
        }
    }
}
