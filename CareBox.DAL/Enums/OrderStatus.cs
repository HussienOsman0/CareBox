using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.DAL.Enums
{
    public enum OrderStatus : byte
    {
        Pending = 1, Paid = 2, Shipped = 3, Delivered = 4
    }
}
