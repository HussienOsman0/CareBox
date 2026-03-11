using CareBox.BLL.DTOs.ProviderDto.Services;
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

    public class ServiceManagementService: IServiceManagementService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ServiceManagementService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
          }

        #region Helpers
        private async Task<ServiceProvider> GetProviderByUserIdAsync(int userId)
        {
            var provider = await _unitOfWork.ServiceProviders.FindAsync(p => p.AppUserId == userId);
            if (provider == null)
                throw new Exception("Provider not found");
            return provider;
        }
        #endregion


        #region Get Provider Services for client Process


        public async Task<IEnumerable<ServiceDto>> GetProviderServicesAsync(int userId)
        {
            var provider = await _unitOfWork.ServiceProviders.FindAsync(p => p.ServiceProviderId == userId);
            if (provider == null)
                throw new Exception("Provider not found");
            var services = await _unitOfWork.Services.FindAllAsync(s => s.ServiceProviderId == provider.ServiceProviderId);
            if (services == null || !services.Any())
                throw new Exception("No services found for this provider");

            return services.Select(s => new ServiceDto
            {
                ServiceId = s.ServiceId,
                ServiceName = s.ServiceName,
                Description = s.Description,
                Price = s.Price
            });
        } 
        #endregion



        #region Get My Services
        public async Task<IEnumerable<ServiceDto>> GetMyServicesAsync(int userId)
        {
            var provider = await GetProviderByUserIdAsync(userId);
            var services = await _unitOfWork.Services.FindAllAsync(s => s.ServiceProviderId == provider.ServiceProviderId);
            if (services == null || !services.Any())
                throw new Exception("No services found for this provider");

            return services.Select(s => new ServiceDto
            {
                ServiceId = s.ServiceId,
                ServiceName = s.ServiceName,
                Description = s.Description,
                Price = s.Price
            });
        } 
        #endregion

        #region GetService By Id
        public async Task<ServiceDto> GetServiceByIdAsync(int userId, int serviceId)
        {
            var provider = await GetProviderByUserIdAsync(userId);

            var service = await _unitOfWork.Services.FindAsync(s => s.ServiceId == serviceId && s.ServiceProviderId == provider.ServiceProviderId);

            if (service == null)
                throw new Exception("Service not found");
            return new ServiceDto
            {
                ServiceId = service.ServiceId,
                ServiceName = service.ServiceName,
                Description = service.Description,
                Price = service.Price
            };
        }
        #endregion

        #region Add Service

        public async Task<ServiceDto> AddServiceAsync(int userId,CreateServiceDto dto)
        {
            var provider = await GetProviderByUserIdAsync(userId);

            var existingService = await _unitOfWork.Services.FindAsync(
                s => s.ServiceName == dto.ServiceName && s.ServiceProviderId == provider.ServiceProviderId
            );

            if (existingService != null)
            {
                throw new Exception($"You already have a service named '{dto.ServiceName}'");
            }
            var newService = new Service
            {
                ServiceProviderId = provider.ServiceProviderId,
                ServiceName = dto.ServiceName,
                Description = dto.Description,
                Price = dto.Price

            };

            await _unitOfWork.Services.AddAsync(newService);
            await _unitOfWork.SaveAsync();

            return new ServiceDto
            {
                ServiceId = newService.ServiceId,
                ServiceName = newService.ServiceName,
                Description = newService.Description,
                Price = newService.Price
            };

        }

        #endregion

        #region Update Service
        public async Task<ServiceDto> UpdateServiceAsync(int userId, int serviceId, UpdateServiceDto dto)
        {
            var provider = await GetProviderByUserIdAsync(userId);

            var service = await _unitOfWork.Services.FindAsync(s => s.ServiceId == serviceId && s.ServiceProviderId == provider.ServiceProviderId);

            if (service == null)
                throw new Exception("Service not found.");

            service.ServiceName = dto.ServiceName;
            service.Description = dto.Description;
            service.Price = dto.Price;

            _unitOfWork.Services.Update(service);
            await _unitOfWork.SaveAsync();

            return new ServiceDto
            {
                ServiceId = service.ServiceId,
                ServiceName = service.ServiceName,
                Description = service.Description,
                Price = service.Price
            };
        }
        #endregion

        #region Delete Service
        public async Task<bool> DeleteServiceAsync(int userId, int serviceId)
        {
            var provider = await GetProviderByUserIdAsync(userId);
            if (provider == null) return false;

            var service = await _unitOfWork.Services.FindAsync(s => s.ServiceId == serviceId && s.ServiceProviderId == provider.ServiceProviderId);

            if (service == null) return false;
            if (service == null)
                throw new Exception("Service not found.");

            _unitOfWork.Services.Delete(service);
            await _unitOfWork.SaveAsync();
            return true;
        }
        #endregion




    }
}
