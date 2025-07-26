using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArifMenu.Domain.Entities
{
    public class QrScanLog
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid MerchantId { get; set; }
        public DateTime ScanDate { get; set; } = DateTime.UtcNow;

        // Relationships
        public Merchant Merchant { get; set; }
    }
}
