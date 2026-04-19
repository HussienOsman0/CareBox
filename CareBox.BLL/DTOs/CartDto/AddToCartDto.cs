using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.DTOs.CartDto
{
    public class AddToCartDto
    {
        public List<CartItemRequestDto> Items { get; set; } = new List<CartItemRequestDto>();
    }
}
