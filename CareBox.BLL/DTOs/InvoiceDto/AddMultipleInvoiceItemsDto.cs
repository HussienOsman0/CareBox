using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.DTOs.InvoiceDto
{
    public class AddMultipleInvoiceItemsDto
    {
        
        public long BookingId { get; set; }

        public List<InvoiceItemDto> Items { get; set; } = new List<InvoiceItemDto>();
    }
}
