using Blockchainsure.Shared.Configurations.Integration;
using Blockchainsure.Shared.Integration.Messages.Commands;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Blockchainsure.PackageTracker.Application.Integration.Producers
{
    public class UpdatePackageStatusProducer : IUpdatePackageStatusProducer
    {
        private readonly ILogger<UpdatePackageStatusProducer> _logger;
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly MessageQueueConfiguration _messageQueueConfiguration;

        public UpdatePackageStatusProducer(
            ILogger<UpdatePackageStatusProducer> logger,
            ISendEndpointProvider sendEndpointProvider,
            MessageQueueConfiguration messageQueueConfiguration)
        {
            _logger = logger;
            _sendEndpointProvider = sendEndpointProvider;
            _messageQueueConfiguration = messageQueueConfiguration;
        }

        public async Task CreatePackageAsync(IUpdatePackageStatus updatePackageStatus)
        {
            var endpoint = await _sendEndpointProvider.GetSendEndpoint(
                new Uri($"queue:{_messageQueueConfiguration.PackageStatusQueueName}"));
            await endpoint.Send(updatePackageStatus);
        }
    }
}
