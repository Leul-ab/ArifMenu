using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArifMenu.Application.DTOs;

namespace ArifMenu.Application.Interfaces
{
    public interface IMerchantDashboardService
    {
        Task<MerchantDashboardResponse> GetMerchantDashboardAsync(Guid merchantUserId);
    }
}
