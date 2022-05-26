using Blockchainsure.PackageTracker.Application.Integration.Messages.Commands;
using Blockchainsure.PackageTracker.Application.Integration.Producers;
using Blockchainsure.PackageTracker.Domain.Models;
using Blockchainsure.PackageTracker.Domain.Repositories;
using Blockchainsure.Shared.Common.CQRS;
using Blockchainsure.Shared.Services.Tracking.Shippo;
using Blockchainsure.Shared.Services.Tracking.Shippo.Messages;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Blockchainsure.PackageTracker.Application.Packages.FetchUpdatePackages
{
    public class FetchUpdatePackagesCommand : IRequest<CommandQueryResult<int>>
    {
        public FetchUpdatePackagesCommand(int statusLastUpdatedHours)
        {
            StatusLastUpdatedDays = statusLastUpdatedHours;
        }

        public int StatusLastUpdatedDays { get; }

        public sealed class FetchUpdatePackagesCommandHandler : IRequestHandler<FetchUpdatePackagesCommand, CommandQueryResult<int>>
        {
            private readonly ILogger<FetchUpdatePackagesCommandHandler> _logger;
            private readonly IShippoTrackingService _trackingService;
            private readonly IUpdatePackageStatusProducer _packageStatusProducer;
            private readonly IPackageRepository _packageRepository;

            public FetchUpdatePackagesCommandHandler(
                ILogger<FetchUpdatePackagesCommandHandler> logger,
                IShippoTrackingService trackingService,
                IUpdatePackageStatusProducer packageStatusProducer,
                IPackageRepository packageRepository)
            {
                _logger = logger;
                _trackingService = trackingService;
                _packageStatusProducer = packageStatusProducer;
                _packageRepository = packageRepository;
            }

            public async Task<CommandQueryResult<int>> Handle(FetchUpdatePackagesCommand request, CancellationToken cancellationToken)
            {
                var packagesToUpdate = await _packageRepository.GetPackagesNotInFinalState(request.StatusLastUpdatedDays);

                if (!packagesToUpdate.Any())
                {
                    _logger.LogInformation($"No packages to update.");
                    return CommandQueryResultFactory.GetResult(0);
                }

                int updatedCount = 0;
                foreach (var package in packagesToUpdate)
                {
                    var trackingServiceResponse = await _trackingService.GetPackageAsync(new ShippoTrackingServiceRequest
                    {
                        Carrier = package.Carrier,
                        TrackingNumber = package.TrackingNumber
                    });

                    _logger.LogInformation($"{package.TrackingNumber}({package.Carrier}): Fetched tracking data.");

                    var latestActivity = package.PackageActivities.OrderByDescending(x => x.Modified).FirstOrDefault();

                    if (latestActivity.Status == trackingServiceResponse.TrackingStatus.Status &&
                        latestActivity.Substatus == trackingServiceResponse.TrackingStatus.Substatus)
                    {
                        _logger.LogInformation($"{package.TrackingNumber}({package.Carrier}): " +
                            $"Status({latestActivity.Status}) and Substatus({latestActivity.Substatus}) not updated. Skipping.");
                        continue;
                    }

                    var addedPackageActivity = await CreatePackageActivity(package, trackingServiceResponse);

                    await SendUpdatedStatusToMessageQueue(package, trackingServiceResponse, addedPackageActivity);

                    updatedCount++;
                }

                return CommandQueryResultFactory.GetResult(updatedCount);
            }

            private async Task SendUpdatedStatusToMessageQueue(Package package, ShippoTrackingServiceResponse trackingServiceResponse, PackageActivity addedPackageActivity)
            {
                await _packageStatusProducer.CreatePackageAsync(new UpdatePackageStatus
                {
                    Carrier = package.Carrier,
                    TrackingNumber = package.TrackingNumber,
                    Status = addedPackageActivity.Status,
                    SubStatus = addedPackageActivity.Substatus,
                    StatusDate = addedPackageActivity.StatusDate,
                    StatusDetails = addedPackageActivity.StatusDetails,
                    City = addedPackageActivity.Location?.City,
                    State = addedPackageActivity.Location?.State,
                    Zip = addedPackageActivity.Location?.Zip,
                    Country = addedPackageActivity.Location?.Country
                });
                _logger.LogInformation($"{package.TrackingNumber}({package.Carrier}): " +
                    $"Send new activity to message queue with Status:{trackingServiceResponse.TrackingStatus.Status} and Substatus:{trackingServiceResponse.TrackingStatus.Substatus}");
            }

            private async Task<PackageActivity> CreatePackageActivity(Package package, ShippoTrackingServiceResponse trackingServiceResponse)
            {
                var addedPackageActivity = await _packageRepository.AddPackageActivity(new PackageActivity
                {
                    Status = trackingServiceResponse.TrackingStatus.Status,
                    Substatus = trackingServiceResponse.TrackingStatus.Substatus,
                    StatusDate = trackingServiceResponse.TrackingStatus.StatusDate,
                    StatusDetails = trackingServiceResponse.TrackingStatus.StatusDetails,
                    Location = trackingServiceResponse.TrackingStatus.Location != null ? new ShippingAddress
                    {
                        City = trackingServiceResponse.TrackingStatus.Location.City,
                        State = trackingServiceResponse.TrackingStatus.Location.State,
                        Zip = trackingServiceResponse.TrackingStatus.Location.Zip,
                        Country = trackingServiceResponse.TrackingStatus.Location.Country
                    } : null,
                    PackageId = package.Id
                });
                await _packageRepository.SaveChangesAsync();
                _logger.LogInformation($"{package.TrackingNumber}({package.Carrier}): " +
                        $"Added activity with Status:{trackingServiceResponse.TrackingStatus.Status} and Substatus:{trackingServiceResponse.TrackingStatus.Substatus}");
                return addedPackageActivity;
            }
        }
    }
}
