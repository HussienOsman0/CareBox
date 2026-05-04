using CareBox.DAL.Enums;
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

        // الربط مع الفئة (اختياري)
        public int? ProductCategoryId { get; set; }
        public virtual ProductCategory ProductCategory { get; set; }

        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string ForModel { get; set; }
        public string Make { get; set; }
        public short Year { get; set; }

        public string? ProductImageUrl { get; set; } 
        public int StockQuantity { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public bool IsDeleted { get; set; } = false;

        //Enums
        public StockStatus StockStatus { get; set; }
        public ProductCondition Condition { get; set; }
        public HorizontalPosition? HorizontalPosition { get; set; } // Front / Rear
        public VerticalPosition? VerticalPosition { get; set; }     // Right / Left

        // المنتج ممكن يكون موجود في كذا سلة
        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
