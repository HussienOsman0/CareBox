using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.DTOs.DashboardDto
{
    public class ProviderEmergencyStatsDto
    {
        public int TodayRequestsCount { get; set; }     // طلبات اليوم
        public int ActiveRequestsCount { get; set; }    // الطلبات النشطة حالياً
        public int CompletedRequestsCount { get; set; } // الطلبات المكتملة
        public int TotalRequestsCount { get; set; }     // إجمالي الطلبات منذ التسجيل
    }
}
