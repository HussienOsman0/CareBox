using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.DTOs.ProviderDto.Profile
{
    public class UpdateProviderProfileDto
    {
        [Required]
        public string ShopName { get; set; }
        

        [Required]
        public string Name { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string WorkingHours { get; set; }

        // اختياري: لو بعت صورة جديدة هنحدثها، لو مبعتش هتفضل القديمة
        public IFormFile? NewLogoImage { get; set; }

        // لو عايز يغير موقعه الجغرافي كمان (اختياري)
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

    }
}
