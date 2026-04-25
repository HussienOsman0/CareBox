using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.DTOs.TechnicianDto
{
    public class CreateTechnicianDto
    {
        [Required]
        public string Name { get; set; } = null!;

        [Phone]
        public string? PhoneNumber { get; set; }
    }
}
