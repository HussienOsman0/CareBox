using CareBox.BLL.DTOs.OrderDto;
using CareBox.BLL.Services.OrderService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CareBox.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
       
            private readonly IOrderService _orderService;

            public OrdersController(IOrderService orderService)
            {
                _orderService = orderService;
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



            #region Checkout
        [HttpPost("Client/Checkout")]
        [Authorize(Roles = "CLIENT")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutRequestDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();

                var result = await _orderService.CheckoutAsync(userId, dto);

                if (result)
                    return Ok(new { success = true, message = "The order was successfully completed." });

                return BadRequest(new { success = false, message = "The request was not completed." });
            }
            catch (Exception ex)
            {
                // هيستقبل رسائل الـ Exception زي "المنتج لم يعد متوفر"
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        #endregion


            #region GetClientOrders
            [HttpGet("my-orders")]
            [Authorize(Roles = "CLIENT")]
            public async Task<IActionResult> GetClientOrders([FromQuery] string? filter)
            {
                try
                {
                    var userId = GetCurrentUserId();
                    var orders = await _orderService.GetClientOrdersAsync(userId, filter);

                    return Ok(new { success = true, data = orders });
                }
                catch (Exception ex)
                {
                    return BadRequest(new { success = false, message = ex.Message });
                }
            }
        #endregion





            #region GetProviderOrders
            [HttpGet("Provider/Orders")]
                [Authorize(Roles = "SERVICEPROVIDER")]
                public async Task<IActionResult> GetProviderOrders([FromQuery] int? status)
                {
                    try
                    {
                        var userId = GetCurrentUserId(); // تأكد من وجود الدالة لاستخراج الـ ID من التوكن
                        var orders = await _orderService.GetProviderOrdersAsync(userId, status);
                        return Ok(new { success = true, data = orders });
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(new { success = false, message = ex.Message });
                    }
                }
            #endregion

            #region GetProviderStats
            [HttpGet("provider-ordersStatus")]
            [Authorize(Roles = "SERVICEPROVIDER")]
            public async Task<IActionResult> GetProviderStats()
            {
                try
                {
                    var userId = GetCurrentUserId();
                    var summary = await _orderService.GetProviderOrderStatsAsync(userId);

                    return Ok(new { success = true, data = summary });
                }
                catch (Exception ex)
                {
                    return BadRequest(new { success = false, message = ex.Message });
                }
            }
            #endregion

            #region UpdateOrderStatus
            [HttpPut("update-status/{orderId}")]
            [Authorize(Roles = "SERVICEPROVIDER")]
            public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromBody] UpdateOrderStatusDto dto)
            {
                try
                {
                    var userId = GetCurrentUserId();
                    var result = await _orderService.UpdateOrderStatusAsync(userId, orderId, dto);

                    if (result)
                        return Ok(new { success = true, message = $"Order status updated to {dto.NewStatus}." });

                    return BadRequest(new { success = false, message = "Could not update order status." });
                }
                catch (Exception ex)
                {
                    return BadRequest(new { success = false, message = ex.Message });
                }
            }
            #endregion



    }
}
