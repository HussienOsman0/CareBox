using CareBox.DAL.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.DAL.Models
{
    public class Listing
    {
        public long ListingId { get; set; } 

        public int SellerClientId { get; set; } 
        public virtual Client SellerClient { get; set; }

        public int VehicleId { get; set; } 
        public virtual Vehicle Vehicle { get; set; }

        public string Title { get; set; } 
        public string Description { get; set; } 
        public decimal Price { get; set; } 
        public ListingStatus Status { get; set; } 

       
        public string Location { get; set; } 

        public string SellerPhoneNumber { get; set; } 

        public virtual ICollection<ListingImage> Images { get; set; } // 
        public virtual ICollection<SavedListing> SavedByUsers { get; set; }
    }
}
