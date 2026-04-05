using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.DTOs.BookingDto
{
    public class ProviderBookingResponseDto
    {
        public long BookingId { get; set; }
        public string BookingCode { get; set; }
        public string ClientName { get; set; }
        public string VehicleDetails { get; set; }
        public DateTime AppointmentDateTime { get; set; }
        public string Status { get; set; }
        public string? ProblemDescription { get; set; }
        public List<string> ServicesIncluded { get; set; }=new List<string>();

    }
}
