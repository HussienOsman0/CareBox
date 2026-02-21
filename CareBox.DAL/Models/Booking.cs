using CareBox.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.DAL.Models
{
    public class Booking
    {
        public long BookingId { get; set; } 

        public int ClientId { get; set; } 
        public virtual Client Client { get; set; }

        public int VehicleId { get; set; } 
        public virtual Vehicle Vehicle { get; set; }

        public int ServiceProviderId { get; set; } 
        public virtual ServiceProvider ServiceProvider { get; set; }

        public DateTime AppointmentDateTime { get; set; } 
        public BookingStatus Status { get; set; } 
        public string BookingCode { get; set; }

        public virtual Invoice? Invoice { get; set; }
    }
}
