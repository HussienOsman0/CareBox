using CareBox.BLL.DTOs.TechnicianDto;
using CareBox.BLL.Repositories.Interfaces;
using CareBox.BLL.Services.ProviderServices.Interfaces;
using CareBox.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.Services.ProviderServices
{
    public class TechnicianService : ITechnicianService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TechnicianService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        #region Helper
        private async Task<int> GetProviderIdAsync(int userId)
        {
            var provider = await _unitOfWork.ServiceProviders.FindAsync(p => p.AppUserId == userId);
            if (provider == null) throw new Exception("Provider not found");
            return provider.ServiceProviderId;
        }

        #endregion

        public async Task<IEnumerable<TechnicianResponseDto>> GetAllMyTechniciansAsync(int userId)
        {
            int providerId = await GetProviderIdAsync(userId);
            var technicians = await _unitOfWork.Technicians.FindAllAsync(t => t.ServiceProviderId == providerId);

            return technicians.Select(t => new TechnicianResponseDto
            {
                TechnicianId = t.TechnicianId,
                Name = t.Name,
                PhoneNumber = t.PhoneNumber,
                IsAvailable = t.IsAvailable
            });
        }

        public async Task<TechnicianResponseDto?> GetTechnicianByIdAsync(int userId, int technicianId)
        {
            int providerId = await GetProviderIdAsync(userId);
            var t = await _unitOfWork.Technicians.FindAsync(t => t.TechnicianId == technicianId && t.ServiceProviderId == providerId);

            if (t == null) return null;

            return new TechnicianResponseDto
            {
                TechnicianId = t.TechnicianId,
                Name = t.Name,
                PhoneNumber = t.PhoneNumber,
                IsAvailable = t.IsAvailable
            };
        }

        public async Task<bool> AddTechnicianAsync(int userId, CreateTechnicianDto dto)
        {
            int providerId = await GetProviderIdAsync(userId);
            var existingTechnician = await _unitOfWork.Technicians.FindAsync(
                t => t.Name == dto.Name && t.ServiceProviderId == providerId
            );

            if (existingTechnician != null)
            {
                throw new Exception($"You already have a technician named '{dto.Name}'");
            }
            var newTechnician = new Technician
            {
                ServiceProviderId = providerId,
                Name = dto.Name,
                PhoneNumber = dto.PhoneNumber,
                IsAvailable = true // افتراضياً الفني متاح عند إضافته
            };

            await _unitOfWork.Technicians.AddAsync(newTechnician);
            return await _unitOfWork.SaveAsync() > 0;
        }

        public async Task<bool> UpdateTechnicianAsync(int userId, int technicianId, UpdateTechnicianDto dto)
        {
            int providerId = await GetProviderIdAsync(userId);
            var technician = await _unitOfWork.Technicians.FindAsync(t => t.TechnicianId == technicianId && t.ServiceProviderId == providerId);

            if (technician == null) throw new Exception("Technician not found or you don't have access to edit it.");

            technician.Name = dto.Name;
            technician.PhoneNumber = dto.PhoneNumber;
            technician.IsAvailable = dto.IsAvailable;

            _unitOfWork.Technicians.Update(technician);
            return await _unitOfWork.SaveAsync() > 0;
        }

        public async Task<bool> DeleteTechnicianAsync(int userId, int technicianId)
        {
            int providerId = await GetProviderIdAsync(userId);
            var technician = await _unitOfWork.Technicians.FindAsync(t => t.TechnicianId == technicianId && t.ServiceProviderId == providerId);

            if (technician == null) throw new Exception("Technician not found.");

            _unitOfWork.Technicians.Delete(technician);
            return await _unitOfWork.SaveAsync() > 0;
        }
    }
}
