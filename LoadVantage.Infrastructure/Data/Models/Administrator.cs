using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using static LoadVantage.Common.GeneralConstants.UserRoles;
using static LoadVantage.Common.ValidationConstants.UserValidations;


namespace LoadVantage.Infrastructure.Data.Models
{
    public class Administrator: BaseUser
    {
        public Administrator(string companyName) : base(companyName)
        {
            Position = AdminPositionName; 
        }
        public Administrator()
        {
            Position = AdminPositionName; 
        }

		public override string GetRoleName() => Role.ToString();


	}
}
