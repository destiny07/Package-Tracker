using Blockchainsure.Shared.Common.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blockchainsure.PackageTracker.Domain.Models
{
    public class Package : BaseEntity
    {
        public Guid Id { get; set; }

        [Required]
        public string TrackingNumber { get; set; }

        [Required]
        public string Carrier { get; set; }

        public ShippingAddress From { get; set; }

        public ShippingAddress To { get; set; }

        public DateTime? Eta { get; set; }

        public DateTime? OriginalEta { get; set; }

        public DateTime UpdatesLastFetched { get; set; }

        public ICollection<PackageActivity> PackageActivities { get; set; }

        [Column(TypeName = "jsonb")]
        public string Raw { get; set; }
    }
}
