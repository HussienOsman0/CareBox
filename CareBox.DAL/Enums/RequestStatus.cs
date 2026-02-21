using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.DAL.Enums
{
    public enum RequestStatus : byte
    {
        Pending = 1, Assigned = 2, Completed = 3
    }
}
