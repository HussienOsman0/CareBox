using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.DTOs.OrderDto
{
    public class ClientOrderResponseDto
    {
        public long OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = null!;
        public decimal TotalAmount { get; set; }
        public string ProviderName { get; set; } = null!;
        public int ItemsCount { get; set; }
        public string DeliveryType { get; set; } = null!;
    }
}
