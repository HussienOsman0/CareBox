using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.DTOs.TechnicianDto
{
    public class TechnicianResponseDto
    {
        public int TechnicianId { get; set; }
        public string Name { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public bool IsAvailable { get; set; } // لمعرفة إذا كان الفني متاح لطلبات الطوارئ
    }
}
