using CareBox.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.DAL.Models
{
    public class Order
    {
        public long OrderId { get; set; } 

        public int ClientId { get; set; } 
        public virtual Client Client { get; set; }

        // 2. ربط الطلب بالسيارة (حسب طلبك عشان القطع تكون معروفة لأي عربية)
        public int? VehicleId { get; set; }
        public virtual Vehicle? Vehicle { get; set; }

        public DateTime OrderDate { get; set; } 
        public OrderStatus Status { get; set; } 
        public decimal TotalAmount { get; set; }

        public int ServiceProviderId { get; set; } // 
        public virtual ServiceProvider ServiceProvider { get; set; }

        // 📦 إضافات التوصيل (Delivery)
        public DeliveryType DeliveryType { get; set; }

        // الحقول دي Nullable (?) عشان لو اختار PickUp هتكون فاضية
        public string? DeliveryAddress { get; set; }
        public string? PhoneNumber { get; set; }
        public string? DeliveryNotes { get; set; }

        public virtual Invoice? Invoice { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
