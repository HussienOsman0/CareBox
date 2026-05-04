using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.DTOs.EmergencyDto
{
    public class AcceptRequestDto
    {
        [Required]
        public long RequestId { get; set; }
        [Required]
        public int TechnicianId { get; set; }
    }
}
