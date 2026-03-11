using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.DTOs.ClientDto.VehicleDto
{
    public class CreateOrUpdateVehicleDto
    {
        [Required]
        public string Make { get; set; }
        [Required]
        public string Model { get; set; }
        [Required]
        public short Year { get; set; }
        [Required, MaxLength(20)]
        public string plateNumber { get; set; }

        [Required]
        public int Kilometers { get; set; }

        public IFormFile? CarImage { get; set; }
    }

}
