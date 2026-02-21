using CareBox.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.DAL.Models
{
    public class Order
    {
        public long OrderId { get; set; } 

        public int ClientId { get; set; } 
        public virtual Client Client { get; set; }

        public DateTime OrderDate { get; set; } 
        public OrderStatus Status { get; set; } 
        public decimal TotalAmount { get; set; } 

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
