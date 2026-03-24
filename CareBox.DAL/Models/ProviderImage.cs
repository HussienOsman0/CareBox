using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.DAL.Models
{
    public class ProviderImage
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } // مسار الصورة

        // العلاقة مع مقدم الخدمة (Foreign Key)
        public int ServiceProviderId { get; set; }
        public virtual ServiceProvider ServiceProvider { get; set; }
    }
}
