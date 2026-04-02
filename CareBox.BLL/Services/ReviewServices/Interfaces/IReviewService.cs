using CareBox.BLL.DTOs.ReviewDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.Services.ReviewServices.Interfaces
{
    public interface IReviewService
    {
        Task<bool> AddReviewAsync(int clientId, CreateReviewDto model);
        Task<IEnumerable<ReviewResponseDto>> GetClientReviewsForProviderAsync(int clientId, int providerId);
        Task<IEnumerable<ReviewResponseDto>> GetAllClientReviewsAsync(int userId);
        Task<IEnumerable<ReviewResponseDto>> GetAllProviderReviewsAsync(int providerId);

        Task<bool> UpdateReviewAsync(int userId, int reviewId, UpdateReviewDto model);
        Task<bool> DeleteReviewAsync(int userId, int reviewId);
    }
}
