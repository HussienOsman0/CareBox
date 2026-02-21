using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.DAL.Models
{
    public class ListingImage
    {
        public long ImageId { get; set; } 

        public long ListingId { get; set; } 
        public virtual Listing Listing { get; set; }

        public string ImageUrl { get; set; } 
    }
}
