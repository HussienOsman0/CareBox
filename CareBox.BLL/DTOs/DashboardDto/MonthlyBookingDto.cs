using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.DTOs.DashboardDto
{
    public class MonthlyBookingDto
    {
        public int Month { get; set; }
        public string MonthName { get; set; } = null!;
        public int BookingCount { get; set; } // عدد الحجوزات
    }
}
