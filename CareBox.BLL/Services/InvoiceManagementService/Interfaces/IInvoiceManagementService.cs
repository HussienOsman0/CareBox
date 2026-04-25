using CareBox.BLL.DTOs.InvoiceDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.Services.InvoiceManagementService.Interfaces
{
    public interface IInvoiceManagementService
    {
        // تعديل الدالة لتقبل لستة من العناصر مرة واحدة
        Task<bool> AddCustomItemsToInvoiceAsync(int providerUserId, AddMultipleInvoiceItemsDto model);

        Task<IEnumerable<ClientInvoiceResponseDto>> GetClientInvoicesAsync(int userId);
        Task<IEnumerable<ProviderInvoiceResponseDto>> GetProviderInvoicesAsync(int userId);



        Task<ClientInvoiceResponseDto> GetClientInvoiceByBookingIdAsync(int userId, long bookingId);
        Task<ClientInvoiceResponseDto> GetClientInvoiceByEmergencyRequestIdAsync(int userId, long EmergencyRequestId);
        Task<ClientInvoiceResponseDto> GetClientInvoiceByOrderIdAsync(int userId, int orderId);


        Task<ProviderInvoiceResponseDto> GetProviderInvoiceByEmergencyRequestIdAsync(int userId, long EmergencyRequestId);
        Task<ProviderInvoiceResponseDto> GetProviderInvoiceByBookingIdAsync(int userId, long bookingId);


        Task<bool> UpdateInvoiceItemPriceAsync(int providerUserId, long invoiceDetailId, decimal newPrice);
        Task<bool> RemoveInvoiceItemAsync(int providerUserId, long invoiceDetailId);
    }
}

