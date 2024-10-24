using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadVantage.Common.Enums
{
    public enum LoadStatus
    {
        Available,    // Load is available for booking
        InTransit,    // Load is currently in transit
        Delivered,    // Load has been delivered
        Billed        // Load has been billed
    }
}
