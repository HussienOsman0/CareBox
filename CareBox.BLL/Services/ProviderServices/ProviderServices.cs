using CareBox.BLL.DTOs.ProviderDto.About;
using CareBox.BLL.DTOs.ProviderDto.Profile;
using CareBox.BLL.Repositories.Interfaces;
using CareBox.BLL.Services.FileServices.Interfaces;
using CareBox.BLL.Services.ProviderServices.Interfaces;
using CareBox.DAL.Models;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.Services.ProviderServices
{
    public class ProviderServices: IProviderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;

        public ProviderServices(IUnitOfWork unitOfWork, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _fileService = fileService;
        }

        #region Provider get profile
        public async Task<ProviderProfileDto> GetProfileAsync(int userId)
        {
            var provider = await _unitOfWork.ServiceProviders.FindAsync(
                p => p.AppUserId == userId,
                new[] { "AppUser", "ProviderType" }
                );

            if (provider == null)
                throw new Exception("Provider not found");

            var providerDto = new ProviderProfileDto
            {
                ShopName = provider.Name,
                Email = provider.AppUser.Email,
                PhoneNumber = provider.AppUser.PhoneNumber,
                Address = provider.Address,
                WorkingHours = provider.WorkingHours,
                LogoImageUrl = provider.LogoImageUrl,
                ProviderType = provider.ProviderType.TypeName,
                Latitude = provider.Location?.Y,
                Longitude = provider.Location?.X
            };

            return providerDto;
        }

        
        #endregion

        #region Provider update profile

        public async Task<ProviderProfileDto> UpdateProfileAsync(int userId, UpdateProviderProfileDto dto)
        {
            var provider = await _unitOfWork.ServiceProviders.FindAsync(
                p => p.AppUserId == userId,
                new[] { "AppUser", "ProviderType" }
                );
            if (provider == null)
                throw new Exception("Provider not found");

            provider.Name = dto.ShopName;
            provider.Address = dto.Address;
            provider.WorkingHours = dto.WorkingHours;

            if (dto.Latitude.HasValue && dto.Longitude.HasValue)
            {
                var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
                provider.Location = geometryFactory.CreatePoint(new Coordinate(dto.Longitude.Value, dto.Latitude.Value));
            }

            if (dto.NewLogoImage != null)
            {
                // حذف الصورة القديمة إذا كانت موجودة
                if (!string.IsNullOrEmpty(provider.LogoImageUrl))
                {
                    _fileService.DeleteFile(provider.LogoImageUrl);
                }
                
                provider.LogoImageUrl=await _fileService.SaveFileAsync(dto.NewLogoImage,"providers");
            }
            _unitOfWork.ServiceProviders.Update(provider);
            await _unitOfWork.SaveAsync();

            return await GetProfileAsync(userId);

        }



        #endregion







        #region Get Provider About for client
        public async Task<ProviderAboutDto> GetProviderAboutForClientAsync(int providerId)
        {
            var provider = await _unitOfWork.ServiceProviders.FindAsync(
             p => p.ServiceProviderId == providerId,
             new[] { "ProviderImages" });



            if (provider == null) throw new Exception("Provider profile not found.");

            var imagesList = provider.ProviderImages.Select(img => new ProviderImageDto
            {
                Id = img.Id,
                ImageUrl = img.ImageUrl
            }).ToList();

            // 3. 💡 لو مفيش صور خالص، هنحط عنصر افتراضي بيقول "no photo"
            if (!imagesList.Any())
            {
                imagesList.Add(new ProviderImageDto
                {
                    Id = 0, // Id افتراضي
                    ImageUrl = "no photo" // أو ممكن تحط مسار صورة ديفولت زي "uploads/default.png"
                });
            }

            var result = new ProviderAboutDto
            {
                ServiceProviderId = provider.ServiceProviderId,
                LocationLink= $"https://www.google.com/maps?q={provider.Location.Y},{provider.Location.X}"?? "No Location",
                Description = string.IsNullOrWhiteSpace(provider.Description) ? "No Description" : provider.Description,
                Images = imagesList
            };

            return result;
        }
        #endregion





        #region Get Provider About
        public async Task<ProviderAboutDto> GetProviderAboutAsync(int providerId)
        {
            var provider = await _unitOfWork.ServiceProviders.FindAsync(
             p => p.AppUserId == providerId,
             new[] { "ProviderImages" });



            if (provider == null) throw new Exception("Provider profile not found.");

            var imagesList = provider.ProviderImages.Select(img => new ProviderImageDto
            {
                Id = img.Id,
                ImageUrl = img.ImageUrl
            }).ToList();

            // 3. 💡 لو مفيش صور خالص، هنحط عنصر افتراضي بيقول "no photo"
            if (!imagesList.Any())
            {
                imagesList.Add(new ProviderImageDto
                {
                    Id = 0, // Id افتراضي
                    ImageUrl = "no photo" // أو ممكن تحط مسار صورة ديفولت زي "uploads/default.png"
                });
            }

            var result = new ProviderAboutDto
            {
                ServiceProviderId = provider.ServiceProviderId,
                LocationLink = $"https://www.google.com/maps?q={provider.Location.Y},{provider.Location.X}" ?? "No Location",
                Description = string.IsNullOrWhiteSpace(provider.Description) ? "No Description" : provider.Description,
                Images = imagesList
            };

            return result;
        }
        #endregion

        #region Update Provider About
        public async Task<bool> UpdateProviderAboutAsync(int providerId, UpdateProviderAboutDto dto)
        {
            var provider = await _unitOfWork.ServiceProviders.FindAsync(
             p => p.AppUserId == providerId,
             new[] { "ProviderImages" });

            if (provider == null)
                return false;

            provider.Description = dto.Description;

            if (dto.ImagesToDeleteIds != null && dto.ImagesToDeleteIds.Any())
            {
                var imagesToRemove = provider.ProviderImages
             .Where(img => dto.ImagesToDeleteIds.Contains(img.Id))
             .ToList();

                foreach (var img in imagesToRemove)
                {
                    // حذف من السيرفر (الملفات)
                    _fileService.DeleteFile(img.ImageUrl); // تأكد من اسم الدالة في IFileService

                    // حذف من الداتابيز
                    provider.ProviderImages.Remove(img);
                }
            }
            if (dto.NewImages != null && dto.NewImages.Any())
            {
                foreach (var file in dto.NewImages)
                {
                    // الدالة عندك في المشروع اسمها SaveFileAsync
                    var imageUrl = await _fileService.SaveFileAsync(file, "provider_about");

                    // إضافة الصورة للـ Entity (الـ EF هيربطها أوتوماتيك بالورشة)
                    provider.ProviderImages.Add(new ProviderImage
                    {
                        ImageUrl = imageUrl

                    });
                }

            }
            _unitOfWork.ServiceProviders.Update(provider);
            await _unitOfWork.SaveAsync();

            return true;


        } 
        #endregion


    }
}
