using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.DAL.Enums
{
    public enum BookingStatus : byte
    {
        Pending = 1, Approved = 2, Cancelled = 3, Completed = 4
    }
}
