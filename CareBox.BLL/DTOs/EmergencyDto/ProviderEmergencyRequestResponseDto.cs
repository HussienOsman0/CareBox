using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.DTOs.EmergencyDto
{
    public class ProviderEmergencyRequestResponseDto
    {
        public long RequestId { get; set; }
        public string ClientName { get; set; } = null!;
        public string VehicleDetails { get; set; } = null!; // "Toyota Corolla (2020) - 123 ABC"
        public string RequestType { get; set; } = null!;
        public string? ManualAddress { get; set; }
        public string Status { get; set; } = null!;
        public string? TechnicianName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
