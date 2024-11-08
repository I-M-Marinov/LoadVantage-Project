using static LoadVantage.Common.GeneralConstants.UserRoles;

namespace LoadVantage.Infrastructure.Data.Models
{
	public class Broker : User
	{
        public Broker(string companyName) : base(companyName)
        {
            Position = BrokerPositionName; // Assigns Broker position name
        }
        public Broker()
        {
            Position = BrokerPositionName; // Assigns Broker position name
        }

        public IList<Load> Loads { get; set; } = new List<Load>();
	}
}
