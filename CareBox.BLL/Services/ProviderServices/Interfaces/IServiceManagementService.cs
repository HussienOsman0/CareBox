using CareBox.BLL.DTOs.ProviderDto.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.Services.ProviderServices.Interfaces
{
    public interface IServiceManagementService
    {
        // عرض كل خدمات البروفايدر الحالي
        Task<IEnumerable<ServiceDto>> GetMyServicesAsync(int userId);

        // عرض خدمة معينة بالتفصيل
        Task<ServiceDto> GetServiceByIdAsync(int userId, int serviceId);

        // إضافة خدمة جديدة
        Task<ServiceDto> AddServiceAsync(int userId, CreateServiceDto dto);

        // تعديل خدمة
        Task<ServiceDto> UpdateServiceAsync(int userId, int serviceId, UpdateServiceDto dto);

        // حذف خدمة
        Task<bool> DeleteServiceAsync(int userId, int serviceId);

        Task<IEnumerable<ServiceCategoryDto>> GetCategoriesForProviderAsync(int providerId);
        Task<IEnumerable<ServiceCategoryDto>> GetMyCategoriesAsync(int userId);
        Task<IEnumerable<ServiceDto>> GetServicesByCategoryIdAsync(int userId, int categoryId);

        Task<IEnumerable<ServiceDto>> GetProviderServicesAsync(int userId);
    }
}
