using CareBox.BLL.DTOs.ProviderDto.About;
using CareBox.BLL.DTOs.ProviderDto.Profile;
using CareBox.BLL.Services.ProviderServices.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CareBox.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProviderProfileController : ControllerBase
    {
        private readonly IProviderService _providerService;

        public ProviderProfileController(IProviderService providerService)
        {
            _providerService = providerService;
        }

        #region profile
        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                    return Unauthorized();

                int userId = int.Parse(userIdClaim.Value);

                var result = await _providerService.GetProfileAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromForm] UpdateProviderProfileDto dto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null) return Unauthorized();

                int userId = int.Parse(userIdClaim.Value);

                var result = await _providerService.UpdateProfileAsync(userId, dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion


        #region About


        #region for client
        [HttpGet("About/{providerId}")]
        public async Task<IActionResult> GetProviderAbout(int providerId)
        {
            try
            {
               
               

                // 2. استدعاء الخدمة

                var result = await _providerService.GetProviderAboutForClientAsync(providerId);

                if (result == null)
                    return NotFound(new { success = false, message = "Provider profile not found." });

                // 3. إرجاع النتيجة
                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region for provider
        [HttpGet("About")]
        [Authorize(Roles = "SERVICEPROVIDER")] // السماح للورشة فقط
        public async Task<IActionResult> GetProviderAbout()
        {
            try
            {
                // 1. جلب رقم المستخدم من الـ Token
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(userIdString, out int userId))
                    return Unauthorized(new { success = false, message = "Invalid user token." });

                // 2. استدعاء الخدمة
                var result = await _providerService.GetProviderAboutAsync(userId);

                if (result == null)
                    return NotFound(new { success = false, message = "Provider profile not found." });

                // 3. إرجاع النتيجة
                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }


        [HttpPut("About")]
        [Authorize(Roles = "SERVICEPROVIDER")] // السماح للورشة فقط
        public async Task<IActionResult> UpdateProviderAbout([FromForm] UpdateProviderAboutDto model)
        {
            // 💡 استخدمنا [FromForm] لأننا نستقبل ملفات (صور) ونصوص في نفس الوقت
            try
            {
                // 1. جلب رقم المستخدم من الـ Token
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(userIdString, out int userId))
                    return Unauthorized(new { success = false, message = "Invalid user token." });

                // 2. استدعاء خدمة التحديث
                var isUpdated = await _providerService.UpdateProviderAboutAsync(userId, model);

                if (!isUpdated)
                    return BadRequest(new { success = false, message = "Failed to update About info." });

                // 3. إرجاع رسالة نجاح
                return Ok(new
                {
                    success = true,
                    message = "About information updated successfully."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        } 
        #endregion

        #endregion
    }
}
