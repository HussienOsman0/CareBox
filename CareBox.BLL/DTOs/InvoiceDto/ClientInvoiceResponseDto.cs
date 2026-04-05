using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.DTOs.InvoiceDto
{
    public class ClientInvoiceResponseDto
    {
        public long InvoiceId { get; set; }
        public DateTime IssueDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string ProviderName { get; set; } = null!;
        public string ProviderType { get; set; } = null!; // اسم نوع مقدم الخدمة
        public List<InvoiceItemDto> Items { get; set; } = new();
    }
}
