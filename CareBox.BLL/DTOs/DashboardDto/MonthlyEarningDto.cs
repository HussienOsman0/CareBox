using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.DTOs.DashboardDto
{
    public class MonthlyEarningDto
    {
        public int MonthNumber { get; set; }        // رقم الشهر (1, 2, 3...)
        public string MonthName { get; set; } = null!; // اسم الشهر (January, February...)
        public decimal TotalEarnings { get; set; } // إجمالي أرباح الشهر
    }
}
