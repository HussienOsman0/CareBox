using CareBox.BLL.DTOs.ClientDto.ClientWithproviders;
using CareBox.BLL.DTOs.ClientDto.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.Services.ClientServices.Interfaces
{
    public interface IClientService
    {

        Task<ClientProfileDto> GetUserProfileAsync(string userId);


        Task<ClientProfileDto> UpdateUserProfileAsync(string userId, UpdateClientProfileDto model);

        Task<IEnumerable<ProviderCardDto>> GetProvidersByTypeAsync(byte typeId);
    }
}
