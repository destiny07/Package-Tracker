using AutoMapper;
using Blockchainsure.PackageTracker.Application.Dto;
using Blockchainsure.PackageTracker.Domain.Models;
using Blockchainsure.PackageTracker.Domain.Repositories;
using Blockchainsure.Shared.Common.CQRS;
using Blockchainsure.Shared.Services.Tracking.Shippo;
using Blockchainsure.Shared.Services.Tracking.Shippo.Messages;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Blockchainsure.PackageTracker.Application.Packages.CreatePackage
{
    public class CreatePackageCommand : IRequest<CommandQueryResult<PackageDto>>
    {
        public CreatePackageCommand(CreatePackageDto createPackage)
        {
            CreatePackage = createPackage;
        }

        public CreatePackageDto CreatePackage { get; }

        public sealed class CreatePackageCommandHandler : IRequestHandler<CreatePackageCommand, CommandQueryResult<PackageDto>>
        {
            private readonly IMapper _mapper;
            private readonly IShippoTrackingService _trackingService;
            private readonly IPackageRepository _packageRepository;

            public CreatePackageCommandHandler(IMapper mapper, IShippoTrackingService trackingService, IPackageRepository packageRepository)
            {
                _mapper = mapper;
                _trackingService = trackingService;
                _packageRepository = packageRepository;
            }

            public async Task<CommandQueryResult<PackageDto>> Handle(CreatePackageCommand request, CancellationToken cancellationToken)
            {
                var existingTrackingNo = await _packageRepository.GetPackageByTrackingNumberAsync(request.CreatePackage.TrackingNumber);

                if (existingTrackingNo != null)
                {
                    return CommandQueryResultFactory.GetErrorResult<PackageDto>(
                        CommandQueryResultCode.Existing, $"Tracking Number: {request.CreatePackage.TrackingNumber}");
                }

                var trackingResponse = await _trackingService.GetPackageAsync(new ShippoTrackingServiceRequest
                {
                    Carrier = request.CreatePackage.Carrier,
                    TrackingNumber = request.CreatePackage.TrackingNumber
                });
                var newPackage = await _packageRepository.CreateAsync(new Package
                {
                    TrackingNumber = trackingResponse.TrackingNumber,
                    Carrier = trackingResponse.Carrier,
                    Eta = trackingResponse.Eta,
                    OriginalEta = trackingResponse.OriginalEta,
                    From = new ShippingAddress
                    {
                        City = trackingResponse.AddressFrom.City,
                        State = trackingResponse.AddressFrom.State,
                        Zip = trackingResponse.AddressFrom.Zip,
                        Country = trackingResponse.AddressFrom.Country
                    },
                    To = new ShippingAddress
                    {
                        City = trackingResponse.AddressTo.City,
                        State = trackingResponse.AddressTo.State,
                        Zip = trackingResponse.AddressTo.Zip,
                        Country = trackingResponse.AddressTo.Country
                    },
                    UpdatesLastFetched = DateTime.UtcNow,
                    Raw = trackingResponse.Raw
                });
                await _packageRepository.AddPackageActivity(new PackageActivity
                {
                    Status = trackingResponse.TrackingStatus.Status,
                    Substatus = trackingResponse.TrackingStatus.Substatus,
                    StatusDate = trackingResponse.TrackingStatus.StatusDate,
                    StatusDetails = trackingResponse.TrackingStatus.StatusDetails,
                    Location = trackingResponse.TrackingStatus.Location != null ? new ShippingAddress {
                        City = trackingResponse.TrackingStatus.Location.City,
                        State = trackingResponse.TrackingStatus.Location.State,
                        Zip = trackingResponse.TrackingStatus.Location.Zip,
                        Country = trackingResponse.TrackingStatus.Location.Country
                    } : null,
                    PackageId = newPackage.Id
                });
                await _packageRepository.SaveChangesAsync();
                var packageDto = _mapper.Map<PackageDto>(newPackage);
                return CommandQueryResultFactory.GetResult(packageDto);
            }
        }
    }
}
