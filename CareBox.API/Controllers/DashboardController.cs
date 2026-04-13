using CareBox.BLL.Services.DashboardServices.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CareBox.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "SERVICEPROVIDER")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
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
        #region Get Monthly Earnings
        [HttpGet("ProviderDashboard/MonthlyEarnings")]
        [Authorize(Roles = "SERVICEPROVIDER")]
        public async Task<IActionResult> GetMonthlyEarnings([FromQuery] int? year)
        {
            try
            {
                var userId = GetCurrentUserId();

                // لو لم يرسل سنة معينة، نجلب أرباح السنة الحالية الافتراضية
                int targetYear = year ?? DateTime.Now.Year;

                var data = await _dashboardService.GetProviderMonthlyEarningsAsync(userId, targetYear);

                return Ok(new { success = true, data = data });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region Get Top Requested Services
        [HttpGet("ProviderDashboard/TopServices")]
        public async Task<IActionResult> GetTopRequestedServices([FromQuery] int? count)
        {
            try
            {
                var userId = GetCurrentUserId();

                var data = await _dashboardService.GetTopRequestedServicesAsync(userId,count);

                return Ok(new { success = true, data = data });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        #endregion


        #region GetMonthlyBookings
        [HttpGet("ProviderDashboard/MonthlyBookings")]
        public async Task<IActionResult> GetMonthlyBookings([FromQuery] int? year)
        {
            try
            {
                var userId = GetCurrentUserId();

                int targetYear = year ?? DateTime.Now.Year;

                var data = await _dashboardService.GetProviderMonthlyBookingsAsync(userId, targetYear);

                return Ok(new { success = true, data = data });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        #endregion


        #region GetTodayBookings
        [HttpGet("ProviderDashboard/TodayBookings")]
        public async Task<IActionResult> GetTodayBookings()
        {
            try
            {
                var userId = GetCurrentUserId();

                var data = await _dashboardService.GetTodayBookingsAsync(userId);
                return Ok(new { success = true, data = data });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        } 
        #endregion

    }
}
