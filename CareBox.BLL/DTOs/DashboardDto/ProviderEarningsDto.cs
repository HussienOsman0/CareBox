using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.DTOs.DashboardDto
{
    public class ProviderEarningsDto
    {
        public decimal DailyEarnings { get; set; }   // أرباح اليوم
        public decimal WeeklyEarnings { get; set; }  // أرباح آخر 7 أيام
        public decimal MonthlyEarnings { get; set; } // أرباح الشهر الحالي
        public decimal TotalEarnings { get; set; }   // إجمالي الأرباح منذ التسجيل
    }
}
