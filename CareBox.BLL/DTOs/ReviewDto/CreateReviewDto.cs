using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.DTOs.ReviewDto
{
    public class CreateReviewDto
    {
        [Required(ErrorMessage = "Service Provider ID is required.")]
        public int ServiceProviderId { get; set; }

        [Required(ErrorMessage = "Rating is required.")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")] // التأكد من أن التقييم بين 1 و 5
        public byte Rating { get; set; }

        public string? Comment { get; set; }
    }
}
