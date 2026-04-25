using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.DTOs.EmergencyDto
{
    public class EmergencyBroadcastDto
    {
        public long RequestId { get; set; }
        public string ClientName { get; set; } = null!;
        public string ClientPhone { get; set; } = null!;
        public string VehicleDetails { get; set; } = null!;
        public string LocationURL { get; set; } = null!;
        public string? ManualAddress { get; set; }
        public string RequestTypeName { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
