using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.DAL.Models
{
    public class ServiceProvider
    {
        
        public int ServiceProviderId { get; set; } 

        public int AppUserId { get; set; } 
        public virtual AppUser AppUser { get; set; }

        public byte ProviderTypeId { get; set; } 
        public virtual ProviderType ProviderType { get; set; }

        public string Name { get; set; } 
        public string Address { get; set; }
        public Point Location { get; set; }

        public string WorkingHours { get; set; } 
        public string? LogoImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }=DateTime.Now;

        // --- الحقول الجديدة لصفحة About ---
        public string? Description { get; set; } // وصف مقدم الخدمة
        public virtual ICollection<ProviderImage> ProviderImages { get; set; } = new List<ProviderImage>(); // صور مقدم الخدمة
        // -----------------------------------
        public virtual ICollection<Technician> Technicians { get; set; } 
        public virtual ICollection<Service> Services { get; set; } 
        public virtual ICollection<Product> Products { get; set; } 
        public virtual ICollection<Booking> Bookings { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }

        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        public virtual ICollection<EmergencyRequest> EmergencyRequests { get; set; } = new List<EmergencyRequest>();


        // ضيف دي مع باقي الـ ICollection اللي عندك
        public virtual ICollection<ServiceCategory> ServiceCategories { get; set; } = new List<ServiceCategory>();
    }
}
