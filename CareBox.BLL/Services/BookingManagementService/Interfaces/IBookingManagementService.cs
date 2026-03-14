using CareBox.BLL.DTOs.BookingDto;
using CareBox.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.Services.BookingManagementService.Interfaces
{
    public interface IBookingManagementService
    {
        Task<BookingResponseDto> CreateBookingAsync(int userId, CreateBookingDto model);


        Task<IEnumerable<ProviderBookingResponseDto>> GetProviderBookingsAsync(int providerUserId,BookingStatus? status=null);
        Task<bool> UpdateBookingStatusAsync(int ProviderId,UpdateBookingStatusDto model);

        Task<IEnumerable<BookingResponseDto>> GetClientBookingsAsync(int userId, string? filter = null);



    }
}
