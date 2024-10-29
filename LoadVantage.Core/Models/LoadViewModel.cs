using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadVantage.Core.Models
{
    public class LoadViewModel
    {
        public Guid Id { get; set; }
        public string OriginCity { get; set; } = null!;
        public string OriginState { get; set; } = null!;
        public string DestinationCity { get; set; } = null!;
        public string DestinationState { get; set; } = null!;      
        public DateTime PickupTime { get; set; }
        public DateTime DeliveryTime { get; set; }
        public decimal PostedPrice { get; set; }
        public double Weight { get; set; }
        public string Status { get; set; } = null!;
        public Guid BrokerId { get; set; }
        public Guid? DispatcherId { get; set; }
    }
}
