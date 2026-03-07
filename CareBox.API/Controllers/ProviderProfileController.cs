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
    }
}
