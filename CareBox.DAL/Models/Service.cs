using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.DAL.Models
{
    public class Service
    {
        public int ServiceId { get; set; } 

        public int ServiceProviderId { get; set; } 
        public virtual ServiceProvider ServiceProvider { get; set; }

        public string ServiceName { get; set; } 
        public string? Description { get; set; } 
        public decimal Price { get; set; }
    }
}
