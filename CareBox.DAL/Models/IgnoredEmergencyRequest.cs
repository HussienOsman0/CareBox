using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.DAL.Models
{
    public class IgnoredEmergencyRequest
    {
        public long EmergencyRequestId { get; set; }
        public EmergencyRequest? EmergencyRequest { get; set; }

        public int ServiceProviderId { get; set; }
        public ServiceProvider? ServiceProvider { get; set; }
    }
}
