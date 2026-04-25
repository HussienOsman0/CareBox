using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.DTOs.EmergencyDto
{
    public class EmergencyTrackingResponseDto
    {
        public long RequestId { get; set; }
        public int providerId{ get; set; }
        public string Status { get; set; } = null!;

        // بيانات مقدم الخدمة (الورشة)
        public string ProviderName { get; set; } = null!;
        public double AverageRating { get; set; }
        public int TotalReviewsCount { get; set; }

        // بيانات الفني
        public string TechnicianName { get; set; } = null!;
        public string? TechnicianPhone { get; set; }

        // بيانات الوصول
        public double? EstimatedDistance { get; set; } // بالكيلومتر
        public int? EstimatedTimeInMinutes { get; set; } // بالدقائق
    }
}
