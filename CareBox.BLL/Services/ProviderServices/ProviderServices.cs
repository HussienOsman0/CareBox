using CareBox.BLL.DTOs.ProviderDto.Profile;
using CareBox.BLL.Repositories.Interfaces;
using CareBox.BLL.Services.FileServices.Interfaces;
using CareBox.BLL.Services.ProviderServices.Interfaces;
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

    }
}
