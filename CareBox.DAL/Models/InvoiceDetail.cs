using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.DAL.Models
{
    public class InvoiceDetail
    {
        public long InvoiceDetailId { get; set; } 

        public long InvoiceId { get; set; } 
        public virtual Invoice Invoice { get; set; }

        public string ItemDescription { get; set; } 
        public decimal Price { get; set; }
    }
}
