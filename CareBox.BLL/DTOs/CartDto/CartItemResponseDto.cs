using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.DTOs.CartDto
{
    public class CartItemResponseDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public decimal Price { get; set; }
        public string StoreName { get; set; } = null!; // اسم الـ Provider
        public int StockQuantity { get; set; } // المتاح في المخزن
        public int SelectedQuantity { get; set; } // اللي العميل اختاره
        public decimal TotalItemPrice { get; set; } // (Price * SelectedQuantity)
    }
}
