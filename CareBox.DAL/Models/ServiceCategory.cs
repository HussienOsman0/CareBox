using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.DAL.Models
{
    public class ServiceCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int ServiceProviderId { get; set; }
        public virtual ServiceProvider ServiceProvider { get; set; } = null!;

        // العلاقة مع الخدمات (Service)
        public virtual ICollection<Service> Services { get; set; } = new List<Service>();
    }
}
