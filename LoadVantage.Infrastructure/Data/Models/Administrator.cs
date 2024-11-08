using static LoadVantage.Common.GeneralConstants.UserRoles;
using static LoadVantage.Common.ValidationConstants;

namespace LoadVantage.Infrastructure.Data.Models
{
    public class Administrator: User
    {
        public Administrator(string companyName) : base(companyName)
        {
            Position = AdminPositionName; // Assigns Broker position name
        }
        public Administrator()
        {
            Position = AdminPositionName; // Assigns Broker position name
        }

    }
}
