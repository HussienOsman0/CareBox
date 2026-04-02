using CareBox.BLL.DTOs.ClientDto.ClientWithproviders;
using CareBox.BLL.DTOs.ClientDto.Profile;
using CareBox.BLL.Repositories.Interfaces;
using CareBox.BLL.Services.ClientServices.Interfaces;
using CareBox.BLL.Services.FileServices.Interfaces;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.Services.ClientServices
{
    public class ClientService : IClientService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;

        public ClientService(IUnitOfWork unitOfWork, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _fileService = fileService;
        }


        #region Profile
        public async Task<ClientProfileDto> GetUserProfileAsync(string userId)
        {
            // 1. تحويل الـ Id لـ int
            if (!int.TryParse(userId, out int appUserId))
                throw new Exception("Invalid User Id");

            // 2. نجيب العميل ومعه بيانات اليوزر (Join)
            // لاحظ استخدام Includes عشان نجيب Email & Phone من AppUser
            var client = await _unitOfWork.Clients.FindAsync(
                c => c.AppUserId == appUserId,
                new[] { "AppUser" }
            );

            if (client == null)
                throw new Exception("Client not found");

            // 3. Mapping يدوي (أو ممكن تستخدم AutoMapper)
            return new ClientProfileDto
            {
                FullName = client.FullName,
                Address = client.Address ??"no Address",
                ClientImageUrl = client.ClientImageUrl?? "no Image",
                Email = client.AppUser.Email,
                PhoneNumber = client.AppUser.PhoneNumber
            };
        }
        #endregion

        #region Edit Profile
        public async Task<ClientProfileDto> UpdateUserProfileAsync(string userId, UpdateClientProfileDto model)
        {
            if (!int.TryParse(userId, out int appUserId))
                throw new Exception("Invalid User Id");

            var client = await _unitOfWork.Clients.FindAsync(
                c => c.AppUserId == appUserId,
                new[] { "AppUser" }
            );

            if (client == null)
                throw new Exception("Client not found");

            // 1. تحديث البيانات النصية
            client.FullName = model.FullName;
            client.Address = model.Address;
            client.AppUser.PhoneNumber = model.PhoneNumber; // تحديث رقم التليفون في جدول اليوزر

            // 2. تحديث الصورة لو موجودة
            if (model.Image != null)
            {
                // (اختياري) ممكن نمسح الصورة القديمة لو مش الـ default
                if (!string.IsNullOrEmpty(client.ClientImageUrl))
                {
                    _fileService.DeleteFile(client.ClientImageUrl);
                }

                // رفع الصورة الجديدة
                client.ClientImageUrl = await _fileService.SaveFileAsync(model.Image, "clients");
            }

            // 3. حفظ التغييرات في الداتابيز
            _unitOfWork.Clients.Update(client);
            await _unitOfWork.SaveAsync();

            // 4. إرجاع البيانات الجديدة
            return new ClientProfileDto
            {
                FullName = client.FullName,
                Address = client.Address,
                ClientImageUrl = client.ClientImageUrl,
                Email = client.AppUser.Email,
                PhoneNumber = client.AppUser.PhoneNumber
            };
        }
        #endregion

        #region List ALL provider By Type Id

        public async Task<IEnumerable<ProviderCardDto>> GetProvidersByTypeAsync(int providerTypeId, double userLat, double userLong)
        {
            // 1. تحديد موقع العميل الحالي
            var userLocation = new Point(userLong, userLat) { SRID = 4326 };

            // 2. جلب مقدمي الخدمة حسب النوع (مع تحميل الخدمات والتقييمات)
            var providers = await _unitOfWork.ServiceProviders.FindAllAsync(
                p => p.ProviderTypeId == providerTypeId && p.Services.Any(),
                new[] { "Services", "Reviews" }
            );

            var resultList = new List<ProviderCardDto>();

            foreach (var provider in providers)
            {
                // 3. حساب المسافة
                // ملاحظة: Distance بترجع فرق الدرجات، بنضرب في 111.32 عشان نحولها تقريباً لكيلومتر
                double distanceDegrees = provider.Location.Distance(userLocation);
                double distanceKm = distanceDegrees * 111.32;

                // 4. حساب التقييم
                double avgRating = provider.Reviews.Any()
                    ? provider.Reviews.Average(r => r.Rating)
                    : 0.0;

                resultList.Add(new ProviderCardDto
                {
                    ServiceProviderId = provider.ServiceProviderId,
                    Name = provider.Name,
                    Address = provider.Address ?? "No Address",
                    LogoImageUrl = provider.LogoImageUrl?? "LogoImage",
                    Rating = Math.Round(avgRating, 1), // تقريب لرقم عشري واحد
                    
                    DistanceInKm = Math.Round(distanceKm, 2),

                    // نأخذ أول 3 خدمات فقط للعرض في الكارت
                    Services = provider.Services.Any()
                       ? provider.Services.Select(s => s.ServiceName).Take(3).ToList()
                       : new List<string> { "No services available" }
                });
            }

            // 5. ترتيب النتائج حسب الأقرب
            return resultList.OrderBy(x => x.DistanceInKm);
        }

        #endregion


    }
}
