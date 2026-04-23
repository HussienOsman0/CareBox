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

        #region Helper
        // دالة مساعدة لتوليد كود فريد
        private string GenerateOrderCode()
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var randomPart = new string(Enumerable.Repeat(chars, 4)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            // التنسيق: CB-السنة-رقم عشوائي
            return $"CB-{DateTime.Now.Year}-{randomPart}";
        }

        #endregion

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
                        OrderCode = GenerateOrderCode(),
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
                OrderCode=o.OrderCode,
                OrderDate = o.OrderDate,
                Status = o.Status.ToString(),
                TotalAmount = o.TotalAmount,
                ProviderName = o.ServiceProvider.Name,
                ItemsCount = o.OrderDetails.Count,
                DeliveryType = o.DeliveryType.ToString()
            }).ToList();
        }
        #endregion


        #region Provider Orders logic
        public async Task<IEnumerable<ProviderOrderResponseDto>> GetProviderOrdersAsync(int providerId, int? status)
        {
            // جلب الـ Provider المرتبط بالمستخدم
            var provider = await _unitOfWork.ServiceProviders.FindAsync(p => p.AppUserId == providerId);
            if (provider == null) throw new Exception("Provider not found.");

            // جلب الطلبات مع كل العلاقات المطلوبة
            var orders = await _unitOfWork.Orders.FindAllAsync(
                o => o.ServiceProviderId == provider.ServiceProviderId,
                new[] { "Client.AppUser", "Vehicle", "OrderDetails.Product" }
            );

            var query = orders.AsQueryable();

            // تطبيق الفلترة حسب الحالة لو موجودة
            if (status.HasValue && status.Value > 0)
            {
                query = query.Where(o => (int)o.Status == status.Value);
            }

            return query.OrderByDescending(o => o.OrderDate).Select(o => new ProviderOrderResponseDto
            {
                OrderId = o.OrderId,
                OrderCode = o.OrderCode,
                OrderDate = o.OrderDate,
                ClientName = o.Client.FullName ?? "Unknown Client",
                CarDetails = o.Vehicle != null ? $"{o.Vehicle.Make} {o.Vehicle.Model} {o.Vehicle.Year}" : "No Car Specified",
                DeliveryType = o.DeliveryType.ToString(),
                DeliveryAddress = o.DeliveryAddress,
                PhoneNumber = o.PhoneNumber,
                DeliveryNotes = o.DeliveryNotes,

                StatusName = o.Status.ToString(),
                TotalPrice = o.TotalAmount,
                Items = o.OrderDetails.Select(d => new ProviderOrderItemDto
                {
                    ProductName = d.Product.Name,
                    Quantity = d.Quantity,
                    UnitPrice = d.PriceAtPurchase
                }).ToList()
            }).ToList();
        }
        #endregion

        #region Get Provider Order Stats
        public async Task<OrderStatusStatsDto> GetProviderOrderStatsAsync(int providerId)
        {
            // 1. التأكد من وجود المزود
            var provider = await _unitOfWork.ServiceProviders.FindAsync(p => p.AppUserId == providerId);
            if (provider == null) throw new Exception("Provider not found.");

            // 2. جلب حالات طلبات التاجر فقط (لا نحتاج لجلب كل البيانات، فقط الـ Status لتقليل الحمل على الداتا بيز)
            var orders = await _unitOfWork.Orders.FindAllAsync(o => o.ServiceProviderId == provider.ServiceProviderId);

            // 3. تجميع البيانات وحساب العدد لكل حالة
            var stats = new OrderStatusStatsDto
            {
                TotalOrders = orders.Count(),
                Pending = orders.Count(o => o.Status == DAL.Enums.OrderStatus.Pending),
                Accepted = orders.Count(o => o.Status == DAL.Enums.OrderStatus.Accepted),
                Preparing = orders.Count(o => o.Status == DAL.Enums.OrderStatus.preparing),
                OutForDelivery = orders.Count(o => o.Status == DAL.Enums.OrderStatus.OutForDelivery),
                ReadyForPickup = orders.Count(o => o.Status == DAL.Enums.OrderStatus.ReadyForPickup),
                Completed = orders.Count(o => o.Status == DAL.Enums.OrderStatus.Completed),
                Cancelled = orders.Count(o => o.Status == DAL.Enums.OrderStatus.Cancelled)
            };

            return stats;
        }
        #endregion


        #region UpdateOrderStatusAsync
        //public async Task<bool> UpdateOrderStatusAsync(int userId, int orderId, OrderStatus newStatus)
        //{
        //    // 1. جلب المورد والتأكد من هويته
        //    var provider = await _unitOfWork.ServiceProviders.FindAsync(p => p.AppUserId == userId);
        //    if (provider == null) throw new Exception("Provider not found.");

        //    // 2. جلب الأوردر مع تفاصيله (OrderDetails)
        //    var order = await _unitOfWork.Orders.FindAsync(
        //        o => o.OrderId == orderId && o.ServiceProviderId == provider.ServiceProviderId,
        //        new[] { "OrderDetails" }
        //    );

        //    if (order == null) throw new Exception("Order not found.");

        //    // 🛡️ بدء الـ Transaction
        //    using var transaction = await _unitOfWork.BeginTransactionAsync();
        //    try
        //    {
        //        // 3. تحديث حالة الأوردر
        //        order.Status = newStatus;
        //        _unitOfWork.Orders.Update(order);

        //        // 4. 👇 لو الحالة "مكتمل"، نكريت الفاتورة أوتوماتيك
        //        if (newStatus == OrderStatus.Completed)
        //        {
        //            var invoice = new Invoice
        //            {
        //                OrderId = order.OrderId,
        //                IssueDate = DateTime.Now,
        //                TotalAmount = order.TotalAmount,
        //                IsDraft = false, // فاتورة نهائية
        //                InvoiceDetails = order.OrderDetails.Select(d => new InvoiceDetail
        //                {
        //                    ItemDescription = $"Product ID: {d.Product.Name} (Qty: {d.Quantity})",
        //                    Price = (d.Quantity * d.PriceAtPurchase)
        //                }).ToList()
        //            };

        //            await _unitOfWork.Invoices.AddAsync(invoice);
        //        }

        //        // 5. حفظ التغييرات
        //        await _unitOfWork.SaveAsync();
        //        await transaction.CommitAsync();

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        await transaction.RollbackAsync();
        //        throw new Exception($"Failed to update status: {ex.Message}");
        //    }
        //}


        public async Task<bool> UpdateOrderStatusAsync(int userId, long orderId, UpdateOrderStatusDto model)
        {
            // 1. البحث عن الطلب (استخدام FindAllAsync لجلب الجداول المرتبطة مثل المنتجات والفاتورة)
            var query = await _unitOfWork.Orders.FindAllAsync(
                o => o.OrderId == orderId,
                new[] { "OrderDetails.Product", "Invoice" }
            );

            var order = query.FirstOrDefault();
            if (order == null)
                throw new Exception("Order not found.");

            // 2. التحقق من صلاحيات المستخدم (هل هو العميل صاحب الطلب أم التاجر؟)
            var client = await _unitOfWork.Clients.FindAsync(c => c.AppUserId == userId);
            var provider = await _unitOfWork.ServiceProviders.FindAsync(p => p.AppUserId == userId);

            bool isClient = client?.ClientID == order.ClientId;
            bool isProvider = provider?.ServiceProviderId == order.ServiceProviderId;

            if (!isClient && !isProvider)
                throw new UnauthorizedAccessException("You are not authorized to update this order.");

            // 3. تطبيق قواعد العمل (Business Rules)

            // أ- لا يمكن تعديل طلب تم إلغاؤه أو إكماله مسبقاً
            if (order.Status == DAL.Enums.OrderStatus.Completed || order.Status == DAL.Enums.OrderStatus.Cancelled)
                throw new Exception($"Cannot change status of a {order.Status} order.");

            // ب- إذا كان المستخدم هو "العميل"، يحق له الإلغاء فقط
            if (isClient && model.NewStatus != DAL.Enums.OrderStatus.Cancelled)
                throw new Exception("Clients are only allowed to cancel orders.");

            // ج- إذا كان "مقدم الخدمة"، لا يمكنه إكمال طلب وهو لا يزال معلقاً (يجب أن يقبله ويجهزه أولاً)
            if (isProvider && order.Status == DAL.Enums.OrderStatus.Pending && model.NewStatus == DAL.Enums.OrderStatus.Completed)
                throw new Exception("Cannot complete a pending order directly. It must be accepted and processed first.");


            // 🛡️ بدء الـ Transaction لضمان الحفظ المزدوج (الطلب + الفاتورة) بأمان
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                // 4. تحديث حالة الطلب
                order.Status = model.NewStatus;
                _unitOfWork.Orders.Update(order);
                // ----------------------------------------------------
                //  منطق إلغاء الطلب (إرجاع المنتجات للمخزن) 🔄
                // ----------------------------------------------------
                if (model.NewStatus == DAL.Enums.OrderStatus.Cancelled)
                {
                    foreach (var detail in order.OrderDetails)
                    {
                        var product = detail.Product;

                        // إضافة الكمية الملغاة مرة أخرى للمخزن
                        product.StockQuantity += detail.Quantity;

                        // تحديث حالة المخزن بناءً على الكمية الجديدة (عشان لو كان OutOfStock يرجع InStock)
                        product.StockStatus = product.StockQuantity == 0 ? DAL.Enums.StockStatus.OutOfStock :
                                             (product.StockQuantity <= 20 ? DAL.Enums.StockStatus.LowStock : DAL.Enums.StockStatus.InStock);

                        _unitOfWork.Products.Update(product);
                    }
                }

                // ----------------------------------------------------
                //  منطق الفواتير (Invoice Logic) - بناءً على طلبك
                // ----------------------------------------------------

                // إنشاء الفاتورة النهائية مباشرة وفقط عند اكتمال الطلب (Completed)
                if (model.NewStatus == DAL.Enums.OrderStatus.Completed)
                {
                    // التأكد من عدم وجود فاتورة سابقة لتجنب التكرار
                    if (order.Invoice == null)
                    {
                        var invoice = new Invoice
                        {
                            OrderId = order.OrderId,


                            IssueDate = DateTime.Now,
                            TotalAmount = order.TotalAmount,
                            
                            IsDraft = false, // الفاتورة نهائية مباشرة وليست مسودة
                            InvoiceDetails = new List<InvoiceDetail>()
                        };

                        // نقل المنتجات التي اشتراها العميل إلى تفاصيل الفاتورة
                        foreach (var detail in order.OrderDetails)
                        {
                            invoice.InvoiceDetails.Add(new InvoiceDetail
                            {
                                ItemDescription = $"{detail.Product.Name} (Qty: {detail.Quantity})",
                               
                                Price = (detail.Quantity * detail.PriceAtPurchase),
                                
                            });
                        }

                        await _unitOfWork.Invoices.AddAsync(invoice);
                    }
                }

                // 6. حفظ التعديلات في قاعدة البيانات
                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                // التراجع عن أي تعديل إذا حدث خطأ (مثل فصل الداتا بيز)
                await transaction.RollbackAsync();
                throw new Exception($"Failed to update status: {ex.Message}");
            }
        }


        #endregion
    }
}
