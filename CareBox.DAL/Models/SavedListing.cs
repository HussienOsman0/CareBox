using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.DAL.Models
{
    public class SavedListing
    {
        public int ClientId { get; set; } 
        public virtual Client Client { get; set; }

        public long ListingId { get; set; } 
        public virtual Listing Listing { get; set; }
    }
}
