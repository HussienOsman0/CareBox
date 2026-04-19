using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.DTOs.OrderDto
{
    public class OrderStatusStatsDto
    {
        public int TotalOrders { get; set; }

        // تفاصيل الحالات
        public int Pending { get; set; }
        public int Accepted { get; set; }
        public int Preparing { get; set; }
        public int OutForDelivery { get; set; }
        public int ReadyForPickup { get; set; }
        public int Completed { get; set; }
        public int Cancelled { get; set; }
    }
}
