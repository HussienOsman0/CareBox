using CareBox.BLL.DTOs.ClientDto.Profile;
using CareBox.BLL.Services.ClientServices.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CareBox.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                // نجيب الـ ID من التوكين
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var result = await _clientService.GetUserProfileAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpPut("EditProfile")]
        public async Task<IActionResult> UpdateProfile([FromForm] UpdateClientProfileDto model)
        {
            // [FromForm] مهمة جداً عشان بنرفع ملفات (صورة)
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var result = await _clientService.UpdateUserProfileAsync(userId, model);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        #region List ALL provider

        [HttpGet("providers/{typeId}")]
        public async Task<IActionResult> GetProvidersByType(int typeId, [FromQuery] double lat, [FromQuery] double lon)
        {
            // مثال: api/client/providers/4?lat=30.05&lon=31.25
            // رقم 4 هو الـ ID الخاص بـ Car Care حسب الـ Seeding
            try
            {
                var providers = await _clientService.GetProvidersByTypeAsync(typeId, lat, lon);
                return Ok(providers);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion



    }
}
