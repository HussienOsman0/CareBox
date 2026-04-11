using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.DTOs.ProviderDto.Services
{
    public class CreateServiceDto
    {
        [Required]
        public string ServiceName { get; set; }
        public string? Description { get; set; }
        [Required]
        [Range(0,double.MaxValue,ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        // ضيف البروبيرتي دي
        public string? CategoryName { get; set; }
    }
}
