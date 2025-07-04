using StoockerMT.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Domain.Entities.MasterDb
{
    public class TenantModuleUsage : BaseEntity
    {
        public int SubscriptionId { get; private set; }

        [MaxLength(100)]
        public string Feature { get; private set; }

        public int UsageCount { get; private set; }

        public DateTime UsageDate { get; private set; }

        [MaxLength(500)]
        public string? MetaData { get; private set; }

        // Navigation Properties
        [ForeignKey("SubscriptionId")]
        public virtual TenantModuleSubscription Subscription { get; set; }

        private TenantModuleUsage() { }

        public TenantModuleUsage(int subscriptionId, string feature, int usageCount, string? metaData = null)
        {
            SubscriptionId = subscriptionId;
            Feature = feature ?? throw new ArgumentNullException(nameof(feature));
            UsageCount = usageCount;
            UsageDate = DateTime.UtcNow;
            MetaData = metaData;
        }
    }
}
