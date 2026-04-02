using CareBox.BLL.DTOs.ReviewDto;
using CareBox.BLL.Services.ReviewServices.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CareBox.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }
        #region Helper
        private int GetUserID()
        {
            var userIdCliam = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdCliam == null)
                throw new Exception("Invalid Token or User found");
            return int.Parse(userIdCliam.Value);
        }
        #endregion


        #region AddReview
        [HttpPost("add-review")]
        [Authorize(Roles = "CLIENT")] // السماح فقط للعملاء بإضافة تقييمات
        public async Task<IActionResult> AddReview([FromBody] CreateReviewDto model)
        {
            try
            {


                int clientId = GetUserID();

                var result = await _reviewService.AddReviewAsync(clientId, model);

                return Ok(new
                {
                    success = true,
                    message = "Thank you! Your review has been submitted successfully."
                });
            }

            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }


        } 
        #endregion

        #region GetMyReviewsForProvider
        [HttpGet("ClientReviews/{providerId}")]
        [Authorize(Roles = "CLIENT")] // العميل فقط هو من يستعرض تقييماته
        public async Task<IActionResult> GetMyReviewsForProvider(int providerId)
        {
            try
            {
                // جلب رقم المستخدم من التوكن
                var userId = GetUserID();

                var reviews = await _reviewService.GetClientReviewsForProviderAsync(userId, providerId);

                return Ok(new
                {
                    success = true,
                    data = reviews
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        } 
        #endregion

        #region GetAllClienReviews
        [HttpGet("ClientReviews")]
        [Authorize(Roles = "CLIENT")] // العميل فقط
        public async Task<IActionResult> GetAllClienReviews()
        {
            try
            {
                // 1. استخراج الـ ID من التوكن
                var userId = GetUserID();

                // 2. استدعاء الخدمة
                var reviews = await _reviewService.GetAllClientReviewsAsync(userId);

                // 3. إرجاع النتيجة
                return Ok(new
                {
                    success = true,
                    data = reviews
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        } 
        #endregion

        #region GetAllProviderReviews
        [HttpGet("ProvidertReviews")]
        [Authorize(Roles = "SERVICEPROVIDER")] // العميل فقط
        public async Task<IActionResult> GetAllProviderReviews()
        {
            try
            {
                // 1. استخراج الـ ID من التوكن
                var userId = GetUserID();

                // 2. استدعاء الخدمة
                var reviews = await _reviewService.GetAllProviderReviewsAsync(userId);

                // 3. إرجاع النتيجة
                return Ok(new
                {
                    success = true,
                    data = reviews
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region UpdateReview
        [HttpPut("UpdateReview/{reviewId}")]
        [Authorize(Roles = "CLIENT")] // العميل فقط
        public async Task<IActionResult> UpdateReview(int reviewId, [FromBody] UpdateReviewDto model)
        {
            try
            {
                var userId = GetUserID();

                

                await _reviewService.UpdateReviewAsync(userId, reviewId, model);

                return Ok(new { success = true, message = "Review updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        } 
        #endregion

        #region DeleteReview
        [HttpDelete("DeleteReview/{reviewId}")]
        [Authorize(Roles = "CLIENT")] // العميل فقط
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            try
            {
                var userId = GetUserID();

                await _reviewService.DeleteReviewAsync(userId, reviewId);

                return Ok(new { success = true, message = "Review deleted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        } 
        #endregion



    }
}
