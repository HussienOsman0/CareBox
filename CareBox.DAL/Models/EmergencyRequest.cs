using CareBox.DAL.Enums;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.DAL.Models
{
    public class EmergencyRequest
    {
        public long RequestId { get; set; } 

        public int ClientId { get; set; } 
        public virtual Client Client { get; set; }

        public int VehicleId { get; set; } 
        public virtual Vehicle Vehicle { get; set; }

        public EmergencyRequestType RequestType { get; set; } // 

        // GEOGRAPHY (Point)
        public Point RequestLocation { get; set; } 

        public RequestStatus Status { get; set; } // 

        public int ServiceProviderId { get; set; } // 
        public virtual ServiceProvider ServiceProvider { get; set; }

        public int? AssignedTechnicianId { get; set; } // 
        public virtual Technician? AssignedTechnician { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
