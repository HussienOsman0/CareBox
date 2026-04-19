using CareBox.BLL.DTOs.CartDto;
using CareBox.BLL.Services.CartServices.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CareBox.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "CLIENT")] // لازم يكون العميل عامل Login
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        #region Helper
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                throw new Exception("Invalid Token or User found");
            return int.Parse(userIdClaim.Value);
        }
        #endregion

        #region Add to cart
        [HttpPost("add-to-cart")]
        public async Task<IActionResult> AddMultipleToCart([FromBody] AddToCartDto dto)
        {
            // التحقق من أن القائمة ليست فارغة
            if (dto.Items == null || !dto.Items.Any())
            {
                return BadRequest(new { success = false, message = "There are no products to add to the cart." });
            }

            try
            {
                var userId = GetCurrentUserId();
                var result = await _cartService.AddToCartAsync(userId, dto);

                if (result)
                {
                    return Ok(new { success = true, message = "The products have been successfully added to the cart." });
                }

                return BadRequest(new { success = false, message = "An error occurred while adding products to the cart." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region Remove Item
        [HttpDelete("remove-item/{productId}")]
        public async Task<IActionResult> RemoveItem(int productId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _cartService.RemoveItemFromCartAsync(userId, productId);

                if (result)
                {
                    return Ok(new { success = true, message = "The product has been successfully removed from the cart." });
                }

                return BadRequest(new { success = false, message = "The product was not found in the basket or an error occurred." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region Clear Cart
        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _cartService.ClearCartAsync(userId);

                if (result)
                {
                    return Ok(new { success = true, message = "The basket was emptied successfully." });
                }

                return BadRequest(new { success = false, message = "An error occurred while emptying the basket." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region UpdateQuantity
        [HttpPatch("update-quantityForProduct/{productId}")]
        [Authorize(Roles = "CLIENT")]
        public async Task<IActionResult> UpdateQuantity(int productId, [FromBody] UpdateCartItemQuantityDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();

                var result = await _cartService.UpdateCartItemQuantityAsync(userId, productId, dto.NewQuantity);

                if (result)
                    return Ok(new { success = true, message = "The quantity has been successfully updated." });

                return BadRequest(new { success = false, message = "Failed to update quantity." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region GetMyCart
        [HttpGet("my-cart")]
        public async Task<IActionResult> GetMyCart()
        {
            try
            {
                var userId = GetCurrentUserId();
                var cartData = await _cartService.GetCartAsync(userId);

                return Ok(new { success = true, data = cartData });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        } 
        #endregion


    }
}
