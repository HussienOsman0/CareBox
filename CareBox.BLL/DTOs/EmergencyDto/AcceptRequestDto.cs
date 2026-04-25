using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.DTOs.EmergencyDto
{
    public class AcceptRequestDto
    {
        public long RequestId { get; set; }
        public int TechnicianId { get; set; }
    }
}
