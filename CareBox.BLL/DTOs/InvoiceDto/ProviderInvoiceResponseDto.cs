using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.DTOs.InvoiceDto
{
    public class ProviderInvoiceResponseDto
    {
        public long InvoiceId { get; set; }
        public DateTime IssueDate { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsDraft { get; set; }
        public string ClientName { get; set; } = null!;
        public List<InvoiceItemDto> Items { get; set; } = new();
    }
}
