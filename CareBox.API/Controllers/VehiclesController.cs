using CareBox.BLL.DTOs.ClientDto.VehicleDto;
using CareBox.BLL.Services.ClientServices.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CareBox.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VehiclesController : ControllerBase
    {
        private readonly IVehicleService _vehicleService;

        public VehiclesController(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        #region Helper
        private int GetUserID()
        {
            var userIdCliam = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdCliam == null)
                throw new Exception("Invalid Token or User found");
            return int.Parse(userIdCliam.Value);
        }
        #endregion

        #region get vehicles
        [HttpGet]
        public async Task<IActionResult> GetMyVehicles()
        {
            try
            {
                var userId = GetUserID();
                var vehicles = await _vehicleService.GetClientVehiclesAsync(userId);
                
                return Ok(new
                {
                    success = true,
                    data = vehicles
                });

            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region Vehicle Details
        [HttpGet("{id}")]

        public async Task<IActionResult> GetVehicleById(int id)
        {
            try
            {
                var userId = GetUserID();
                var vehicle = await _vehicleService.GetVehicleByIdAsync(userId, id);
                return Ok(new
                {
                    success = true,
                    data = vehicle
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region Add Vehicle
        [HttpPost]
        public async Task<IActionResult> AddVehicle([FromForm] CreateOrUpdateVehicleDto newVehicle)
        {
            try
            {
                var userId = GetUserID();
                var vehicle = await _vehicleService.AddVehicleAsync(userId, newVehicle);
                return Ok(new
                {
                    success = true,
                    data = vehicle
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }


        #endregion

        #region Update Vehicle
        [HttpPut("{id}")]

        public async Task<IActionResult> UpdateVehicle(int id, [FromForm] CreateOrUpdateVehicleDto updatedVehicle)
        {
            try
            {
                var userId = GetUserID();
                var vehicle = await _vehicleService.UpdateVehicleAsync(userId, id, updatedVehicle);
                return Ok(new
                {
                    success = true,
                    data = vehicle
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Delete Vehicle
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVehicle(int id)
        {
            try
            {
                var userId = GetUserID();
                var result = await _vehicleService.DeleteVehicleAsync(userId, id);
                if (!result)
                    return NotFound(new { success = false, message = "Vehicle not found or you don't have permission to delete it." });
                return Ok(new
                {
                    success = true,
                    message = "Vehicle deleted successfully."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        #endregion

    }
}
