using CareBox.DAL.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.DTOs.EmergencyDto
{
    public class CreateEmergencyRequestDto
    {
        [Required]
        public int VehicleId { get; set; }

        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }

        public string? ManualAddress { get; set; }

        [Required]
        public EmergencyRequestType RequestType { get; set; }

    }
}
