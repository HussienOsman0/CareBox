using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.DTOs.ClientDto.VehicleDto
{
    public class VehicleDto
    {
        public int VehicleId { get; set; }
        public string? CarImage { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public short Year { get; set; }
        public string plateNumber { get; set; }
        public int Kilometers { get; set; }


    }
}
