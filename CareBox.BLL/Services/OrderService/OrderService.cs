using CareBox.BLL.DTOs.OrderDto;
using CareBox.BLL.Repositories.Interfaces;
using CareBox.BLL.Services.OrderService.Interfaces;
using CareBox.DAL.Enums;
using CareBox.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

namespace CareBox.BLL.Services.OrderService
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region CheckoutAsync
        public async Task<bool> CheckoutAsync(int userId, CheckoutRequestDto dto)
        {
            // 🛡️ بدء الـ Transaction لحماية العملية بالكامل
            using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                // 1. جلب العميل
                var client = await _unitOfWork.Clients.FindAsync(c => c.AppUserId == userId);
                if (client == null) throw new Exception("The shopping cart is empty!");

                // 2. جلب السلة مع المنتجات
                var cart = await _unitOfWork.Carts.FindAsync(
                    c => c.ClientId == client.ClientID,
                    new[] { "CartItems.Product" }
                );

                if (cart == null || !cart.CartItems.Any())
                    throw new Exception("The shopping cart is empty!");

                // 3. تقسيم الطلبات حسب الورشة/التاجر
                var itemsByProvider = cart.CartItems.GroupBy(ci => ci.Product.ServiceProviderId);

                foreach (var providerGroup in itemsByProvider)
                {
                    decimal totalAmount = 0;

                    // أ. تجهيز الطلب الأساسي
                    var newOrder = new Order
                    {
                        ClientId = client.ClientID,
                        VehicleId = dto.VehicleId,
                        ServiceProviderId = providerGroup.Key, // الـ ProviderId المربوط بالمجموعة دي
                        OrderDate = DateTime.Now,
                        Status = OrderStatus.Pending,

                        DeliveryType = dto.DeliveryType,
                        DeliveryAddress = dto.DeliveryType == DeliveryType.HomeDelivery ? dto.DeliveryAddress : null,
                        PhoneNumber = dto.DeliveryType == DeliveryType.HomeDelivery ? dto.PhoneNumber : null,
                        DeliveryNotes = dto.DeliveryNotes,

                        OrderDetails = new List<OrderDetail>()
                    };

                    // ب. الدوران على المنتجات اللي تبع التاجر ده
                    foreach (var item in providerGroup)
                    {
                        // التحقق من المخزن
                        if (item.Quantity > item.Product.StockQuantity)
                        {
                            throw new Exception($"Sorry, the requested quantity of '{item.Product.Name}' is unavailable. Available: {item.Product.StockQuantity}");
                        }

                        // خصم الكمية من المخزن وتحديث الحالة
                        item.Product.StockQuantity -= item.Quantity;
                        item.Product.StockStatus = item.Product.StockQuantity == 0 ? StockStatus.OutOfStock :
                                                   (item.Product.StockQuantity <= 20 ? StockStatus.LowStock : StockStatus.InStock);

                        _unitOfWork.Products.Update(item.Product);

                        // حساب الإجمالي
                        totalAmount += (item.Quantity * item.Product.Price);

                        // إضافة التفاصيل
                        newOrder.OrderDetails.Add(new OrderDetail
                        {
                            ProductId = item.ProductId,
                            Quantity = item.Quantity,
                            PriceAtPurchase = item.Product.Price
                        });
                    }

                    newOrder.TotalAmount = totalAmount;

                    // حفظ الطلب في الـ DbContext (لسه مرحش الداتا بيز بسبب الـ Transaction)
                    await _unitOfWork.Orders.AddAsync(newOrder);
                }

                // 4. مسح المنتجات من السلة
                foreach (var item in cart.CartItems.ToList())
                {
                    _unitOfWork.CartItems.Delete(item);
                }

                // 5. حفظ كل التعديلات (طلبات جديدة، خصم مخزون، تفريغ السلة)
                await _unitOfWork.SaveAsync();

                // ✅ 6. تأكيد التغييرات في قاعدة البيانات
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                // ❌ في حالة حدوث أي خطأ، نلغي كل العمليات اللي تمت فوق
                await transaction.RollbackAsync();
                throw new Exception($"An error occurred while completing the request: {ex.Message}");
            }
        }
        #endregion

        #region Get Client Orders (Filtered)
        public async Task<IEnumerable<ClientOrderResponseDto>> GetClientOrdersAsync(int userId, string? filter)
        {
            var client = await _unitOfWork.Clients.FindAsync(c => c.AppUserId == userId);
            if (client == null) throw new Exception("Client not found.");

            // جلب جميع طلبات العميل مع بيانات المتجر وتفاصيل الطلب
            var orders = await _unitOfWork.Orders.FindAllAsync(
                o => o.ClientId == client.ClientID,
                new[] { "ServiceProvider", "OrderDetails" }
            );

            var query = orders.AsQueryable();

            // تطبيق الفلترة
            if (filter?.ToLower() == "current") 
            {
                // الطلبات التي لم تنتهِ بعد
                query = query.Where(o => o.Status != OrderStatus.Completed && o.Status != OrderStatus.Cancelled);
            }
            else if (filter?.ToLower() == "past")
            {
                // الطلبات المكتملة أو الملغاة
                query = query.Where(o => o.Status == OrderStatus.Completed || o.Status == OrderStatus.Cancelled);
            }

            return query.OrderByDescending(o => o.OrderDate).Select(o => new ClientOrderResponseDto
            {
                OrderId = o.OrderId,
                OrderDate = o.OrderDate,
                Status = o.Status.ToString(),
                TotalAmount = o.TotalAmount,
                ProviderName = o.ServiceProvider.Name,
                ItemsCount = o.OrderDetails.Count,
                DeliveryType = o.DeliveryType.ToString()
            }).ToList();
        }
        #endregion
    }
}
