using CareBox.BLL.DTOs.TechnicianDto;
using CareBox.BLL.Services.ProviderServices.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CareBox.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "SERVICEPROVIDER")] // 👈 حماية الكنترولر بالكامل للورش فقط
    public class TechnicianController : ControllerBase
    {
        private readonly ITechnicianService _technicianService;

        public TechnicianController(ITechnicianService technicianService)
        {
            _technicianService = technicianService;
        }

        #region Helper
        private int GetUserId()
        {
            var userIdCliam = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdCliam == null)
                throw new Exception("Invalid Token or User found");
            return int.Parse(userIdCliam.Value);
        }
        #endregion

        [HttpGet("my-technicians")]
        public async Task<IActionResult> GetAllMyTechnicians()
        {
            try
            {
                var result = await _technicianService.GetAllMyTechniciansAsync(GetUserId());
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex) { return BadRequest(new { success = false, message = ex.Message }); }
        }

        [HttpGet("my-Active-technicians")]
        public async Task<IActionResult> GetMyActiveTechnicians()
        {
            try
            {
                var result = await _technicianService.GetMyActiveTechniciansAsync(GetUserId());
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex) { return BadRequest(new { success = false, message = ex.Message }); }
        }

        [HttpPost("add-technician")]
        public async Task<IActionResult> Add([FromBody] CreateTechnicianDto dto)
        {
            try
            {
                var result = await _technicianService.AddTechnicianAsync(GetUserId(), dto);
                if (result) return Ok(new { success = true, message = "Technician Added" });
                return BadRequest(new { success = false, message = "Technician Added Failed" });
            }
            catch (Exception ex) { return BadRequest(new { success = false, message = ex.Message }); }
        }

        [HttpPut("update-technician/{idTechnician}")]
        public async Task<IActionResult> Update(int idTechnician, [FromBody] UpdateTechnicianDto dto)
        {
            try
            {
                var result = await _technicianService.UpdateTechnicianAsync(GetUserId(), idTechnician, dto);
                if (result) return Ok(new { success = true, message = "technician Updated" });
                return BadRequest(new { success = false, message = "technician Updated Failed" });
            }
            catch (Exception ex) { return BadRequest(new { success = false, message = ex.Message }); }
        }

        [HttpDelete("delete-technician/{idTechnician}")]
        public async Task<IActionResult> Delete(int idTechnician)
        {
            try
            {
                var result = await _technicianService.DeleteTechnicianAsync(GetUserId(), idTechnician);
                if (result) return Ok(new { success = true, message = "technician Deleted" });
                return BadRequest(new { success = false, message = "technician Deleted failed" });
            }
            catch (Exception ex) { return BadRequest(new { success = false, message = ex.Message }); }
        }
    }
}
