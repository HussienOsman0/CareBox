using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.DAL.Enums
{
    public enum OrderStatus : byte
    {
        Pending = 1,        // قيد الانتظار (العميل لسه طالب)
        Accepted = 2,       // التاجر وافق عليه
        preparing = 3,     // جاري التجهيز

        OutForDelivery = 4, // في الطريق (لو توصيل)
        ReadyForPickup = 5, // جاهز للاستلام (لو Pick up)


        Completed = 6,      // مكتمل
        Cancelled = 7
    }
}
