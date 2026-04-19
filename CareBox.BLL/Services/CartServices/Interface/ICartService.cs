using CareBox.BLL.DTOs.CartDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.Services.CartServices.Interface
{
    public interface ICartService
    {
        Task<bool> AddToCartAsync(int userId, AddToCartDto dto);

        Task<bool> RemoveItemFromCartAsync(int userId, int productId);

        // تفريغ السلة بالكامل
        Task<bool> ClearCartAsync(int userId);

        Task<bool> UpdateCartItemQuantityAsync(int userId, int productId, int newQuantity);

        Task<CartResponseDto> GetCartAsync(int userId);
    }
}
