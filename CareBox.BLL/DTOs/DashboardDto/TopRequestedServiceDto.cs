using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.DTOs.DashboardDto
{
    public class TopRequestedServiceDto
    {
        public string ServiceName { get; set; } = string.Empty;
        public int RequestCount { get; set; }
    }
}
