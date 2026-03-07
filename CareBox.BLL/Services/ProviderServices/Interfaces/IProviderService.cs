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
    }
}
