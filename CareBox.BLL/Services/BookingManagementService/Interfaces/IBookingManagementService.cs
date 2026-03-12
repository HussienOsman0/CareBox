using CareBox.BLL.DTOs.BookingDto;
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
    }
}
