using CareBox.BLL.DTOs.BookingDto;
using CareBox.BLL.Services.BookingManagementService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CareBox.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "CLIENT")] // السماح للعملاء فقط بإنشاء حجز
    public class BookingsController : ControllerBase
    {
        private readonly IBookingManagementService _bookingService;

        public BookingsController(IBookingManagementService bookingService)
        {
            _bookingService = bookingService;
        }
        #region Helper
        private int GetCurrentUserId()
        {
            var userIdCliam = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdCliam == null)
                throw new Exception("Invalid Token or User found");
            return int.Parse(userIdCliam.Value);
        } 
        #endregion

        [HttpPost("CreateBooking")]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto model)
        {
            try
            {
                // جلب الـ Id الخاص بالمستخدم من التوكن
                var userId = GetCurrentUserId();

                var result = await _bookingService.CreateBookingAsync(userId, model);

                return Ok(new
                {
                    success = true,
                    message = "Booking created successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
    }
}
