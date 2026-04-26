using CareBox.BLL.DTOs.EmergencyDto;
using CareBox.BLL.Repositories.Interfaces;
using CareBox.BLL.Services.EmergencyService.Interface;
using CareBox.DAL.Enums;
using CareBox.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.Services.EmergencyService
{
    public class EmergencyRequestService : IEmergencyRequestService
    {
        private readonly IUnitOfWork _unitOfWork;

        public EmergencyRequestService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region CreateRequestAsync
        public async Task<EmergencyBroadcastDto> CreateRequestAsync(int userId, CreateEmergencyRequestDto dto)
        {
            // 1. جلب العميل
            var client = await _unitOfWork.Clients.FindAsync(c => c.AppUserId == userId, new[] { "AppUser" });
            if (client == null) throw new Exception("Client not found.");

            // 2. 🛡️ قاعدة الأمان: هل العميل لديه طلب طوارئ نشط؟
            var activeRequest = await _unitOfWork.EmergencyRequests.FindAsync(
                e => e.ClientId == client.ClientID &&
                     e.Status != RequestStatus.Completed &&
                     e.Status != RequestStatus.Cancelled);

            if (activeRequest != null)
                throw new Exception("You already have an emergency live request. You cannot open a new request before the current version of DeVenue..");

            // 3. التأكد من السيارة
            var vehicle = await _unitOfWork.Vehicles.GetByIdAsync(dto.VehicleId);
            if (vehicle == null || vehicle.ClientId != client.ClientID)
                throw new Exception("The car is invalid .");

            // 4. إنشاء النقطة الجغرافية (نظام NTS يطلب خط الطول أولاً ثم العرض X,Y)
            var locationPoint = new Point(dto.Longitude, dto.Latitude) { SRID = 4326 };

            // 5. إنشاء الطلب
            var request = new EmergencyRequest
            {
                ClientId = client.ClientID,
                VehicleId = vehicle.VehicleId,
                RequestType = dto.RequestType,

                RequestLocation = locationPoint,
                ManualAddress = dto.ManualAddress,
                Status = RequestStatus.Pending,
                CreatedAt = DateTime.Now
            };

            await _unitOfWork.EmergencyRequests.AddAsync(request);
            await _unitOfWork.SaveAsync(); // الحفظ لتوليد الـ RequestId

            // 6. إرجاع بيانات البث (الإذاعة)
            return new EmergencyBroadcastDto
            {
                RequestId = request.RequestId,
                CreatedAt = request.CreatedAt
            };
        }
        #endregion
        #region GetTrackingDetailsAsync
        public async Task<EmergencyTrackingResponseDto> GetTrackingDetailsAsync(int userId, long requestId)
        {
            // 1. جلب العميل صاحب الطلب
            var client = await _unitOfWork.Clients.FindAsync(c => c.AppUserId == userId);
            if (client == null) throw new Exception("Client not found.");

            // 2. جلب الطلب مع كل العلاقات اللازمة (الورشة، التقييمات، الفني)
            var request = (await _unitOfWork.EmergencyRequests.FindAllAsync(
                e => e.RequestId == requestId && e.ClientId == client.ClientID,
                new[] { "ServiceProvider.Reviews", "AssignedTechnician" }
            )).FirstOrDefault();

            if (request == null) throw new Exception("Request not found.");

            // 3. التحقق من حالة الطلب
            if (request.Status == RequestStatus.Pending)
                return new EmergencyTrackingResponseDto { Status = request.Status.ToString() };

            // 4. حساب متوسط التقييمات وعددها للورشة
            var reviews = request.ServiceProvider?.Reviews ?? new List<Review>();
            double avgRating = reviews.Any() ? Math.Round(reviews.Average(r => r.Rating), 1) : 0.0;
            int reviewCount = reviews.Count();

            // 5. مابينج للـ DTO
            return new EmergencyTrackingResponseDto
            {
                RequestId = request.RequestId,
                providerId = (int)request.ServiceProviderId,
                Status = request.Status.ToString(),
                ProviderName = request.ServiceProvider?.Name ?? "N/A",
                AverageRating = avgRating,
                TotalReviewsCount = reviewCount,
                TechnicianName = request.AssignedTechnician?.Name ?? "N/A",
                TechnicianPhone = request.AssignedTechnician?.PhoneNumber,
                EstimatedDistance = request.EstimatedDistance,
                EstimatedTimeInMinutes = request.EstimatedTimeInMinutes
            };
        }

        #endregion
        #region GetClientEmergencyRequestsAsync
        public async Task<IEnumerable<ClientEmergencyRequestResponseDto>> GetClientEmergencyRequestsAsync(int userId, string? filter = null)
        {
            // 1. جلب العميل
            var client = await _unitOfWork.Clients.FindAsync(c => c.AppUserId == userId);
            if (client == null) throw new Exception("Client not found.");



            // 3. جلب البيانات مع العلاقات
            var query = await _unitOfWork.EmergencyRequests.FindAllAsync(
                e => e.ClientId == client.ClientID,
                new[] { "Vehicle", "ServiceProvider.AppUser", "AssignedTechnician", "Invoice" }
            );

            var requests = query.AsQueryable();

                // تطبيق الفلترة
                if (filter?.ToLower() == "current")
                {
                    // الطلبات التي لم تنتهِ بعد
                    requests = requests.Where(o => o.Status != RequestStatus.Completed && o.Status != RequestStatus.Cancelled); // <-- تعديل 2
                }
                else if (filter?.ToLower() == "past")
                {
                    // الطلبات المكتملة أو الملغاة
                    requests = requests.Where(o => o.Status == RequestStatus.Completed || o.Status == RequestStatus.Cancelled); // <-- تعديل 2
                }
            

            var requestsList = requests.OrderByDescending(e => e.CreatedAt).ToList();

            // 5. المابينج للـ DTO
            return requestsList.Select(e => new ClientEmergencyRequestResponseDto
            {
                RequestId = e.RequestId,
                providerId = e.ServiceProviderId ?? 0, // <-- تعديل 3
                RequestTypeName = e.RequestType.ToString(),
                VehicleDetails = $"{e.Vehicle.Make} {e.Vehicle.Model}",
                Status = e.Status.ToString(),
                CreatedAt = e.CreatedAt,
                ProviderName = e.ServiceProvider?.Name ?? "Pending Acceptance",
                TechnicianName = e.AssignedTechnician?.Name ?? "Pending Acceptance",
                TechnicianPhone = e.AssignedTechnician?.PhoneNumber ?? "Pending Acceptance"
            }).ToList();
        }
        #endregion

     

        #region UpdateEmergencyStatusAsync
        public async Task<bool> UpdateEmergencyStatusAsync(int userId, UpdateEmergencyStatusDto model)
        {
            // 1. البحث عن طلب الطوارئ (مع جلب الفاتورة والفني المرتبط)
            var query = await _unitOfWork.EmergencyRequests.FindAllAsync(
                e => e.RequestId == model.RequestId,
                new[] { "Invoice", "AssignedTechnician" }
            );

            var request = query.FirstOrDefault();
            if (request == null)
                throw new Exception("Emergency Request not found.");

            // 2. التحقق من صلاحيات المستخدم
            var client = await _unitOfWork.Clients.FindAsync(c => c.AppUserId == userId);
            var provider = await _unitOfWork.ServiceProviders.FindAsync(p => p.AppUserId == userId);

            bool isClient = client?.ClientID == request.ClientId;
            // نتأكد إن الورشة دي هي اللي مسكت الطلب (لو الطلب لسه Pending مش هيكون ليه ProviderId)
            bool isProvider = provider?.ServiceProviderId == request.ServiceProviderId;

            if (!isClient && !isProvider)
                throw new UnauthorizedAccessException("You are not authorized to update this emergency request.");

            // 3. تطبيق قواعد العمل (Business Rules)
            if (request.Status == DAL.Enums.RequestStatus.Completed || request.Status == DAL.Enums.RequestStatus.Cancelled)
                throw new Exception($"Cannot change status of a {request.Status} request.");

            if (isClient && model.NewStatus != DAL.Enums.RequestStatus.Cancelled)
                throw new Exception("Clients are only allowed to cancel requests.");

            // 🛡️ بدء الـ Transaction
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                // 4. تحديث حالة الطلب
                request.Status = model.NewStatus;

                // ----------------------------------------------------
                // 5. اللوجيك المرتبط بكل حالة (الفني + الفواتير)
                // ----------------------------------------------------

                // أ- في حالة الإلغاء (Cancelled)
                if (model.NewStatus == DAL.Enums.RequestStatus.Cancelled)
                {
                    // لو كان فيه فني اتحرك للطلب، نرجعه متاح تاني
                    if (request.AssignedTechnician != null)
                    {
                        request.AssignedTechnician.IsAvailable = true;
                        _unitOfWork.Technicians.Update(request.AssignedTechnician);
                    }
                }

                // ب- في حالة الاكتمال (Completed)
                else if (model.NewStatus == DAL.Enums.RequestStatus.Completed)
                {
                    // 1. تسجيل وقت الانتهاء


                    // 2. تحرير الفني ليكون جاهز لطلبات جديدة
                    if (request.AssignedTechnician != null)
                    {
                        request.AssignedTechnician.IsAvailable = true;
                        _unitOfWork.Technicians.Update(request.AssignedTechnician);
                    }

                    // 3. تقفيل الفاتورة (تحويلها من Draft لنهائية)
                    if (request.Invoice != null)
                    {
                        request.Invoice.IsDraft = false;
                        request.Invoice.IssueDate = DateTime.Now;
                        _unitOfWork.Invoices.Update(request.Invoice);
                    }
                    else
                    {
                        throw new Exception("Cannot complete request without a draft invoice.");
                    }
                }

                // 6. حفظ التعديلات في قاعدة البيانات
                _unitOfWork.EmergencyRequests.Update(request);
                await _unitOfWork.SaveAsync();

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Failed to update status: {ex.Message}");
            }
        }
        #endregion


        #region GetPendingRequestsAsync
        public async Task<IEnumerable<EmergencyBroadcastDto>> GetPendingRequestsAsync()
        {
            var pending = await _unitOfWork.EmergencyRequests.FindAllAsync(
                e => e.Status == DAL.Enums.RequestStatus.Pending,
                new[] { "Client.AppUser", "Vehicle" }
            );

            return pending.Select(e => new EmergencyBroadcastDto
            {
                RequestId = e.RequestId,
                ClientName = e.Client.FullName,
                VehicleDetails = $"{e.Vehicle.Make} {e.Vehicle.Model}",
                LocationURL = $"https://www.google.com/maps/search/?api=1&query={e.RequestLocation.Y},{e.RequestLocation.X}",
                ManualAddress = e.ManualAddress,
                CreatedAt = e.CreatedAt,
                RequestTypeName = e.RequestType.ToString(),

            });
        }
        #endregion
        #region AcceptRequestAsync
        public async Task<bool> AcceptRequestAsync(int userId, AcceptRequestDto dto)
        {
            // 1. جلب مقدم الخدمة (التاجر)
            var provider = await _unitOfWork.ServiceProviders.FindAsync(p => p.AppUserId == userId);
            if (provider == null) throw new Exception("Provider not found.");

            // 2. جلب الطلب مع بيانات العميل
            var request = await _unitOfWork.EmergencyRequests.GetByIdAsync(dto.RequestId);
            if (request == null) throw new Exception("Request not found.");
            if (request.Status != RequestStatus.Pending) throw new Exception("This request is no longer available.");

            // 3. جلب الفني والتأكد من إتاحته
            var technician = await _unitOfWork.Technicians.GetByIdAsync(dto.TechnicianId);
            if (technician == null || technician.ServiceProviderId != provider.ServiceProviderId)
                throw new Exception("Technician not found or doesn't belong to your store.");

            if (!technician.IsAvailable) throw new Exception("Technician is currently busy.");

            // 🛡️ بدء الـ Transaction
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                // 4. حساب المسافة والوقت المتوقع
                // إحداثيات مقدم الخدمة
                var providerLoc = provider.Location; // نقطة جغرافية (geography) من قاعدة البيانات

                // حساب المسافة (بالمتر) - EF Core مع NTS بيحسبها بدقة لو العمود geography
                double distanceInMeters = request.RequestLocation.Distance(providerLoc) * 100000; // تقريب للمسافة الجغرافية
                request.EstimatedDistance = Math.Round(distanceInMeters / 1000, 2); // تحويل لكيلومتر

                // حساب الوقت (بافتراض متوسط سرعة 40 كم/ساعة + 5 دقائق تجهيز)
                request.EstimatedTimeInMinutes = (int)((request.EstimatedDistance / 40) * 60) + 5;

                // 5. تحديث حالة الطلب والفني
                request.ServiceProviderId = provider.ServiceProviderId;
                request.AssignedTechnicianId = technician.TechnicianId;
                request.Status = RequestStatus.Accepted;


                technician.IsAvailable = false; // الفني أصبح مشغولاً ❌

                // 6. إنشاء الفاتورة الـ Draft بصفر 🧾
                var invoice = new Invoice
                {
                    EmergencyRequestId = request.RequestId,
                    IssueDate = DateTime.Now,
                    TotalAmount = 0,
                    IsDraft = true, // مسودة
                    InvoiceDetails = new List<InvoiceDetail>
                    {
                        new InvoiceDetail
                        {
                            ItemDescription = $"Emergency Service: {request.RequestType}",
                            Price = 0
                        }
                    }
                };

                _unitOfWork.EmergencyRequests.Update(request);
                _unitOfWork.Technicians.Update(technician);
                await _unitOfWork.Invoices.AddAsync(invoice);

                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        #endregion


        #region GetProviderEmergencyRequestsAsync
        public async Task<IEnumerable<ProviderEmergencyRequestResponseDto>> GetProviderEmergencyRequestsAsync(int userId)
        {
            // 1. جلب مقدم الخدمة أولاً
            var provider = await _unitOfWork.ServiceProviders.FindAsync(p => p.AppUserId == userId);
            if (provider == null) throw new Exception("Provider not found.");

            // 2. جلب كل الطلبات المرتبطة بهذا الـ Provider
            // نستخدم الـ Include لجلب بيانات (العميل، اليوزر، العربية، والفني)
            var requests = await _unitOfWork.EmergencyRequests.FindAllAsync(
                e => e.ServiceProviderId == provider.ServiceProviderId,
                new[] { "Client.AppUser", "Vehicle", "AssignedTechnician" }
            );

            // 3. المابينج للـ DTO
            return requests.OrderByDescending(e => e.CreatedAt).Select(e => new ProviderEmergencyRequestResponseDto
            {
                RequestId = e.RequestId,
                ClientName = e.Client?.FullName ?? "N/A",
                VehicleDetails = e.Vehicle != null
                    ? $"{e.Vehicle.Make} {e.Vehicle.Model} ({e.Vehicle.Year}) - {e.Vehicle.PlateNumber}"
                    : "N/A",
                RequestType = e.RequestType.ToString(),
                ManualAddress = e.ManualAddress,
                Status = e.Status.ToString(),
                TechnicianName = e.AssignedTechnician?.Name ?? "لم يتم التعيين",
                CreatedAt = e.CreatedAt
            }).ToList();
        }
        #endregion
        #region GetRequestDetailsAsync
        public async Task<EmergencyRequestDetailsDto> GetRequestDetailsAsync(long requestId, int userId)
        {
            var provider = await _unitOfWork.ServiceProviders.FindAsync(p => p.AppUserId == userId);
            if (provider == null) throw new Exception("Provider not found.");

            var query = await _unitOfWork.EmergencyRequests.FindAllAsync(
                e => e.RequestId == requestId,
                new[] {
            "Client.AppUser",
            "Vehicle",
            "AssignedTechnician",
            "ServiceProvider",
            "Invoice"
                }
            );

            var request = query.FirstOrDefault();

            if (request == null) throw new Exception("Request not found.");

            return new EmergencyRequestDetailsDto
            {
                RequestId = request.RequestId,
                Status = request.Status.ToString(),
                RequestType = request.RequestType.ToString(),
                LocationURL= $"https://www.google.com/maps/search/?api=1&query={request.RequestLocation.Y},{request.RequestLocation.X}",
                CreatedAt = request.CreatedAt,

                ManualAddress = request.ManualAddress,
                EstimatedDistance = request.EstimatedDistance,
                EstimatedTimeInMinutes = request.EstimatedTimeInMinutes,

                ClientName = request.Client?.FullName ?? "N/A",
                ClientPhone = request.Client?.AppUser?.PhoneNumber ?? "N/A",


                VehicleDetails = $"{request.Vehicle?.Make} {request.Vehicle?.Model} ({request.Vehicle?.Year})",

                TechnicianId = request.AssignedTechnicianId,
                TechnicianName = request.AssignedTechnician?.Name,
                TechnicianPhone = request.AssignedTechnician?.PhoneNumber,


            };
        } 
        #endregion

    }
}
