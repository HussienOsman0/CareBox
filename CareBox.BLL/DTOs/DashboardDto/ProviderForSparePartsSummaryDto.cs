using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.DTOs.DashboardDto
{
    public class ProviderForSparePartsSummaryDto
    {
        public decimal CurrentMonthEarnings { get; set; }
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public int CanceledOrders { get; set; }
    }
}
