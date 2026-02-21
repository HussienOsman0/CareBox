using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.DAL.Models
{
    public class Client
    {
        public int ClientID { get; set; } 
        public int AppUserId { get; set; } // FK to AppClient (1:1) 
        public string FullName { get; set; } 
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string? ClientImageUrl { get; set; }
        public string? Address { get; set; }
        public virtual AppUser AppUser { get; set; }
        public virtual ICollection<Vehicle> Vehicles { get; set; } 
        public virtual ICollection<Booking> Bookings { get; set; }
        public virtual ICollection<Listing> Listings { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<SavedListing> SavedListings { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
    }
}

