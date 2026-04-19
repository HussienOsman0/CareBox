using CareBox.BLL.DTOs.OrderDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.Services.OrderService.Interfaces
{
    public interface IOrderService
    {
        // الدالة دي بترجع true لو الطلبات اتعملت بنجاح
        Task<bool> CheckoutAsync(int userId, CheckoutRequestDto dto);

        Task<IEnumerable<ClientOrderResponseDto>> GetClientOrdersAsync(int userId, string? filter);
    }
}
