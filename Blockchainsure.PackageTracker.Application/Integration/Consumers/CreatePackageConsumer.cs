using Blockchainsure.PackageTracker.Application.Packages.CreatePackage;
using Blockchainsure.Shared.Configurations.Integration;
using Blockchainsure.Shared.Integration.Messages.Commands;
using MassTransit;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Blockchainsure.PackageTracker.Application.Integration.Consumers
{
    public class CreatePackageConsumer : IConsumer<ICreatePackage>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public CreatePackageConsumer(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task Consume(ConsumeContext<ICreatePackage> context)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<CreatePackageConsumer>>();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            logger.LogInformation($"Received message: {context.Message.TrackingNumber}({context.Message.Carrier})");

            await mediator.Send(new CreatePackageCommand(new CreatePackageDto
            {
                Carrier = context.Message.Carrier,
                TrackingNumber = context.Message.TrackingNumber
            }));
        }
    }
}
