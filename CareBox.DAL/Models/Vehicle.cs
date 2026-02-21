using CareBox.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.DAL.Models
{
    public class Vehicle
    {
        public int VehicleId { get; set; } 

        public int ClientId { get; set; }  
        public virtual Client Client { get; set; }
        public string? CarImageUrl { get; set; }

        //public VehicleType VehicleType     // removed
        public string Make { get; set; } 
        public string Model { get; set; } 
        public short Year { get; set; } 
        
        public int Kilometers { get; set; }

        //  added PlateNumber (new)
        public string PlateNumber { get; set; }

        public virtual ICollection<Booking> Bookings { get; set; }
    }
}
