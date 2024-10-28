using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadVantage.Common.Enums
{
    public enum LoadStatus
    {
        Available,        // Load is available for booking
        Booked,          // Load is booked with the dispatcher
        Delivered    // Load has been delivered
    }
}
