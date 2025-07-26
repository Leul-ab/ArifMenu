using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArifMenu.Domain.Entities
{

    public class MerchantQrLink
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid MerchantId { get; set; }
        public string QrSlug { get; set; } = string.Empty; // unique string
        public int ScanCount { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Merchant Merchant { get; set; }
    }


}
