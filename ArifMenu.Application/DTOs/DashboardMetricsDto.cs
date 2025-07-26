using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArifMenu.Application.DTOs
{
    public class DashboardMetricsDto
    {
        public int TotalMerchants { get; set; }
        public int ActiveMerchants { get; set; }
        public int InActiveMerchants { get; set; }

        public int MerchantsAddedThisWeek { get; set; }
        public int MerchantsAddedThisMonth { get; set; }
        public int MerchantsAddedThisYear { get; set; }

    }
}
