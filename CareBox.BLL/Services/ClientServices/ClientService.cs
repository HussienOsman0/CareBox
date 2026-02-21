using CareBox.BLL.DTOs.ClientDto.ClientWithproviders;
using CareBox.BLL.DTOs.ClientDto.Profile;
using CareBox.BLL.Repositories.Interfaces;
using CareBox.BLL.Services.ClientServices.Interfaces;
using CareBox.BLL.Services.FileServices.Interfaces;
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
                Address = client.Address,
                ClientImageUrl = client.ClientImageUrl,
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

        public async Task<IEnumerable<ProviderCardDto>> GetProvidersByTypeAsync(byte typeId)
        {
            // === Future Scalability Switch ===
            // حالياً: false (لأداء أسرع، استعلام بسيط بدون Joins)
            // مستقبلاً: اجعلها true لتفعيل جلب الخدمات والتقييمات
            bool includeDetails = false;

            // تحديد الـ Includes بناءً على المتغير
            string[] includes = includeDetails ? new[] { "Services", "Reviews" } : null;

            // 1. الاستعلام من قاعدة البيانات
            var providers = await _unitOfWork.ServiceProviders.FindAllAsync(
                criteria: p => p.ProviderTypeId == typeId,
                includes: includes
            );

            // 2. التحويل (Mapping)
            var result = providers.Select(p => new ProviderCardDto
            {
                ServiceProviderId = p.ServiceProviderId,
                Name = p.Name,
                Address = p.Address,
                LogoImageUrl = p.LogoImageUrl,
                WorkingHours = p.WorkingHours,

                // منطق ذكي للتعامل مع البيانات المستقبلية
                // إذا كان includeDetails = true والبيانات موجودة، سيتم حسابها
                // إذا كان false، ستعود null فوراً بدون أي overhead
                Rating = (includeDetails && p.Reviews != null && p.Reviews.Any())
                         ? (byte)Math.Round(p.Reviews.Average(r => r.Rating))
                         : null,

                Services = (includeDetails && p.Services != null && p.Services.Any())
                           ? p.Services.Select(s => s.ServiceName).Take(3).ToList() // نكتفي بأول 3 خدمات للعرض
                           : null
            }).ToList();

            return result;
        }

        #endregion


    }
}
