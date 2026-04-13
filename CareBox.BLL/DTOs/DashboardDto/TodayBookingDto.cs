using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.DTOs.DashboardDto
{
    public class TodayBookingDto
    {
        public long BookingId { get; set; }
        public string ClientName { get; set; } = null!; // من AppUser المربوط بالعميل
        public string VehicleInfo { get; set; } = null!; // (ماركة + موديل + رقم اللوحة)
    }
}
