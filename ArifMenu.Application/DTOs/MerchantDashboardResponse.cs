using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArifMenu.Application.DTOs
{
    public class MerchantDashboardResponse
    {
        public int TotalMenus { get; set; }
        public int TotalCategories { get; set; }
        public int TotalScans { get; set; }

        public int TodayScans { get; set; }
        public int ThisWeekScans { get; set; }
        public int ThisMonthScans { get; set; }

        public int LastMonthScans { get; set; }
        public double MonthlyGrowthPercentage { get; set; }

        public List<DailyScanChartData> WeeklyChart { get; set; } = new();
        public List<MonthlyScanChartData> MonthlyChart { get; set; } = new();
    }

    public class DailyScanChartData
    {
        public string Date { get; set; } = null!;
        public int Count { get; set; }
    }

    public class MonthlyScanChartData
    {
        public string Month { get; set; } = null!;
        public int Count { get; set; }
    }
}
