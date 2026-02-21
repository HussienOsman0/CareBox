using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.DTOs.ClientDto.ClientWithproviders
{
    public class ProviderCardDto
    {
        public int ServiceProviderId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string? LogoImageUrl { get; set; }
        public string WorkingHours { get; set; }

        // --- Future Placeholders (For Scalability) ---
        // حالياً سنرسلها null، ومستقبلاً عند تفعيلها لن يتكسر الـ Frontend
        public byte? Rating { get; set; }
        public List<string>? Services { get; set; }
    }
}
