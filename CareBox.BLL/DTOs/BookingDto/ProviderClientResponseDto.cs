using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.DTOs.BookingDto
{
    public class ProviderClientResponseDto
    {
        public string ClientName { get; set; } = null!;
        public string ClientPhone { get; set; } = null!;
        public string CarMake { get; set; } = null!;
        public string CarModel { get; set; } = null!;
        public int Kilometers { get; set; }
    }
}
