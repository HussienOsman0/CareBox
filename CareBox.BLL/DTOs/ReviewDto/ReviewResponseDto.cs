using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.DTOs.ReviewDto
{
    public class ReviewResponseDto
    {
        public long ReviewId { get; set; }
       
        public string ProviderName { get; set; } = null!; // عشان نعرض اسم الورشة للعميل
        public string ClientName { get; set; } = null!;
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
