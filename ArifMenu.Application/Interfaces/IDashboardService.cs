using ArifMenu.Application.DTOs;
using ArifMenu.Application.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ArifMenu.Application.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardMetricsDto> GetDashboardMetricsAsync();

        Task<List<HistoricalMerchantDataDto>> GetHistoricalMerchantDataAsync(GranularityType granularity);
    }
}
