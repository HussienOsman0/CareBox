using CareBox.BLL.DTOs.ClientDto.VehicleDto;
using CareBox.BLL.Repositories.Interfaces;
using CareBox.BLL.Services.ClientServices.Interfaces;
using CareBox.BLL.Services.FileServices.Interfaces;
using CareBox.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.Services.ClientServices
{
    public class VehicleService : IVehicleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;

        public VehicleService(IUnitOfWork unitOfWork, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _fileService = fileService;
        }

        #region Helper

        private async Task<Client> GetClientByUserId(int userId)
        {
            var client = await _unitOfWork.Clients.FindAsync(c => c.AppUserId == userId);
            if (client == null)
                throw new Exception("Client not found.");
            return client;
        }



        #endregion

        #region Add vehicle
        public  async Task<VehicleDto> AddVehicleAsync(int clientId, CreateOrUpdateVehicleDto newVehicle)
        {
            var client = await GetClientByUserId(clientId);

            string ? imageUrl = null;
            if (newVehicle.CarImage != null)
            {
                imageUrl = await _fileService.SaveFileAsync(newVehicle.CarImage, "vehicle-images");
            }
            var vehicle = new Vehicle
            {
                Make = newVehicle.Make,
                Model = newVehicle.Model,
                Year = newVehicle.Year,
                PlateNumber = newVehicle.plateNumber,
                Kilometers = newVehicle.Kilometers,
                CarImageUrl = imageUrl,
                ClientId = client.ClientID
            };
            await _unitOfWork.Vehicles.AddAsync(vehicle);
            await _unitOfWork.SaveAsync();

            return await GetVehicleByIdAsync(clientId, vehicle.VehicleId);
        }
        #endregion




        #region Get Vehicles
        public async Task<IEnumerable<VehicleDto>> GetClientVehiclesAsync(int clientId)
        {
            var client =await GetClientByUserId(clientId);

            var vehicles =await  _unitOfWork.Vehicles.FindAllAsync(v=>v.ClientId == client.ClientID && !v.IsDeleted);
            if (vehicles == null || !vehicles.Any())
                throw new Exception("No vehicles found");


            return vehicles.Select(v=> new VehicleDto
            {
                VehicleId = v.VehicleId,
                Make = v.Make,
                Model = v.Model,
                Year = v.Year,
                plateNumber = v.PlateNumber,
                Kilometers = v.Kilometers,
                CarImage = v.CarImageUrl ?? "No Image"
            });
        }
        #endregion

        #region Get Vehicles by id
        public async Task<VehicleDto> GetVehicleByIdAsync(int clientId, int vehicleId)
        {
            var client = await GetClientByUserId(clientId);
            var vehicle = await _unitOfWork.Vehicles.FindAsync(v => v.VehicleId == vehicleId && v.ClientId == client.ClientID);
            if (vehicle == null)
                throw new Exception("Vehicle not found or you don't have permission to access it");

            return new VehicleDto
            {
                VehicleId = vehicle.VehicleId,
                Make = vehicle.Make,
                Model = vehicle.Model,
                Year = vehicle.Year,
                plateNumber = vehicle.PlateNumber,
                Kilometers = vehicle.Kilometers,
                CarImage = vehicle.CarImageUrl ?? "No Image"
            };
        }
        #endregion


        #region Update Vehicle
        public async Task<VehicleDto> UpdateVehicleAsync(int clientId, int vehicleId, CreateOrUpdateVehicleDto updatedVehicle)
        {
            var client = await GetClientByUserId(clientId);
            var vehicle = await _unitOfWork.Vehicles.FindAsync(v => v.VehicleId == vehicleId && v.ClientId == client.ClientID);
            if (vehicle == null)
                throw new Exception("Vehicle not found or you don't have permission to access it");
            vehicle.Make = updatedVehicle.Make;
            vehicle.Model = updatedVehicle.Model;
            vehicle.Year = updatedVehicle.Year;
            vehicle.PlateNumber = updatedVehicle.plateNumber;
            vehicle.Kilometers = updatedVehicle.Kilometers;
            if (updatedVehicle.CarImage != null)
            {
                // Delete old image if exists
                if (!string.IsNullOrEmpty(vehicle.CarImageUrl))
                {
                    _fileService.DeleteFile(vehicle.CarImageUrl);
                }
                // Save new image
                vehicle.CarImageUrl = await _fileService.SaveFileAsync(updatedVehicle.CarImage, "vehicle-images");
            }
            _unitOfWork.Vehicles.Update(vehicle);
            await _unitOfWork.SaveAsync();

            return await GetVehicleByIdAsync(clientId, vehicle.VehicleId);
        } 
        #endregion


        #region Delete vehicle
        public async Task<bool> DeleteVehicleAsync(int clientId, int vehicleId)
        {
            var client = await GetClientByUserId(clientId);
            var vehicle = await _unitOfWork.Vehicles.FindAsync(v => v.VehicleId == vehicleId && v.ClientId == client.ClientID);
            if (vehicle == null)
                throw new Exception("Vehicle not found.");
            // 👇 التعديل هنا: تحديث الحالة بدل المسح
            vehicle.IsDeleted = true;
            _unitOfWork.Vehicles.Update(vehicle);

        
            await _unitOfWork.SaveAsync();
            return true;
        }

        #endregion


    }
}
