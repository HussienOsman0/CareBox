using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.DTOs.BookingDto
{
    public class CreateBookingDto
    {
        [Required]
        public int ServiceProviderId { get; set; } // 1. Select Provider

        [Required]
        public int VehicleId { get; set; } // 2. Select Vehicle

        [Required]
        [MinLength(1, ErrorMessage = "You must select at least one service.")]
        public List<int> ServiceIds { get; set; } = new List<int>(); // 3. Select Services (M:M)

        [Required]
        public DateTime AppointmentDateTime { get; set; } // 4. Appointment Date & Time

        public string? ProblemDescription { get; set; }
    }
}
