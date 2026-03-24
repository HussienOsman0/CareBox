using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.DTOs.ProviderDto.About
{
    public class UpdateProviderAboutDto
    {
        public string? Description { get; set; }

        // صور جديدة عاوز يرفعها
        public List<IFormFile>? NewImages { get; set; }

        // لو عاوز يحذف صور قديمة، يبعت الـ IDs بتاعتها هنا
        public List<int>? ImagesToDeleteIds { get; set; }
    }
}
