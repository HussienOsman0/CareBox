using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.DTOs.CartDto
{
    public class CartResponseDto
    {
        public List<CartItemResponseDto> Items { get; set; } = new List<CartItemResponseDto>();
        public decimal TotalCartPrice { get; set; } // إجمالي السلة بالكامل
    }
}
