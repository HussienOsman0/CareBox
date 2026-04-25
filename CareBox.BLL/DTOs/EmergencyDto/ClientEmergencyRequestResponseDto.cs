using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.DTOs.EmergencyDto
{
    public class ClientEmergencyRequestResponseDto
    {
        public long RequestId { get; set; }
        public  int providerId { get; set; }
        public string RequestTypeName { get; set; } = null!;
        public string VehicleDetails { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public string? ProviderName { get; set; } // اسم الورشة لو الطلب اتقبل
        public double? TotalAmount { get; set; } // مبلغ الفاتورة لو الطلب مكتمل
        public string TechnicianName { get; set; } = null!;
        public string? TechnicianPhone { get; set; }
    }
}
