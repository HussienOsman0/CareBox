using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.DTOs.DashboardDto
{
    public class ProviderEmergencyTypeStatsDto
    {
        public int Maintenance { get; set; }   // صيانة عامة
        public int DeadBattery { get; set; }   // بطارية نايمة
        public int FlatTire { get; set; }      // كاوتش
        public int Accident { get; set; }      // حادثة / ونش
        public int OutOfGas { get; set; }      // بنزين خلص (Fuel Shortage)
    }
}
