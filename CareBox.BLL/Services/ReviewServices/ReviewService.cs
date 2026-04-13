using CareBox.BLL.DTOs.ReviewDto;
using CareBox.BLL.Repositories.Interfaces;
using CareBox.BLL.Services.ReviewServices.Interfaces;
using CareBox.DAL.Enums;
using CareBox.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.Services.ReviewServices
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReviewService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region Add review
        public async Task<bool> AddReviewAsync(int userId, CreateReviewDto model)
        {
            // 1. جلب بيانات العميل
            var client = await _unitOfWork.Clients.FindAsync(c => c.AppUserId == userId);
            if (client == null) throw new Exception("Client not found.");

            // 2. التحقق: هل يوجد حجز واحد على الأقل حالته Completed بين هذا العميل وهذه الورشة؟
            var hasCompletedBooking = await _unitOfWork.Bookings.IsExistAsync(b =>
                b.ClientId == client.ClientID &&
                b.ServiceProviderId == model.ServiceProviderId &&
                b.Status == BookingStatus.Completed);

            if (!hasCompletedBooking)
                throw new Exception("You can only review this provider after completing at least one service with them.");



            // 3. إنشاء التقييم وحفظه
            var review = new Review
            {
                ClientId = client.ClientID,
                ServiceProviderId = model.ServiceProviderId,
                Rating = model.Rating,
                Comment = model.Comment,
                CreatedAt = DateTime.Now

            };

            await _unitOfWork.Reviews.AddAsync(review);
            await _unitOfWork.SaveAsync();

            return true;
        }
        #endregion

        #region Get Client Reviews For ProviderAsync

        public async Task<IEnumerable<ReviewResponseDto>> GetClientReviewsForProviderAsync(int userId, int providerId)
        {
            // 1. جلب بيانات العميل باستخدام الـ UserId (AppUserId)
            var client = await _unitOfWork.Clients.FindAsync(c => c.AppUserId == userId);
            if (client == null) throw new Exception("Client not found.");

            // 2. جلب التقييمات الخاصة بهذا العميل لهذه الورشة مع جلب بيانات الورشة (Include)
            var reviews = await _unitOfWork.Reviews.FindAllAsync(
                r => r.ClientId == client.ClientID && r.ServiceProviderId == providerId,
                new[] { "ServiceProvider" } // Include عشان نجيب اسم الورشة
            );

            // 3. تحويل البيانات إلى DTO
            var response = reviews.Select(r => new ReviewResponseDto
            {
                ReviewId = r.ReviewId,

                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt

            }).OrderByDescending(r => r.CreatedAt).ToList();

            return response;
        }

        #endregion

        #region Get All Client Reviews
        public async Task<IEnumerable<ReviewResponseDto>> GetAllClientReviewsAsync(int userId)
        {
            // 1. جلب بيانات العميل
            var client = await _unitOfWork.Clients.FindAsync(c => c.AppUserId == userId);
            if (client == null) throw new Exception("Client not found.");

            // 2. جلب كل التقييمات الخاصة بهذا العميل لجميع الورش
            var reviews = await _unitOfWork.Reviews.FindAllAsync(
                r => r.ClientId == client.ClientID,
                new[] { "ServiceProvider" } // بنجيب بيانات الورشة عشان نعرض اسمها
            );

            // 3. تحويل البيانات وترتيبها من الأحدث للأقدم
            var response = reviews.Select(r => new ReviewResponseDto
            {
                ReviewId = r.ReviewId,
                ProviderName = r.ServiceProvider.Name, // اسم الورشة اللي اتقيمت
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt

            }).OrderByDescending(r => r.CreatedAt).ToList();

            return response;
        }
        #endregion

        #region Get All Provider Reviews
        public async Task<IEnumerable<ReviewResponseDto>> GetAllProviderReviewsAsync(int providerId)
        {
            var provider = await _unitOfWork.ServiceProviders.FindAsync(c => c.AppUserId == providerId);
            if (provider == null) throw new Exception("provider not found.");

            // 2. جلب كل التقييمات الخاصة بهذا العميل لجميع الورش
            var reviews = await _unitOfWork.Reviews.FindAllAsync(
                r => r.ServiceProviderId == provider.ServiceProviderId,
                new[] { "Client" }
            );

            // 3. تحويل البيانات وترتيبها من الأحدث للأقدم
            var response = reviews.Select(r => new ReviewResponseDto
            {
                ReviewId = r.ReviewId,
                ClientName= r.Client.FullName, // اسم العميل اللي عمل التقييم
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt

            }).OrderByDescending(r => r.CreatedAt).ToList();

            return response;
        }
        #endregion


        #region Get All Provider Reviews for client
        public async Task<IEnumerable<ReviewResponseDto>> GetAllProviderReviewsforClientAsync(int providerId)
        {
            var provider = await _unitOfWork.ServiceProviders.FindAsync(c => c.ServiceProviderId== providerId);
            if (provider == null) throw new Exception("provider not found.");

            // 2. جلب كل التقييمات الخاصة بهذا العميل لجميع الورش
            var reviews = await _unitOfWork.Reviews.FindAllAsync(
                r => r.ServiceProviderId == provider.ServiceProviderId,
                new[] { "Client" }
            );

            // 3. تحويل البيانات وترتيبها من الأحدث للأقدم
            var response = reviews.Select(r => new ReviewResponseDto
            {
                ReviewId = r.ReviewId,
                ClientName = r.Client.FullName, // اسم العميل اللي عمل التقييم
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt

            }).OrderByDescending(r => r.CreatedAt).ToList();

            return response;
        }
        #endregion



        public async Task<bool> UpdateReviewAsync(int userId, int reviewId, UpdateReviewDto model)
        {
            // 1. جلب بيانات العميل
            var client = await _unitOfWork.Clients.FindAsync(c => c.AppUserId == userId);
            if (client == null) throw new Exception("Client not found.");

            // 2. جلب التقييم والتأكد إنه يخص العميل ده
            var review = await _unitOfWork.Reviews.FindAsync(r => r.ReviewId == reviewId && r.ClientId == client.ClientID);
            if (review == null) throw new Exception("Review not found or you don't have permission to update it.");

            // 3. 💡 تطبيق قاعدة الـ 24 ساعة (يوم واحد)
            if ((DateTime.Now - review.CreatedAt).TotalHours > 24)
                throw new Exception("You cannot update a review after 24 hours of adding it.");

            // 4. تحديث البيانات
            review.Rating = model.Rating;
            review.Comment = model.Comment;
            review.CreatedAt = DateTime.Now;

            // (اختياري) ممكن تعمل حقل اسمه UpdatedAt لو حابب تسجل وقت التعديل
            // review.UpdatedAt = DateTime.Now; 

            _unitOfWork.Reviews.Update(review);
            await _unitOfWork.SaveAsync();

            return true;
        }

        public async Task<bool> DeleteReviewAsync(int userId, int reviewId)
        {
            // 1. جلب بيانات العميل
            var client = await _unitOfWork.Clients.FindAsync(c => c.AppUserId == userId);
            if (client == null) throw new Exception("Client not found.");

            // 2. جلب التقييم والتأكد إنه يخص العميل ده
            var review = await _unitOfWork.Reviews.FindAsync(r => r.ReviewId == reviewId && r.ClientId == client.ClientID);
            if (review == null) throw new Exception("Review not found or you don't have permission to delete it.");

            // 3. 💡 تطبيق قاعدة الـ 24 ساعة 
            if ((DateTime.Now - review.CreatedAt).TotalHours > 24)
                throw new Exception("You cannot delete a review after 24 hours of adding it.");

            // 4. الحذف
            _unitOfWork.Reviews.Delete(review);
            await _unitOfWork.SaveAsync();

            return true;
        }
    }
}
