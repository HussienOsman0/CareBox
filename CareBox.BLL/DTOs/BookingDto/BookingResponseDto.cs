using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.DTOs.BookingDto
{
    public class BookingResponseDto
    {
        public long BookingId { get; set; }
        public string BookingCode { get; set; } = null!;
        public string ProviderName { get; set; } = null!;
        public string VehicleDetails { get; set; } = null!;
        public DateTime AppointmentDateTime { get; set; }
        public string Status { get; set; } = null!;
        public decimal TotalPrice { get; set; }
        public List<string> ServicesIncluded { get; set; } = new List<string>();
    
    }
}
