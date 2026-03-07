using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.DTOs.ClientDto.ClientWithproviders
{
    public class ProviderCardDto
    {
       
            public int ServiceProviderId { get; set; } // عشان نستخدمه لما نضغط على الكارت
            public string Name { get; set; } // اسم المكان
            public string Address { get; set; } // العنوان
            public string LogoImageUrl { get; set; } // صورة اللوجو

            // التقييم
            public double Rating { get; set; }
            

            // المسافة
            public double DistanceInKm { get; set; }
            

            // الخدمات (مثلا: غسيل، تلميع..)
            public List<string> Services { get; set; } = new List<string>();
        
    }
}
