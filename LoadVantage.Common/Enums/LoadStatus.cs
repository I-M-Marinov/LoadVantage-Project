using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadVantage.Common.Enums
{
    public enum LoadStatus
    {
        Cancelled = 0,          // Load is cancelled 
        Created = 1,           // Load is created
        Available = 2,        // Load is posted 
        Booked = 3,          // Load is booked with a dispatcher
        Delivered = 4       // Load has been delivered
    }
}
