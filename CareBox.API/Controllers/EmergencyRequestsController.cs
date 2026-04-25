using CareBox.BLL.DTOs.EmergencyDto;
using CareBox.BLL.Services.EmergencyService.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;


namespace CareBox.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmergencyRequestsController : ControllerBase
    {
        private readonly IEmergencyRequestService _emergencyService;


        public EmergencyRequestsController(
            IEmergencyRequestService emergencyService)

        {
            _emergencyService = emergencyService;

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
        #region CreateRequest
        [HttpPost("Create-EmergencyRequest")]
        [Authorize(Roles = "CLIENT")]
        public async Task<IActionResult> CreateRequest([FromBody] CreateEmergencyRequestDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();

                // 1. إنشاء الطلب في الداتا بيز
                var Data = await _emergencyService.CreateRequestAsync(userId, dto);

                return Ok(new
                {
                    success = true,
                    message = "Effective assistance has been requested; Gary is searching for the fastest workshop...",
                    data = Data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        #endregion
        #region TrackRequest
        [HttpGet("Client/Track-EmergencyRequest/{requestId}")]
        [Authorize(Roles = "CLIENT")]
        public async Task<IActionResult> TrackRequest(long requestId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _emergencyService.GetTrackingDetailsAsync(userId, requestId);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        #endregion
        #region GetMyRequests
        [HttpGet("MyEmergencyRequests")]
        [Authorize(Roles = "CLIENT")]
        public async Task<IActionResult> GetMyRequests([FromQuery] string? filter)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _emergencyService.GetClientEmergencyRequestsAsync(userId, filter);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        #endregion




        [HttpPatch("UpdateEmergencyStatus")]

        public async Task<IActionResult> UpdateStatus([FromBody] UpdateEmergencyStatusDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _emergencyService.UpdateEmergencyStatusAsync(userId, dto);

                if (result)
                    return Ok(new { success = true, message = "The active response status has been updated." });

                return BadRequest(new { success = false, message = "An error occurred while updating." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }






        #region GetPendingRequests
        [Authorize(Roles = "SERVICEPROVIDER")] // تأكيد إن مزود الخدمة بس اللي يشوفها
        [HttpGet("pending-requests")]
        public async Task<IActionResult> GetPendingRequests()
        {
            try
            {
                var requests = await _emergencyService.GetPendingRequestsAsync();

                if (requests == null || !requests.Any())
                {
                    // نرجع مصفوفة فارغة عشان الـ Front-end ميعملش Crash
                    return Ok(new List<EmergencyBroadcastDto>());
                }

                return Ok(requests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "An error occurred while fetching orders", Details = ex.Message });
            }
        }
        #endregion

        #region AcceptRequest
        [HttpPost("Accept-AcceptRequest")]
        [Authorize(Roles = "SERVICEPROVIDER")]
        public async Task<IActionResult> Accept([FromBody] AcceptRequestDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _emergencyService.AcceptRequestAsync(userId, dto);
                return Ok(new { success = true, message = "The request was successfully accepted, the technician is on his way to the customer." });
            }
            catch (Exception ex) { return BadRequest(new { success = false, message = ex.Message }); }
        }

        [HttpPost("Reject/{requestId}")]
        [Authorize(Roles = "PROVIDER")]
        public async Task<IActionResult> Reject(long requestId)
        {
            // الرفض هنا مجرد "تجاهل" من الورشة، الطلب يفضل Pending للباقي
            return Ok(new { success = true, message = "تم تجاهل الطلب." });
        }

        #endregion

       



    }
}
