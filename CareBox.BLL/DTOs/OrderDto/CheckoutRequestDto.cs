using CareBox.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.DTOs.OrderDto
{
    public class CheckoutRequestDto
    {
        // اختياري: لو العميل عاوز يربط الطلب بعربية معينة
        public int? VehicleId { get; set; }

        // 1 = PickUp (استلام من الفرع), 2 = HomeDelivery (توصيل)
        public DeliveryType DeliveryType { get; set; }

        // البيانات دي هتتبعت بس لو اختار HomeDelivery
        public string? DeliveryAddress { get; set; }
        public string? PhoneNumber { get; set; }
        public string? DeliveryNotes { get; set; }
    }
}
