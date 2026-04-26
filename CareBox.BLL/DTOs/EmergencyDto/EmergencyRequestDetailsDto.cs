using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.DTOs.EmergencyDto
{
    public class EmergencyRequestDetailsDto
    {
        public long RequestId { get; set; }
        public string Status { get; set; } = null!;
        public string RequestType { get; set; } = null!;
        public string? ProblemDescription { get; set; }
        public DateTime CreatedAt { get; set; }

        // معلومات الموقع
        public string LocationURL { get; set; } = null!;
        public string? ManualAddress { get; set; }
        public double? EstimatedDistance { get; set; }
        public int? EstimatedTimeInMinutes { get; set; }

        // معلومات العميل
        public string ClientName { get; set; } = null!;
        public string ClientPhone { get; set; } = null!;

        // معلومات العربية
        public string VehicleDetails { get; set; } = null!;

        // معلومات الفني (تظهر فقط لو الطلب مقبول)
        public int? TechnicianId { get; set; }
        public string? TechnicianName { get; set; }
        public string? TechnicianPhone { get; set; }

    }
}
