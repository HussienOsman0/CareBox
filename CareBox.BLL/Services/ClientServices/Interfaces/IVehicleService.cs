using CareBox.BLL.DTOs.ClientDto.VehicleDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.Services.ClientServices.Interfaces
{
    public interface IVehicleService
    {
        Task<IEnumerable<VehicleDto>> GetClientVehiclesAsync(int clientId);

        Task<VehicleDto> GetVehicleByIdAsync(int clientId, int vehicleId);

        Task<VehicleDto> AddVehicleAsync(int clientId, CreateOrUpdateVehicleDto newVehicle);
        Task<VehicleDto> UpdateVehicleAsync(int clientId, int vehicleId, CreateOrUpdateVehicleDto updatedVehicle);

        Task<bool> DeleteVehicleAsync(int clientId, int vehicleId);
    }
}
