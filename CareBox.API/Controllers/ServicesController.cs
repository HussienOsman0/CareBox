using CareBox.BLL.DTOs.ProviderDto.Services;
using CareBox.BLL.Services.ProviderServices.Interfaces;
using CareBox.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace CareBox.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ServicesController : ControllerBase
    {
        private readonly IServiceManagementService _serviceManagement;

        public ServicesController(IServiceManagementService serviceManagement)
        {
            _serviceManagement = serviceManagement;
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

        [HttpGet]
        public async Task<IActionResult> GetMyServices()
        {
            try
            {
                int userId = GetCurrentUserId();
                var services = await _serviceManagement.GetMyServicesAsync(userId);
                return Ok(new
                {
                    success = true,
                    data = services
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetServiceById(int id)
        {
            try
            {
                var result = await _serviceManagement.GetServiceByIdAsync(GetCurrentUserId(), id);
                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            catch (Exception ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddService([FromBody] CreateServiceDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var result = await _serviceManagement.AddServiceAsync(GetCurrentUserId(), dto);
                return Ok(new { success = true, message = "Service Added successfully", data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateService(int id, [FromBody] UpdateServiceDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var result = await _serviceManagement.UpdateServiceAsync(GetCurrentUserId(), id, dto);
                return Ok(new { success = true, message = "Service Updated successfully", data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteService(int id)
        {
            try
            {
                var isDeleted = await _serviceManagement.DeleteServiceAsync(GetCurrentUserId(), id);

                // 1. لو الخدمة مش موجودة أو الحذف فشل
                if (!isDeleted)
                    return BadRequest(new { success = false, message = "Service not found or you don't own it" });

                // 2. لو الحذف تم بنجاح، نرجع رسالة نجاح بدلاً من NoContent
                return Ok(new { success = true, message = "Service deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }



    }
}
