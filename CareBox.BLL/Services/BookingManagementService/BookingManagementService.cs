using CareBox.BLL.DTOs.BookingDto;
using CareBox.BLL.Repositories.Interfaces;
using CareBox.BLL.Services.BookingManagementService.Interfaces;
using CareBox.DAL.Enums;
using CareBox.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.Services.BookingManagementService
{
    public class BookingManagementService: IBookingManagementService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BookingManagementService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region Helper
        #region Get Client by id
        private async Task<Client> GetClientByUserId(int userId)
        {
            var client = await _unitOfWork.Clients.FindAsync(c => c.AppUserId == userId);
            if (client == null)
                throw new Exception("Client not found.");
            return client;
        }
        #endregion


        #region Get Service Provider by id
        private async Task<ServiceProvider> GetProviderByUserIdAsync(int userId)
        {
            var provider = await _unitOfWork.ServiceProviders.FindAsync(p => p.AppUserId == userId);
            if (provider == null)
                throw new Exception("Service provider not found.");
            return provider;
        }

        #endregion

        #endregion

        #region Create Booking
        public async Task<BookingResponseDto> CreateBookingAsync(int userId, CreateBookingDto model)
        {
            var client = await GetClientByUserId(userId);
           
            var vehicle = await _unitOfWork.Vehicles.FindAsync(v => v.VehicleId == model.VehicleId && v.ClientId == client.ClientID);
            if (vehicle == null)
                throw new Exception("Vehicle not found or does not belong to the client.");
            var provider = await _unitOfWork.ServiceProviders.FindAsync(p => p.ServiceProviderId == model.ServiceProviderId);
            if (provider == null)
                throw new Exception("Service provider not found.");
            var booking = new Booking
            {
                ClientId = client.ClientID,
                VehicleId = vehicle.VehicleId,
                ServiceProviderId = provider.ServiceProviderId,
                AppointmentDateTime = model.AppointmentDateTime,
                Status = DAL.Enums.BookingStatus.Pending,
                ProblemDescription = model.ProblemDescription,
                BookingCode = $"BKG-{DateTime.Now:yyyyMMdd}-{new Random().Next(1000, 9999)}",
                BookingServices = new List<BookingService>()
            };

            decimal totalPrice = 0;
            var ServicesNames = new List<string>();

            foreach (var serviceId in model.ServiceIds.Distinct())
            {
                var service = await _unitOfWork.Services.FindAsync(s => s.ServiceId == serviceId && s.ServiceProviderId == model.ServiceProviderId);
                if (service == null)
                    throw new Exception($"Service with ID {serviceId} is invalid or does not belong to the selected provider.");
                booking.BookingServices.Add(new BookingService
                {
                    ServiceId = serviceId,
                    Booking = booking,
                    Price = service.Price
                });
                totalPrice += service.Price;
                ServicesNames.Add(service.ServiceName);

            }
            booking.TotalPrice = totalPrice;

            await _unitOfWork.Bookings.AddAsync(booking);
            await _unitOfWork.SaveAsync();
            return new BookingResponseDto
            {
                BookingId = booking.BookingId,
                BookingCode = booking.BookingCode,
                ProviderName = provider.Name,
                VehicleDetails = $"{vehicle.Make} {vehicle.Model} ({vehicle.PlateNumber})",
                AppointmentDateTime = booking.AppointmentDateTime,
                Status = booking.Status.ToString(),
                TotalPrice = totalPrice,
                ServicesIncluded = ServicesNames
            };
        }


        #endregion

        #region Get Provider Bookings
        public async Task<IEnumerable<ProviderBookingResponseDto>> GetProviderBookingsAsync(int providerUserId, BookingStatus? status = null)
        {
            var provider = await GetProviderByUserIdAsync(providerUserId);

            var query = await _unitOfWork.Bookings.FindAllAsync(b => b.ServiceProviderId == provider.ServiceProviderId,
                new[] { "Client", "Client.AppUser", "Vehicle", "BookingServices.Service" }
                );

            if (status.HasValue)
            { 
                query = query.Where(b=> b.Status ==status.Value);
            }

            var sortedBookings = query.OrderByDescending(b=>b.AppointmentDateTime).ToList();

            var response = sortedBookings.Select(b => new ProviderBookingResponseDto
            {
                BookingId = b.BookingId,
                BookingCode = b.BookingCode,
                ClientName = b.Client.FullName,
                VehicleDetails = $"{b.Vehicle.Make} {b.Vehicle.Model} ({b.Vehicle.PlateNumber})",
                AppointmentDateTime = b.AppointmentDateTime,
                ProblemDescription = string.IsNullOrWhiteSpace(b.ProblemDescription) ? "No description provided" : b.ProblemDescription,
                Status = b.Status.ToString(),
                ServicesIncluded = b.BookingServices.Select(bs => bs.Service.ServiceName).ToList()
            });
            return response;

        }
        #endregion

        #region Get Client Bookings
        public async Task<IEnumerable<BookingResponseDto>> GetClientBookingsAsync(int userId, string? filter = null)
        {
            // 1. جلب بيانات العميل باستخدام الـ Helper Method الموجودة عندك مسبقاً
            var client = await GetClientByUserId(userId);

            // 2. جلب الحجوزات الخاصة بهذا العميل مع الجداول المرتبطة
            var query = await _unitOfWork.Bookings.FindAllAsync(
                b => b.ClientId == client.ClientID,
                new[] { "ServiceProvider", "Vehicle", "BookingServices.Service" }
            );

            // 3. تطبيق الفلتر الذكي (Current أو Past)
            if (!string.IsNullOrEmpty(filter))
            {
                if (filter.ToLower() == "current")
                {
                    // الحالية: المعلقة أو المقبولة
                    query = query.Where(b => b.Status == DAL.Enums.BookingStatus.Pending ||
                                             b.Status == DAL.Enums.BookingStatus.Approved);
                }
                else if (filter.ToLower() == "past")
                {
                    // السابقة: المكتملة أو الملغية
                    query = query.Where(b => b.Status == DAL.Enums.BookingStatus.Completed ||
                                             b.Status == DAL.Enums.BookingStatus.Cancelled);
                }
            }

            // 4. الترتيب: الأحدث أولاً (حسب وقت الحجز)
            var sortedBookings = query.OrderByDescending(b => b.AppointmentDateTime).ToList();

            // 5. تحويل البيانات إلى DTO لإرجاعها للعميل
            var response = sortedBookings.Select(b => new BookingResponseDto
            {
                BookingId = b.BookingId,
                BookingCode = b.BookingCode,
                ProviderName = b.ServiceProvider.Name, // افترضت أن الاسم في الـ ServiceProvider هو Name بناءً على كود الـ Create
                VehicleDetails = $"{b.Vehicle.Make} {b.Vehicle.Model} ({b.Vehicle.PlateNumber})",
                AppointmentDateTime = b.AppointmentDateTime,
                Status = b.Status.ToString(),
                TotalPrice = b.TotalPrice,
                ServicesIncluded = b.BookingServices.Select(bs => bs.Service.ServiceName).ToList()
            });

            return response;
        }
        #endregion

        #region Update Booking Status
        public async Task<bool> UpdateBookingStatusAsync(int userId, UpdateBookingStatusDto model)
        {
            // 1. البحث عن الحجز (استخدام FindAllAsync لجلب الجداول المرتبطة مثل الخدمات والفاتورة)
            var query = await _unitOfWork.Bookings.FindAllAsync(
                b => b.BookingId == model.BookingId,
                new[] { "BookingServices.Service", "Invoice" }
            );

            var booking = query.FirstOrDefault();
            if (booking == null)
                throw new Exception("Booking not found.");

            // 2. التحقق من صلاحيات المستخدم (هل هو العميل صاحب الحجز أم مقدم الخدمة؟)
            var client = await _unitOfWork.Clients.FindAsync(c => c.ClientID == booking.ClientId);
            var provider = await _unitOfWork.ServiceProviders.FindAsync(p => p.ServiceProviderId == booking.ServiceProviderId);

            bool isClient = client?.AppUserId == userId;
            bool isProvider = provider?.AppUserId == userId;

            if (!isClient && !isProvider)
                throw new UnauthorizedAccessException("You are not authorized to update this booking.");

            // 3. تطبيق قواعد العمل (Business Rules)

            // أ- لا يمكن تعديل حجز تم إلغاؤه أو إكماله مسبقاً
            if (booking.Status == DAL.Enums.BookingStatus.Completed || booking.Status == DAL.Enums.BookingStatus.Cancelled)
                throw new Exception($"Cannot change status of a {booking.Status} booking.");

            // ب- إذا كان المستخدم هو "العميل"، يحق له الإلغاء فقط
            if (isClient && model.Status != DAL.Enums.BookingStatus.Cancelled)
                throw new Exception("Clients are only allowed to cancel bookings.");

            // ج- إذا كان "مقدم الخدمة"، لا يمكنه إكمال حجز وهو لا يزال معلقاً (يجب أن يقبله أولاً)
            if (isProvider && booking.Status == DAL.Enums.BookingStatus.Pending && model.Status == DAL.Enums.BookingStatus.Completed)
                throw new Exception("Cannot complete a pending booking. It must be approved first.");

            // 4. تحديث حالة الحجز
            booking.Status = model.Status;

            // ----------------------------------------------------
            // 5. منطق الفواتير (Invoice Logic)
            // ----------------------------------------------------

            // أ- عند القبول (Approved): إنشاء فاتورة مسودة (Draft)
            if (model.Status == DAL.Enums.BookingStatus.Approved && booking.Invoice == null)
            {
                var invoice = new Invoice
                {
                    BookingId = booking.BookingId,
                    TotalAmount = booking.TotalPrice,
                    IssueDate = DateTime.Now,
                    IsDraft = true, // الفاتورة تبدأ كمسودة قابلة للتعديل
                    InvoiceDetails = new List<InvoiceDetail>()
                };

                // نقل الخدمات الأساسية التي اختارها العميل إلى الفاتورة
                foreach (var bookingService in booking.BookingServices)
                {
                    invoice.InvoiceDetails.Add(new InvoiceDetail
                    {
                        // إذا لم تكن هناك خدمة مسجلة، نضع وصفاً افتراضياً أو الوصف المكتوب
                        ItemDescription = bookingService.Service?.ServiceName ?? "خدمة إضافية",
                        Price = bookingService.Price
                    });
                }
                await _unitOfWork.Invoices.AddAsync(invoice);
            }

            // ب- عند الاكتمال (Completed): إغلاق الفاتورة وتحويلها لنهائية
            else if (model.Status == DAL.Enums.BookingStatus.Completed)
            {
                if (booking.Invoice != null)
                {
                    booking.Invoice.IsDraft = false; // إغلاق الفاتورة (لم تعد مسودة)
                    booking.Invoice.IssueDate = DateTime.Now; // تحديث تاريخ الإصدار ليوم الانتهاء الفعلي

                    // تحديث إجمالي الحجز ليتطابق مع إجمالي الفاتورة (في حال إضافة الميكانيكي لأي قطع أو مصنعية)
                    booking.TotalPrice = booking.Invoice.TotalAmount;
                }
                else
                {
                    throw new Exception("Cannot complete booking without a draft invoice.");
                }
            }

            // 6. حفظ التعديلات في قاعدة البيانات
            await _unitOfWork.SaveAsync();

            return true;
        }
        #endregion






        #region Get Provider Clients
        public async Task<IEnumerable<ProviderClientResponseDto>> GetProviderClientsAsync(int providerUserId)
        {
            var provider = await GetProviderByUserIdAsync(providerUserId);
            var bookings = await _unitOfWork.Bookings.FindAllAsync(b => b.ServiceProviderId == provider.ServiceProviderId
            , new[] { "Client", "Client.AppUser", "Vehicle" });

            var clientsList = bookings
            .GroupBy(b => new { b.ClientId, b.VehicleId })
            .Select(g => g.First()) // نأخذ أول حجز من كل مجموعة
            .Select(b => new ProviderClientResponseDto
            {
                ClientName = b.Client.FullName,
                ClientPhone = b.Client.AppUser?.PhoneNumber ?? "No Phone Number",
                CarMake = b.Vehicle.Make,
                CarModel = b.Vehicle.Model,
                Kilometers = b.Vehicle.Kilometers
            });
            

            return clientsList;

        }
        #endregion
    }
}
