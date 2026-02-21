using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.DAL.Models
{
    public class Product
    {
        public int ProductId { get; set; } 

        public int ServiceProviderId { get; set; }  
        public virtual ServiceProvider ServiceProvider { get; set; }

        public string Name { get; set; } 
        public string? Description { get; set; } 
        public decimal Price { get; set; } 
        public string ForModel { get; set; } 
        public string Make { get; set; } 
        public short Year { get; set; } 
        public int StockStatus { get; set; } 

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
