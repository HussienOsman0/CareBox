using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.DAL.Models
{
    public class Invoice
    {
        public long InvoiceId { get; set; }

        public long? BookingId { get; set; }
        public virtual Booking? Booking { get; set; }

        public long? OrderId { get; set; }
        public virtual Order? Order { get; set; }

        public long? EmergencyRequestId { get; set; }
        public virtual EmergencyRequest? EmergencyRequest { get; set; }

        public decimal TotalAmount { get; set; } 
        public DateTime IssueDate { get; set; }

        public bool IsDraft { get; set; } = true;

        public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; } 
    }
}

