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

        [HttpGet("providers/carcare")]
        public async Task<IActionResult> GetProvidersTypeCarCare()
        {
            // Car Care Type Id = 4 
            byte typeId = 4;

            var providers = await _clientService.GetProvidersByTypeAsync(typeId);

            // نرجع 200 OK دائماً حتى لو القائمة فارغة (Empty List)
            return Ok(providers);
        }
        [HttpGet("providers/Maintenance")]
        public async Task<IActionResult> GetProvidersTypeMaintenance()
        {
            // Maintenance Type Id = 1
            byte typeId = 1;

            var providers = await _clientService.GetProvidersByTypeAsync(typeId);

            // نرجع 200 OK دائماً حتى لو القائمة فارغة (Empty List)
            return Ok(providers);
        }
        [HttpGet("providers/SpareParts")]
        public async Task<IActionResult> GetProvidersTypeSpareParts()
        {
            // Spare Parts Type Id = 2
            byte typeId = 2;

            var providers = await _clientService.GetProvidersByTypeAsync(typeId);

            // نرجع 200 OK دائماً حتى لو القائمة فارغة (Empty List)
            return Ok(providers);
        }
        [HttpGet("providers/Emergency")]
        public async Task<IActionResult> GetProvidersTypeEmergency()
        {
            // Emergency Type Id = 3 
            byte typeId =3;

            var providers = await _clientService.GetProvidersByTypeAsync(typeId);

            // نرجع 200 OK دائماً حتى لو القائمة فارغة (Empty List)
            return Ok(providers);
        }
        #endregion



    }
}
