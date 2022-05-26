using Blockchainsure.Shared.Common.Model;
using System;

namespace Blockchainsure.PackageTracker.Domain.Models
{
    public class PackageActivity : BaseEntity
    {
        public Guid Id { get; set; }
        public string Status { get; set; }
        public string Substatus { get; set; }
        public DateTime StatusDate { get; set; }
        public string StatusDetails { get; set; }
        public ShippingAddress Location { get; set; }

        public Guid PackageId { get; set; }
        public Package Package { get; set; }
    }
}
