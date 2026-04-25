using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.DTOs.InvoiceDto
{
    public class InvoiceItemDto
    {
        public long ItemId { get; set; }
        public string ItemDescription { get; set; } = null!;
        public decimal Price { get; set; }
    }
}
