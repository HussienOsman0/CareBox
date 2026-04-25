using CareBox.BLL.DTOs.TechnicianDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.Services.ProviderServices.Interfaces
{
    public interface ITechnicianService
    {
        Task<IEnumerable<TechnicianResponseDto>> GetAllMyTechniciansAsync(int userId);
        Task<TechnicianResponseDto?> GetTechnicianByIdAsync(int userId, int technicianId);
        Task<bool> AddTechnicianAsync(int userId, CreateTechnicianDto dto);
        Task<bool> UpdateTechnicianAsync(int userId, int technicianId, UpdateTechnicianDto dto);
        Task<bool> DeleteTechnicianAsync(int userId, int technicianId);
    }
}
