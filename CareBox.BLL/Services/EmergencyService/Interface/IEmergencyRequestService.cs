using CareBox.BLL.DTOs.EmergencyDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.Services.EmergencyService.Interface
{
    public interface IEmergencyRequestService
    {
        Task<EmergencyBroadcastDto> CreateRequestAsync(int userId, CreateEmergencyRequestDto dto);

        Task<IEnumerable<EmergencyBroadcastDto>> GetPendingRequestsAsync();
        Task<bool> AcceptRequestAsync(int userId, AcceptRequestDto dto);

        Task<EmergencyTrackingResponseDto> GetTrackingDetailsAsync(int userId, long requestId);
        Task<IEnumerable<ClientEmergencyRequestResponseDto>> GetClientEmergencyRequestsAsync(int userId, string? filter = null);
        Task<bool> UpdateEmergencyStatusAsync(int userId, UpdateEmergencyStatusDto model);
    }
}
