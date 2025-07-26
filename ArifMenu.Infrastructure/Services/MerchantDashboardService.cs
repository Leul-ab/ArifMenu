using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArifMenu.Application.DTOs;
using ArifMenu.Application.Interfaces;
using ArifMenu.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ArifMenu.Infrastructure.Services
{
    public class MerchantDashboardService : IMerchantDashboardService
    {
        private readonly ArifMenuDbContext _context;

        public MerchantDashboardService(ArifMenuDbContext context)
        {
            _context = context;
        }

        public async Task<MerchantDashboardResponse> GetMerchantDashboardAsync(Guid merchantUserId)
        {
            var merchant = await _context.Merchants
                .FirstOrDefaultAsync(x => x.UserId == merchantUserId);

            if (merchant == null) throw new Exception("Merchant not found");

            var now = DateTime.UtcNow;
            var startOfWeek = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Utc).AddDays(-(int)now.DayOfWeek);
            var startOfMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var startOfLastMonth = startOfMonth.AddMonths(-1);
            var endOfLastMonth = startOfMonth.AddDays(-1);

            var menus = await _context.Menus.CountAsync(x => x.MerchantId == merchant.Id && x.IsActive);
            var categories = await _context.MenuCategories.CountAsync(x => x.MerchantId == merchant.Id);
            var totalScans = await _context.MerchantQrLinks
                .Where(x => x.MerchantId == merchant.Id)
                .SumAsync(x => x.ScanCount);

            var today = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Utc);

            var todayScans = await _context.QrScanLogs
                .CountAsync(x => x.MerchantId == merchant.Id &&
                                x.ScanDate >= today &&
                                x.ScanDate < today.AddDays(1));

            var thisWeekScans = await _context.QrScanLogs
                .CountAsync(x => x.MerchantId == merchant.Id && x.ScanDate >= startOfWeek);

            var thisMonthScans = await _context.QrScanLogs
                .CountAsync(x => x.MerchantId == merchant.Id && x.ScanDate >= startOfMonth);

            var lastMonthScans = await _context.QrScanLogs
                .CountAsync(x => x.MerchantId == merchant.Id &&
                               x.ScanDate >= startOfLastMonth &&
                               x.ScanDate <= endOfLastMonth);

            double growth = lastMonthScans == 0 ? 100 :
                Math.Round(((double)(thisMonthScans - lastMonthScans) / lastMonthScans) * 100, 2);

            var weeklyChart = await _context.QrScanLogs
                .Where(x => x.MerchantId == merchant.Id && x.ScanDate >= startOfWeek)
                .GroupBy(x => new DateTime(x.ScanDate.Year, x.ScanDate.Month, x.ScanDate.Day, 0, 0, 0, DateTimeKind.Utc))
                .Select(g => new DailyScanChartData
                {
                    Date = g.Key.ToString("yyyy-MM-dd"),
                    Count = g.Count()
                }).ToListAsync();

            var monthlyChart = await _context.QrScanLogs
                .Where(x => x.MerchantId == merchant.Id && x.ScanDate >= now.AddMonths(-6))
                .GroupBy(x => new { x.ScanDate.Year, x.ScanDate.Month })
                .Select(g => new MonthlyScanChartData
                {
                    Month = $"{g.Key.Year}-{g.Key.Month:00}",
                    Count = g.Count()
                }).ToListAsync();

            return new MerchantDashboardResponse
            {
                TotalMenus = menus,
                TotalCategories = categories,
                TotalScans = totalScans,
                TodayScans = todayScans,
                ThisWeekScans = thisWeekScans,
                ThisMonthScans = thisMonthScans,
                LastMonthScans = lastMonthScans,
                MonthlyGrowthPercentage = growth,
                WeeklyChart = weeklyChart,
                MonthlyChart = monthlyChart
            };
        }
    }
}