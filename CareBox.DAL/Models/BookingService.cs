using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.DAL.Models
{
    public class BookingService
    {
        public long BookingId { get; set; }
        public virtual Booking Booking { get; set; }

        public int ServiceId { get; set; }
        public virtual Service Service { get; set; }

        // السعر الفعلي وقت الحجز (مهم جداً للفواتير)
        public decimal Price { get; set; }
    }
}
