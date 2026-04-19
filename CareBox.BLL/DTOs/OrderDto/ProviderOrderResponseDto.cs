using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.DTOs.OrderDto
{
    public class ProviderOrderResponseDto
    {
        public long OrderId { get; set; }
        public string OrderCode { get; set; } = null!;
        public DateTime OrderDate { get; set; }
        public string ClientName { get; set; } = null!;
        public string? CarDetails { get; set; } // Make - Model - Year

        public List<ProviderOrderItemDto> Items { get; set; } = new();

        public string DeliveryType { get; set; } = null!;
        public string? DeliveryAddress { get; set; }
        public string? PhoneNumber { get; set; }
        public string? DeliveryNotes { get; set; }

        
        public string StatusName { get; set; } = null!;
        public decimal TotalPrice { get; set; }
    }
}
