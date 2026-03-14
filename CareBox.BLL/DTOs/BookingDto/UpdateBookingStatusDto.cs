using CareBox.DAL.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.DTOs.BookingDto
{
    public class UpdateBookingStatusDto
    {
        [Required]
        public long BookingId { get; set; }
        [Required]
        public BookingStatus Status { get; set; }
    }
}
