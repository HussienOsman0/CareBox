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
            var services = await _unitOfWork.Services.FindAllAsync(s => s.ServiceProviderId == provider.ServiceProviderId&&s.ServiceCategoryId==null);
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
                Price = s.Price,
                CategoryName=s.ServiceCategoryId != null ? _unitOfWork.ServiceCategories.FindAsync(c => c.Id == s.ServiceCategoryId.Value).Result.Name : "No CategoryName"
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

            // اللوجيك الجديد الخاص بالـ Category
            int? categoryId = null;

            if (!string.IsNullOrWhiteSpace(dto.CategoryName))
            {
                // بندور هل الـ Category ده موجود قبل كده ولا لأ (يفضل تتجاهل حالة الأحرف)
                var category = await _unitOfWork.ServiceCategories.FindAsync(
                    c => c.Name.ToLower() == dto.CategoryName.ToLower() && c.ServiceProviderId == provider.ServiceProviderId
                );

                if (category == null)
                {
                    // لو مش موجود، بنكريت واحد جديد
                    category = new ServiceCategory {
                        Name = dto.CategoryName,
                        ServiceProviderId = provider.ServiceProviderId
                    };
                    await _unitOfWork.ServiceCategories.AddAsync(category);

                    // لازم نعمل Save هنا عشان الـ ID بتاع الـ Category الجديد يتولد ونقدر نستخدمه
                    await _unitOfWork.SaveAsync();
                }

                categoryId = category.Id;
            }


            var newService = new Service
            {
                ServiceProviderId = provider.ServiceProviderId,
                ServiceName = dto.ServiceName,
                Description = dto.Description,
                Price = dto.Price,
                ServiceCategoryId = categoryId // هينزل بـ null لو اليوزر مبعتوش، أو هياخد الـ ID لو اتبعت
            };

            await _unitOfWork.Services.AddAsync(newService);
            await _unitOfWork.SaveAsync();

            return new ServiceDto
            {
                ServiceId = newService.ServiceId,
                ServiceName = newService.ServiceName,
                Description = newService.Description,
                Price = newService.Price,
                CategoryName = dto.CategoryName?? "No CategoryName " // بنرجع الاسم اللي اليوزر بعته
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
            // === اللوجيك الخاص بتحديث الـ Category ===
            if (!string.IsNullOrWhiteSpace(dto.CategoryName))
            {
                // بندور على الـ Category بنفس الاسم
                var category = await _unitOfWork.ServiceCategories.FindAsync(
                    c => c.Name.ToLower() == dto.CategoryName.ToLower()&&c.ServiceProviderId == provider.ServiceProviderId
                );

                if (category == null)
                {
                    // لو مش موجود نكريت واحد جديد
                    category = new ServiceCategory { Name = dto.CategoryName, ServiceProviderId = provider.ServiceProviderId };
                    await _unitOfWork.ServiceCategories.AddAsync(category);
                    await _unitOfWork.SaveAsync(); // بنسيف هنا عشان الـ ID الجديد يتولد
                }

                // بنربط الـ Service بالـ Category (سواء القديم اللي لقيناه أو الجديد اللي اتكريت)
                service.ServiceCategoryId = category.Id;
            }
            else
            {
                // لو اليوزر بعت الـ CategoryName فاضي أو null، معناها إنه عاوز يشيل الخدمة دي من أي Category
                service.ServiceCategoryId = null;
            }
            _unitOfWork.Services.Update(service);
            await _unitOfWork.SaveAsync();

            return new ServiceDto
            {
                ServiceId = service.ServiceId,
                ServiceName = service.ServiceName,
                Description = service.Description,
                Price = service.Price,
                CategoryName = dto.CategoryName ?? "No CategoryName "
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




        #region GetCategoriesForProvider
        public async Task<IEnumerable<ServiceCategoryDto>> GetCategoriesForProviderAsync(int providerId)
        {


            // 1. نجيب كل الخدمات بتاعت البروفايدر ده اللي مربوطة بـ Category
            var providerServices = await _unitOfWork.Services.FindAllAsync(
                s => s.ServiceProviderId == providerId && s.ServiceCategoryId != null
            );

            // 2. نستخرج الـ IDs بتاعت الأقسام دي (بدون تكرار)
            var categoryIds = providerServices
                .Select(s => s.ServiceCategoryId.Value)
                .Distinct()
                .ToList();

            if (!categoryIds.Any())
                return new List<ServiceCategoryDto>(); // لو معندوش أقسام نرجع لستة فاضية

            // 3. نجيب بيانات الأقسام من جدول الـ Categories بناءً على الـ IDs
            var categories = await _unitOfWork.ServiceCategories.FindAllAsync(
                c => categoryIds.Contains(c.Id)
            );

            return categories.Select(c => new ServiceCategoryDto
            {
                CategoryId = c.Id,
                CategoryName = c.Name
            });
        } 
        #endregion

        #region Get My Categories for provider
        public async Task<IEnumerable<ServiceCategoryDto>> GetMyCategoriesAsync(int userId)
        {
            var provider = await GetProviderByUserIdAsync(userId);

            // 1. نجيب كل الخدمات بتاعت البروفايدر ده اللي مربوطة بـ Category
            var providerServices = await _unitOfWork.Services.FindAllAsync(
                s => s.ServiceProviderId == provider.ServiceProviderId && s.ServiceCategoryId != null
            );

            // 2. نستخرج الـ IDs بتاعت الأقسام دي (بدون تكرار)
            var categoryIds = providerServices
                .Select(s => s.ServiceCategoryId.Value)
                .Distinct()
                .ToList();

            if (!categoryIds.Any())
                return new List<ServiceCategoryDto>(); // لو معندوش أقسام نرجع لستة فاضية

            // 3. نجيب بيانات الأقسام من جدول الـ Categories بناءً على الـ IDs
            var categories = await _unitOfWork.ServiceCategories.FindAllAsync(
                c => categoryIds.Contains(c.Id)
            );

            return categories.Select(c => new ServiceCategoryDto
            {
                CategoryId = c.Id,
                CategoryName = c.Name
            });
        }
        #endregion

        #region Get Services By Category
        public async Task<IEnumerable<ServiceDto>> GetServicesByCategoryIdAsync(int prociderId, int categoryId)
        {
            

            // بنجيب الخدمات اللي تبع البروفايدر ده وكمان تبع الـ Category ده
            var services = await _unitOfWork.Services.FindAllAsync(
                s => s.ServiceProviderId == prociderId && s.ServiceCategoryId == categoryId
            );

            if (services == null || !services.Any())
                throw new Exception("No services found in this category.");

            // عشان نرجع اسم القسم في الـ DTO، هنجيبه الأول
            var category = await _unitOfWork.ServiceCategories.FindAsync(c => c.Id == categoryId);

            return services.Select(s => new ServiceDto
            {
                ServiceId = s.ServiceId,
                ServiceName = s.ServiceName,
                Description = s.Description,
                Price = s.Price,
                CategoryName = category?.Name // بنبعت اسم القسم
            });
        }
        #endregion



    }
}
