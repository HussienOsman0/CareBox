using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.DTOs.ProviderDto.About
{
    public class ProviderAboutDto
    {
        public int ServiceProviderId { get; set; }
        public string? Description { get; set; }
        public List<ProviderImageDto> Images { get; set; } = new List<ProviderImageDto>();
    }
}
