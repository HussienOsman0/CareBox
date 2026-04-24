using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.DAL.Enums
{
    public enum EmergencyRequestType : byte
    {
        Maintenance = 1, // صيانة عامة
        Battery = 2, // بطارية نايمة
        FlatTire = 3,    // كاوتش مهوي/مخروم
        Accident = 4,    // حادثة / محتاج ونش
        FuelShortage = 5     // البنزين خلص
    }
}
