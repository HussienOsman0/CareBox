using CareBox.BLL.DTOs.BookingDto;
using CareBox.BLL.Services.BookingManagementService.Interfaces;
using CareBox.DAL.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CareBox.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // السماح للعملاء فقط بإنشاء حجز
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


        #region Create booking
        [Authorize(Roles = "CLIENT")]
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
        #endregion

        #region Provider Bookings

        [Authorize(Roles = "SERVICEPROVIDER")]
        [HttpGet("ProviderBookings")]
        public async Task<IActionResult> GetProviderBookings([FromQuery] BookingStatus? status)
        {
            try
            {
                var userId = GetCurrentUserId();
                var bookings = await _bookingService.GetProviderBookingsAsync(userId, status);
                return Ok(new
                {
                    success = true,
                    message = "Bookings retrieved successfully.",
                    data = bookings
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

        #endregion

        #region Client Booking
        [Authorize(Roles = "CLIENT")] 
        [HttpGet("ClientBookings")]
        public async Task<IActionResult> GetClientBookings([FromQuery] string? filter)
        {
            try
            {
                var userId = GetCurrentUserId();

                var result = await _bookingService.GetClientBookingsAsync(userId, filter);

                return Ok(new
                {
                    success = true,
                    message = "Client bookings retrieved successfully.",
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
        #endregion

        #region UpdateStatus


        [Authorize]
        [HttpPatch("UpdateStatus")]
        public async Task<IActionResult> UpdateBookingStatus([FromBody] UpdateBookingStatusDto model)
        {
            try
            {
                var userId = GetCurrentUserId();

                var result = await _bookingService.UpdateBookingStatusAsync(userId, model);

                return Ok(new
                {
                    success = true,
                    message = "Booking status updated successfully."
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message); // إرجاع 403 إذا حاول شخص التعديل على حجز لا يخصه
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
        #endregion


    }
}
