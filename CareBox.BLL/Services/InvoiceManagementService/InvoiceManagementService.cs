using CareBox.BLL.DTOs.InvoiceDto;
using CareBox.BLL.Repositories.Interfaces;
using CareBox.BLL.Services.InvoiceManagementService.Interfaces;
using CareBox.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.Services.InvoiceManagementService
{
    public class InvoiceManagementService : IInvoiceManagementService
    {
        private readonly IUnitOfWork _unitOfWork;

        public InvoiceManagementService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region Helpers
        private async Task<ServiceProvider> GetProviderByUserIdAsync(int userId)
        {
            var provider = await _unitOfWork.ServiceProviders.FindAsync(p => p.AppUserId == userId);
            if (provider == null)
                throw new Exception("Provider not found");
            return provider;
        }
        #endregion

        #region put to Invoice
        public async Task<bool> AddCustomItemsToInvoiceAsync(int providerUserId, AddMultipleInvoiceItemsDto model)
        {
            var provider = await GetProviderByUserIdAsync(providerUserId);

            // 1. البحث عن الفاتورة باستخدام BookingId بدلاً من InvoiceId
            var query = await _unitOfWork.Invoices.FindAllAsync(
                i => i.BookingId == model.BookingId, // التغيير الجوهري هنا
                new[] { "InvoiceDetails", "Booking", "Booking.ServiceProvider" }
            );

            var invoice = query.FirstOrDefault();

            if (invoice == null)
                throw new Exception("No invoice found for this booking.");

            if (invoice.Booking?.ServiceProvider?.AppUserId != providerUserId)
                throw new UnauthorizedAccessException("You are not authorized to edit this invoice.");

            if (!invoice.IsDraft)
                throw new Exception("Cannot add items. The invoice is final and no longer a draft.");

            decimal totalAddedAmount = 0;

            foreach (var item in model.Items)
            {
                var newDetail = new InvoiceDetail
                {
                    ItemDescription = item.ItemDescription,
                    Price = item.Price
                };

                invoice.InvoiceDetails.Add(newDetail);
                totalAddedAmount += item.Price;
            }

            // 5. تحديث السعر الإجمالي للفاتورة وللحجز أيضاً
            invoice.TotalAmount += totalAddedAmount;
            invoice.Booking.TotalPrice = invoice.TotalAmount; // تحديث سعر الحجز ليكون متطابقاً دائماً

            // 6. حفظ التعديلات
            await _unitOfWork.SaveAsync();

            return true;
        }

        #endregion


        #region Get Client Invoices
        #region old logic
        //public async Task<IEnumerable<ClientInvoiceResponseDto>> GetClientInvoicesAsync(int userId)
        //{
        //    // جلب كائن العميل أولاً
        //    var client = await _unitOfWork.Clients.FindAsync(c => c.AppUserId == userId);
        //    if (client == null) throw new Exception("Client not found.");

        //    // جلب الفواتير مع تفاصيل الخدمة ونوع مقدم الخدمة
        //    var invoices = await _unitOfWork.Invoices.FindAllAsync(
        //    i => i.Booking.ClientId == client.ClientID && i.IsDraft == false,
        //    new[] { "InvoiceDetails", "Booking.ServiceProvider.ProviderType" }
        //    );

        //    // المابينج اليدوي (Manual Mapping)
        //    return invoices.OrderByDescending(i => i.IssueDate).Select(i => new ClientInvoiceResponseDto
        //    {
        //        InvoiceId = i.InvoiceId,
        //        IssueDate = i.IssueDate,
        //        TotalAmount = i.TotalAmount,
        //        ProviderName = i.Booking?.ServiceProvider?.Name ?? "N/A",
        //        ProviderType = i.Booking?.ServiceProvider?.ProviderType?.TypeName ?? "N/A",
        //        Items = i.InvoiceDetails.Select(d => new InvoiceItemDto
        //        {
        //            ItemDescription = d.ItemDescription,
        //            Price = d.Price
        //        }).ToList()
        //    });
        //} 
        #endregion


        public async Task<IEnumerable<ClientInvoiceResponseDto>> GetClientInvoicesAsync(int userId)
        {
            // 1. جلب كائن العميل أولاً
            var client = await _unitOfWork.Clients.FindAsync(c => c.AppUserId == userId);
            if (client == null) throw new Exception("Client not found.");

            // 2. جلب الفواتير المرتبطة بالعميل من (الحجوزات، الطلبات، الطلبات الطارئة)
            var invoices = await _unitOfWork.Invoices.FindAllAsync(
                i => i.IsDraft == false &&
                     (
                         (i.Booking != null && i.Booking.ClientId == client.ClientID) ||
                         (i.Order != null && i.Order.ClientId == client.ClientID) ||
                         (i.EmergencyRequest != null && i.EmergencyRequest.ClientId == client.ClientID)
                     ),
                new[]
                {
                "InvoiceDetails",
                "Booking.ServiceProvider.ProviderType",
                "Order.ServiceProvider.ProviderType",
                "EmergencyRequest.ServiceProvider.ProviderType"
                }
            );

            // 3. المابينج اليدوي الديناميكي (Manual Mapping)
            return invoices.OrderByDescending(i => i.IssueDate).Select(i => new ClientInvoiceResponseDto
            {
                InvoiceId = i.InvoiceId,
                IssueDate = i.IssueDate,
                TotalAmount = i.TotalAmount,

                // 👇 البحث عن اسم المورد في المصادر الثلاثة بالترتيب
                ProviderName = i.Booking?.ServiceProvider?.Name
                            ?? i.Order?.ServiceProvider?.Name
                            ?? i.EmergencyRequest?.ServiceProvider?.Name
                            ?? "N/A",

                // 👇 البحث عن نوع المورد في المصادر الثلاثة
                ProviderType = i.Booking?.ServiceProvider?.ProviderType?.TypeName
                            ?? i.Order?.ServiceProvider?.ProviderType?.TypeName
                            ?? i.EmergencyRequest?.ServiceProvider?.ProviderType?.TypeName
                            ?? "N/A",

                Items = i.InvoiceDetails.Select(d => new InvoiceItemDto
                {
                    ItemDescription = d.ItemDescription,
                    Price = d.Price
                }).ToList()
            });
        }
        #endregion

        #region Get Provider Invoices
        public async Task<IEnumerable<ProviderInvoiceResponseDto>> GetProviderInvoicesAsync(int userId)
        {
            var provider = await _unitOfWork.ServiceProviders.FindAsync(p => p.AppUserId == userId);
            if (provider == null) throw new Exception("Provider not found.");

            // 1. جلب الفواتير مع عمل Include للـ 3 أنظمة (الـ Client جوه كل نظام فيهم)
            // لاحظ إننا ضفنا Client مباشر كمان تحسباً لو إنت رابط الفاتورة بالعميل مباشرة في الداتابيز
            var invoices = await _unitOfWork.Invoices.FindAllAsync(
                i => i.IsDraft == false &&
                     (
                        (i.Booking != null && i.Booking.ServiceProviderId == provider.ServiceProviderId) ||
                        (i.Order != null && i.Order.ServiceProviderId == provider.ServiceProviderId) ||
                        (i.EmergencyRequest != null && i.EmergencyRequest.ServiceProviderId == provider.ServiceProviderId)
                     ),
                new[] { "InvoiceDetails", "Booking.Client", "Order.Client", "EmergencyRequest.Client", "Client" });


            return invoices.OrderByDescending(i => i.IssueDate).Select(i => new ProviderInvoiceResponseDto
            {
                InvoiceId = i.InvoiceId,
                IssueDate = i.IssueDate,
                TotalAmount = i.TotalAmount,
                IsDraft = i.IsDraft, // ستكون دائماً false هنا بناءً على الفلتر
                ClientName = i.Booking?.Client?.FullName ??
                             i.Order?.Client?.FullName ??
                             i.EmergencyRequest?.Client?.FullName ?? "N/A",


                Items = i.InvoiceDetails.Select(d => new InvoiceItemDto
                {
                    ItemDescription = d.ItemDescription,
                    Price = d.Price
                }).ToList()
            });
        }
        #endregion




        #region GetClientInvoiceByBookingIdAsync
        // 1. عرض تفاصيل الفاتورة للعميل برقم الحجز
        public async Task<ClientInvoiceResponseDto> GetClientInvoiceByBookingIdAsync(int userId, long bookingId)
        {
            var client = await _unitOfWork.Clients.FindAsync(c => c.AppUserId == userId);
            if (client == null) throw new Exception("Client not found.");

            // العميل لا يرى الفاتورة إلا إذا كانت مغلقة (IsDraft == false)
            var query = await _unitOfWork.Invoices.FindAllAsync(
                i => i.BookingId == bookingId && i.Booking.ClientId == client.ClientID && i.IsDraft == false,
                new[] { "InvoiceDetails", "Booking.ServiceProvider.ProviderType" }
            );

            var invoice = query.FirstOrDefault();
            if (invoice == null)
                throw new Exception("Invoice not found or it is still a draft.");

            return new ClientInvoiceResponseDto
            {
                InvoiceId = invoice.InvoiceId,
                IssueDate = invoice.IssueDate,
                TotalAmount = invoice.TotalAmount,

                ProviderName = invoice.Booking?.ServiceProvider?.Name ?? "N/A",
                ProviderType = invoice.Booking?.ServiceProvider?.ProviderType?.TypeName ?? "N/A",
                Items = invoice.InvoiceDetails.Select(d => new InvoiceItemDto
                {
                    ItemDescription = d.ItemDescription,
                    Price = d.Price
                }).ToList()
            };
        }

        // 2. عرض تفاصيل الفاتورة لمقدم الخدمة برقم الحجز

        #endregion

        #region GetClientInvoiceByEmergencyRequestIdAsync
        // 1. عرض تفاصيل الفاتورة للعميل برقم الحجز
        public async Task<ClientInvoiceResponseDto> GetClientInvoiceByEmergencyRequestIdAsync(int userId, long EmergencyRequestId)
        {
            var client = await _unitOfWork.Clients.FindAsync(c => c.AppUserId == userId);
            if (client == null) throw new Exception("Client not found.");

            // العميل لا يرى الفاتورة إلا إذا كانت مغلقة (IsDraft == false)
            var query = await _unitOfWork.Invoices.FindAllAsync(
                i => i.EmergencyRequestId == EmergencyRequestId && i.EmergencyRequest.ClientId == client.ClientID && i.IsDraft == false,
                new[] { "InvoiceDetails", "EmergencyRequest.ServiceProvider.ProviderType" }
            );

            var invoice = query.FirstOrDefault();
            if (invoice == null)
                throw new Exception("Invoice not found or it is still a draft.");

            return new ClientInvoiceResponseDto
            {
                InvoiceId = invoice.InvoiceId,
                IssueDate = invoice.IssueDate,
                TotalAmount = invoice.TotalAmount,

               
                ProviderName = invoice.EmergencyRequest?.ServiceProvider?.Name ?? "N/A",
                ProviderType = invoice.EmergencyRequest?.ServiceProvider?.ProviderType?.TypeName ?? "N/A",
                Items = invoice.InvoiceDetails.Select(d => new InvoiceItemDto
                {

                    ItemDescription = d.ItemDescription,
                    Price = d.Price
                }).ToList()
            };
        }

        // 2. عرض تفاصيل الفاتورة لمقدم الخدمة برقم الحجز

        #endregion

        #region GetClientInvoiceByOrderIdAsync
        public async Task<ClientInvoiceResponseDto> GetClientInvoiceByOrderIdAsync(int userId, int orderId)
        {
            // 1. التأكد من هوية العميل
            var client = await _unitOfWork.Clients.FindAsync(c => c.AppUserId == userId);
            if (client == null) throw new Exception("Client not found.");

            // 2. البحث عن الفاتورة المربوطة بهذا الطلب والعميل، والتأكد أنها ليست مسودة
            var query = await _unitOfWork.Invoices.FindAllAsync(
                i => i.OrderId == orderId &&
                     i.Order != null &&
                     i.Order.ClientId == client.ClientID &&
                     i.IsDraft == false,
                new[] { "InvoiceDetails", "Order.ServiceProvider.ProviderType" } // Include لبيانات التاجر
            );

            var invoice = query.FirstOrDefault();
            if (invoice == null)
                throw new Exception("Invoice not found or it is still a draft.");

            // 3. التحويل للـ DTO
            return new ClientInvoiceResponseDto
            {
                InvoiceId = invoice.InvoiceId,
                IssueDate = invoice.IssueDate,
                TotalAmount = invoice.TotalAmount,

                // استخراج اسم التاجر (Provider) ونوعه
                ProviderName = invoice.Order?.ServiceProvider?.Name ?? "N/A",
                ProviderType = invoice.Order?.ServiceProvider?.ProviderType?.TypeName ?? "N/A",

                Items = invoice.InvoiceDetails.Select(d => new InvoiceItemDto
                {
                    // استخدمنا ItemName أو ItemDescription حسب المتاح في موديل InvoiceDetail
                    ItemDescription = d.ItemDescription ?? "تفاصيل المنتج",
                    Price = d.Price
                }).ToList()
            };
        }
        #endregion




        #region GetProviderInvoiceByBookingId
        public async Task<ProviderInvoiceResponseDto> GetProviderInvoiceByBookingIdAsync(int userId, long bookingId)
        {
            var provider = await _unitOfWork.ServiceProviders.FindAsync(p => p.AppUserId == userId);
            if (provider == null) throw new Exception("Provider not found.");

            // مقدم الخدمة يرى الفاتورة سواء كانت مسودة أو نهائية (لم نضع شرط IsDraft)
            var query = await _unitOfWork.Invoices.FindAllAsync(
                i => i.BookingId == bookingId && i.Booking.ServiceProviderId == provider.ServiceProviderId,
                new[] { "InvoiceDetails", "Booking.Client" }
            );

            var invoice = query.FirstOrDefault();
            if (invoice == null)
                throw new Exception("Invoice not found for this booking.");

            return new ProviderInvoiceResponseDto
            {
                InvoiceId = invoice.InvoiceId,
                IssueDate = invoice.IssueDate,
                TotalAmount = invoice.TotalAmount,
                IsDraft = invoice.IsDraft,
                ClientName = invoice.Booking?.Client?.FullName ?? "N/A",
                Items = invoice.InvoiceDetails.Select(d => new InvoiceItemDto
                {
                    ItemDescription = d.ItemDescription,
                    Price = d.Price
                }).ToList()
            };
        }
        #endregion

        #region GetProviderInvoice EmergencyRequestId
        public async Task<ProviderInvoiceResponseDto> GetProviderInvoiceByEmergencyRequestIdAsync(int userId, long EmergencyRequestId)
        {
            var provider = await _unitOfWork.ServiceProviders.FindAsync(p => p.AppUserId == userId);
            if (provider == null) throw new Exception("Provider not found.");

            // مقدم الخدمة يرى الفاتورة سواء كانت مسودة أو نهائية (لم نضع شرط IsDraft)
            var query = await _unitOfWork.Invoices.FindAllAsync(
                i => i.EmergencyRequestId == EmergencyRequestId && i.EmergencyRequest.ServiceProviderId == provider.ServiceProviderId,
                new[] { "InvoiceDetails", "EmergencyRequest.Client" }
            );

            var invoice = query.FirstOrDefault();
            if (invoice == null)
                throw new Exception("Invoice not found for this  EmergencyRequest.");

            return new ProviderInvoiceResponseDto
            {
                InvoiceId = invoice.InvoiceId,
                IssueDate = invoice.IssueDate,
                TotalAmount = invoice.TotalAmount,
                IsDraft = invoice.IsDraft,
                ClientName = invoice.Booking?.Client?.FullName ?? "N/A",
                Items = invoice.InvoiceDetails.Select(d => new InvoiceItemDto
                {
                    ItemId= d.InvoiceDetailId,
                    ItemDescription = d.ItemDescription,
                    Price = d.Price
                }).ToList()
            };
        }
        #endregion







        #region Item Edit

        // 1. تعديل سعر بند معين
        public async Task<bool> UpdateInvoiceItemPriceAsync(int providerUserId, long invoiceDetailId, decimal newPrice)
        {
            var provider = await _unitOfWork.ServiceProviders.FindAsync(p => p.AppUserId == providerUserId);
            if (provider == null) throw new Exception("Provider not found.");

            // التعديل هنا: إضافة الجداول المرتبطة (Includes) في ثاني باراميتر
            var query = await _unitOfWork.InvoiceDetails.FindAllAsync(
                d => d.InvoiceDetailId == invoiceDetailId && (
                    (d.Invoice.Booking != null && d.Invoice.Booking.ServiceProviderId == provider.ServiceProviderId) ||
                    (d.Invoice.Order != null && d.Invoice.Order.ServiceProviderId == provider.ServiceProviderId) ||
                    (d.Invoice.EmergencyRequest != null && d.Invoice.EmergencyRequest.ServiceProviderId == provider.ServiceProviderId)
                ),
                new[] { "Invoice", "Invoice.Booking", "Invoice.Order", "Invoice.EmergencyRequest" } // <-- السطر السحري اللي هيحل المشكلة
            );

            var detail = query.FirstOrDefault();

            if (detail == null) throw new Exception("Invoice item not found or you are not authorized.");

            // حماية إضافية للتأكد أن الفاتورة تم تحميلها بنجاح
            if (detail.Invoice == null) throw new Exception("Invoice relation is missing.");

            // التحقق من أن الفاتورة ما زالت مسودة
            if (!detail.Invoice.IsDraft)
                throw new Exception("Cannot edit price. The invoice is final.");

            // تحديث الإجمالي للفاتورة (خصم السعر القديم وإضافة الجديد)
            detail.Invoice.TotalAmount = (detail.Invoice.TotalAmount - detail.Price) + newPrice;

            // تحديث سعر البند
            detail.Price = newPrice;

            await _unitOfWork.SaveAsync();
            return true;
        }
        // 2. مسح بند معين من الفاتورة
        public async Task<bool> RemoveInvoiceItemAsync(int providerUserId, long invoiceDetailId)
        {
            var provider = await _unitOfWork.ServiceProviders.FindAsync(p => p.AppUserId == providerUserId);
            if (provider == null) throw new Exception("Provider not found.");

            var query = await _unitOfWork.InvoiceDetails.FindAllAsync(
                    d => d.InvoiceDetailId == invoiceDetailId && (
                            (d.Invoice.Booking != null && d.Invoice.Booking.ServiceProviderId == provider.ServiceProviderId) ||
                            (d.Invoice.Order != null && d.Invoice.Order.ServiceProviderId == provider.ServiceProviderId) ||
                            (d.Invoice.EmergencyRequest != null && d.Invoice.EmergencyRequest.ServiceProviderId == provider.ServiceProviderId)
                        ),
                     new[] { "Invoice", "Invoice.Booking", "Invoice.Order", "Invoice.EmergencyRequest" } // <-- السطر السحري اللي هيحل المشكلة
                );
            var detail = query.FirstOrDefault();

            if (detail == null) throw new Exception("Invoice item not find.");

            if (!detail.Invoice.IsDraft)
                throw new Exception("Cannot delete items from a final invoice.");

            // خصم السعر من إجمالي الفاتورة قبل الحذف
            detail.Invoice.TotalAmount -= detail.Price;

            _unitOfWork.InvoiceDetails.Delete(detail); // تأكد من وجود دالة Delete في الـ Repository
            await _unitOfWork.SaveAsync();
            return true;
        }

        #endregion
    }


}
