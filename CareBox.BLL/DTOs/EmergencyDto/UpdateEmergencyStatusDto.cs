using CareBox.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.DTOs.EmergencyDto
{
    public class UpdateEmergencyStatusDto
    {
        public long RequestId { get; set; }
        public RequestStatus NewStatus { get; set; }
    }
}
