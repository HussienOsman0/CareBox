using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.DAL.Models
{
    public class Technician
    {
        public int TechnicianId { get; set; } 

        public int ServiceProviderId { get; set; } 
        public virtual ServiceProvider ServiceProvider { get; set; }

        public string Name { get; set; } //  

        public bool IsAvailable { get; set; } // 

        public virtual ICollection<EmergencyRequest> EmergencyRequests { get; set; }
    }
}
