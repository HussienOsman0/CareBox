using CareBox.BLL.DTOs.DashboardDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.Services.DashboardServices.Interfaces
{
    public interface IDashboardService
    {
        Task<IEnumerable<MonthlyEarningDto>> GetProviderMonthlyEarningsAsync(int providerId, int year);
        Task<IEnumerable<TopRequestedServiceDto>> GetTopRequestedServicesAsync(int userId, int? count = null);

        Task<IEnumerable<MonthlyBookingDto>> GetProviderMonthlyBookingsAsync(int userId, int year);

        Task<IEnumerable<TodayBookingDto>> GetTodayBookingsAsync(int userId);

        // ضيف السطر ده في IUnitOfWork.cs
        Task<ProviderForSparePartsSummaryDto> ProviderForSparePartsSummaryAsync(int userId);

        Task<ProviderEarningsDto> GetProviderEarningsAsync(int userId);

        Task<ProviderEmergencyStatsDto> GetProviderEmergencyStatsAsync(int userId);

        Task<ProviderEmergencyTypeStatsDto> GetProviderEmergencyTypeStatsAsync(int userId);

    }
}
