using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.DAL.Enums
{
    public enum RequestStatus : byte
    {
        Pending = 1,     // جاري البحث عن سيارة إنقاذ (لسه محدش قبل)
        Accepted = 2,    // الورشة قبلت الطلب
        OnTheWay = 3,    // الفني في الطريق للعميل
        Arrived = 4,     // الفني وصل للموقع

        Completed = 5,   // تم الانتهاء (وهنا تطلع الفاتورة)
        Cancelled = 6    // تم الإلغاء
    }
}
