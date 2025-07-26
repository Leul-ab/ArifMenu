using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArifMenu.Application.DTOs
{
    public class HistoricalMerchantDataDto
    {
        public string Period { get; set; } = string.Empty;
        public int MerchantCount { get; set; }
    }
}
