using System.ComponentModel.DataAnnotations;
using static LoadVantage.Common.GeneralConstants.UserRoles;
using static LoadVantage.Common.ValidationConstants;

namespace LoadVantage.Infrastructure.Data.Models
{
	public class Broker : User
	{
        public override string Position => BrokerPositionName; // Sets role to Broker

		public ICollection<Load> Loads { get; set; } = new List<Load>();
	}
}
