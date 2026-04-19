using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.DAL.Models
{
    public class Cart
    {
        public int Id { get; set; }

        // ربط السلة بالعميل
        public int ClientId { get; set; }
        public virtual Client Client { get; set; } = null!;

        // محتويات السلة
        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}
