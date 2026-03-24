using CareBox.BLL.DTOs.ProviderDto.About;
using CareBox.BLL.DTOs.ProviderDto.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.Services.ProviderServices.Interfaces
{
    public interface IProviderService
    {
        public Task<ProviderProfileDto> GetProfileAsync(int userId);
        public Task<ProviderProfileDto> UpdateProfileAsync(int userId, UpdateProviderProfileDto updateProviderProfileDto);

        Task<ProviderAboutDto> GetProviderAboutAsync(int providerId);
        Task<ProviderAboutDto> GetProviderAboutForClientAsync(int providerId);
        Task<bool> UpdateProviderAboutAsync(int providerId, UpdateProviderAboutDto dto);
    }
}
