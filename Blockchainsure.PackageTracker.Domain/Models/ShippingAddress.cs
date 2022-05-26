using Blockchainsure.Shared.Common.Model;
using System;

namespace Blockchainsure.PackageTracker.Domain.Models
{
    public class ShippingAddress : BaseEntity
    {
        public Guid Id { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
    }
}
