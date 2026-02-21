using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.DAL.Models
{
    public class OrderDetail
    {
        public long OrderId { get; set; } // 
        public virtual Order Order { get; set; }

        public int ProductId { get; set; } // 
        public virtual Product Product { get; set; }

        public int Quantity { get; set; } // 
        public decimal PriceAtPurchase { get; set; } //
    }
}
