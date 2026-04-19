using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.DAL.Models
{
    public class CartItem
    {
        public int Id { get; set; }

        // ربط العنصر بالسلة
        public int CartId { get; set; }
        public virtual Cart Cart { get; set; } = null!;

        // ربط العنصر بالمنتج
        public int ProductId { get; set; }
        public virtual Product Product { get; set; } = null!;

        // الكمية المطلوبة من المنتج ده
        public int Quantity { get; set; }
    }
}
