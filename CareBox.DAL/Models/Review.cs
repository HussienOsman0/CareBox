using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.DAL.Models
{
    public class Review
    {
        public long ReviewId { get; set; } // 

        public int ClientId { get; set; } // 
        public virtual Client Client { get; set; }

        public int ServiceProviderId { get; set; } // 
        public virtual ServiceProvider ServiceProvider { get; set; }

        public byte Rating { get; set; } 
        public string? Comment { get; set; } 
    }
}
