using CareBox.BLL.DTOs.DashboardDto;
using CareBox.BLL.Repositories.Interfaces;
using CareBox.BLL.Services.DashboardServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.Services.DashboardServices
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region GetProviderMonthlyEarningsAsync
        public async Task<IEnumerable<MonthlyEarningDto>> GetProviderMonthlyEarningsAsync(int userId, int year)
        {
            // 1. جلب بيانات الورشة
            var provider = await _unitOfWork.ServiceProviders.FindAsync(p => p.AppUserId == userId);
            if (provider == null) throw new Exception("Provider not found.");

            var Invoices = await _unitOfWork.Invoices.FindAllAsync(i => i.IssueDate.Year == year && i.IsDraft == false && (
                         (i.Booking != null && i.Booking.ServiceProviderId == provider.ServiceProviderId) ||
                         (i.Order != null && i.Order.ServiceProviderId == provider.ServiceProviderId) ||
                         (i.EmergencyRequest != null && i.EmergencyRequest.ServiceProviderId == provider.ServiceProviderId)

            ),
            new[] { "Booking", "Order", "EmergencyRequest" }
            );

            var monthlyEarnings = Enumerable.Range(1, 12).Select(month => new MonthlyEarningDto
            {
                MonthNumber = month,
                MonthName = new DateTime(year, month, 1).ToString("MMMM"),
                TotalEarnings = Invoices
                    .Where(i => i.IssueDate.Month == month)
                    .Sum(i => i.TotalAmount)
            }).ToList();
            return monthlyEarnings;

        }
        #endregion


        #region Get TopRequested Services Async
        public async Task<IEnumerable<TopRequestedServiceDto>> GetTopRequestedServicesAsync(int userId, int? count = null)
        {
            int takeCount = count ?? 5;
            // 1. جلب بيانات الورشة
            var provider = await _unitOfWork.ServiceProviders.FindAsync(p => p.AppUserId == userId);
            if (provider == null) throw new Exception("Provider not found.");

            // 2. جلب جميع خدمات هذه الورشة مع الحجوزات المرتبطة بكل خدمة
            var services = await _unitOfWork.Services.FindAllAsync(
                s => s.ServiceProviderId == provider.ServiceProviderId,
                new[] { "BookingServices" } // 👈 مهم جداً عشان نقدر نعد الحجوزات
            );

            // 3. التحويل لـ DTO، العد، الترتيب، واختيار أعلى 5
            var topServices = services
                .Select(s => new TopRequestedServiceDto
                {

                    ServiceName = s.ServiceName,
                    // بنعد الخدمة دي موجودة في كام حجز
                    RequestCount = s.BookingServices?.Count ?? 0
                })
                .Where(s => s.RequestCount > 0) // بنستبعد الخدمات اللي متطلبتش خالص
                .OrderByDescending(s => s.RequestCount) // بنرتب من الأكتر للأقل
                .Take(takeCount) // بناخد أعلى 5 بس
                .ToList();

            return topServices;
        }
        #endregion

        #region GetProviderMonthlyBookings
        public async Task<IEnumerable<MonthlyBookingDto>> GetProviderMonthlyBookingsAsync(int userId, int year)
        {
            // 1. جلب بيانات الورشة
            var provider = await _unitOfWork.ServiceProviders.FindAsync(p => p.AppUserId == userId);
            if (provider == null) throw new Exception("Provider not found.");

            // 2. جلب الحجوزات الخاصة بالورشة في هذه السنة
            var bookings = await _unitOfWork.Bookings.FindAllAsync(
                b => b.ServiceProviderId == provider.ServiceProviderId && b.AppointmentDateTime.Year == year
            );

            // 3. تجميع البيانات لكل شهر (من 1 لـ 12)
            var monthlyBookings = Enumerable.Range(1, 12).Select(month => new MonthlyBookingDto
            {
                Month = month,
                MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month),
                BookingCount = bookings.Count(b => b.AppointmentDateTime.Month == month)
            }).ToList();

            return monthlyBookings;
        }
        #endregion

        #region GetTodayBookingsAsync
        public async Task<IEnumerable<TodayBookingDto>> GetTodayBookingsAsync(int userId)
        {
            // 1. جلب بيانات الورشة
            var provider = await _unitOfWork.ServiceProviders.FindAsync(p => p.AppUserId == userId);
            if (provider == null) throw new Exception("Provider not found.");

            // 2. جلب حجزات النهاردة مع العميل والعربية
            var today = DateTime.Today;
            var bookings = await _unitOfWork.Bookings.FindAllAsync(
                b => b.ServiceProviderId == provider.ServiceProviderId &&
                     b.AppointmentDateTime.Date == today, // فلترة باليوم فقط
                new[] { "Client.AppUser", "Vehicle" } // Include للبيانات المربوطة
            );

            // 3. التحويل لـ DTO
            return bookings.Select(b => new TodayBookingDto
            {
                BookingId = b.BookingId,
                ClientName = b.Client.FullName ?? "Unknown", // اسم العميل
                VehicleInfo = $"{b.Vehicle.Make} {b.Vehicle.Model} ({b.Vehicle.PlateNumber})", // تفاصيل العربية

            }).ToList();
        }
        #endregion



        #region ProviderForSparePartsSummaryAsync
        public async Task<ProviderForSparePartsSummaryDto> ProviderForSparePartsSummaryAsync(int userId)
        {
            // 1. جلب بيانات الـ Provider
            var provider = await _unitOfWork.ServiceProviders.FindAsync(p => p.AppUserId == userId);
            if (provider == null) throw new Exception("Provider not found.");

            var currentDate = DateTime.Now;

            // 2. حساب أرباح "الشهر الحالي" من الفواتير المعتمدة
            var currentMonthInvoices = await _unitOfWork.Invoices.FindAllAsync(
                i => i.IssueDate.Year == currentDate.Year &&
                     i.IssueDate.Month == currentDate.Month &&
                     i.IsDraft == false &&
                     (

                         (i.Order != null && i.Order.ServiceProviderId == provider.ServiceProviderId)

                     ),
                new[] { "Order" }
            );

            decimal currentMonthEarnings = currentMonthInvoices.Sum(i => i.TotalAmount);

            // 3. إحصائيات الطلبات (Orders) باستخدام CountAsync للـ Performance العالي
            int totalOrders = await _unitOfWork.Orders.CountAsync(
                o => o.ServiceProviderId == provider.ServiceProviderId
            );

            int pendingOrders = await _unitOfWork.Orders.CountAsync(
                o => o.ServiceProviderId == provider.ServiceProviderId && o.Status == DAL.Enums.OrderStatus.Pending
            );

            int canceledOrders = await _unitOfWork.Orders.CountAsync(
                o => o.ServiceProviderId == provider.ServiceProviderId && o.Status == DAL.Enums.OrderStatus.Cancelled
            );

            // 4. إرجاع النتيجة للـ Front-end
            return new ProviderForSparePartsSummaryDto
            {
                CurrentMonthEarnings = currentMonthEarnings,
                TotalOrders = totalOrders,
                PendingOrders = pendingOrders,
                CanceledOrders = canceledOrders
            };
        } 
        #endregion

    }
}
