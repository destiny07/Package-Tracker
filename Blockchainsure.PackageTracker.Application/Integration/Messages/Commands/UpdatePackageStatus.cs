using Blockchainsure.Shared.Integration.Messages.Commands;
using System;

namespace Blockchainsure.PackageTracker.Application.Integration.Messages.Commands
{
    public class UpdatePackageStatus : IUpdatePackageStatus
    {
        public string Carrier { get; set; }
        public string TrackingNumber { get; set; }
        public string Status { get; set; }
        public string SubStatus { get; set; }
        public DateTime StatusDate { get; set; }
        public string StatusDetails { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
    }
}
