using ArifMenu.Application.DTOs;
using ArifMenu.Application.Enums;
using ArifMenu.Application.Interfaces;
using ArifMenu.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArifMenu.Infrastructure.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ArifMenuDbContext _context;

        public DashboardService(ArifMenuDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardMetricsDto> GetDashboardMetricsAsync()
        {
            try
            {
                var now = DateTime.UtcNow;

                // Start of the week: Assuming Sunday is the first day (can adjust if needed)
                var startOfWeek = now.Date.AddDays(-(int)now.DayOfWeek);

                // Normalize to UTC midnights
                var startOfMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                var startOfYear = new DateTime(now.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc);

                var totalMerchants = await _context.Merchants.CountAsync();
                var activeMerchants = await _context.Merchants.CountAsync(m => m.IsActive);
                var inactiveMerchants = totalMerchants - activeMerchants;

                var merchantsThisWeek = await _context.Merchants
                    .CountAsync(m => m.CreatedAt >= startOfWeek);
                var merchantsThisMonth = await _context.Merchants
                    .CountAsync(m => m.CreatedAt >= startOfMonth);
                var merchantsThisYear = await _context.Merchants
                    .CountAsync(m => m.CreatedAt >= startOfYear);

                return new DashboardMetricsDto
                {
                    TotalMerchants = totalMerchants,
                    ActiveMerchants = activeMerchants,
                    InActiveMerchants = inactiveMerchants,
                    MerchantsAddedThisWeek = merchantsThisWeek,
                    MerchantsAddedThisMonth = merchantsThisMonth,
                    MerchantsAddedThisYear = merchantsThisYear
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine("DASHBOARD ERROR: " + ex);
                throw;
            }
        }

        public async Task<List<HistoricalMerchantDataDto>> GetHistoricalMerchantDataAsync(GranularityType granularity)
        {
            var now = DateTime.UtcNow;
            var startDate = now.AddMonths(-6); // last 6 months, change as needed

            var query = _context.Merchants
                .Where(m => m.CreatedAt >= startDate)
                .AsEnumerable(); // switch to in-memory so we can use C# DateTime functions

            var grouped = granularity switch
            {
                GranularityType.Weekly => query
                    .GroupBy(m => System.Globalization.CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(
                        m.CreatedAt,
                        System.Globalization.CalendarWeekRule.FirstDay,
                        DayOfWeek.Sunday))
                    .Select(g => new HistoricalMerchantDataDto
                    {
                        Period = $"Week {g.Key}",
                        MerchantCount = g.Count()
                    }),

                GranularityType.Monthly => query
                    .GroupBy(m => $"{m.CreatedAt.Year}-{m.CreatedAt.Month:D2}")
                    .Select(g => new HistoricalMerchantDataDto
                    {
                        Period = g.Key,
                        MerchantCount = g.Count()
                    }),

                _ => throw new ArgumentOutOfRangeException(nameof(granularity))
            };


            return grouped.OrderBy(g => g.Period).ToList();
        }


    }
}
