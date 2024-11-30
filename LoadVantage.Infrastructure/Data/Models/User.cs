using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static LoadVantage.Common.ValidationConstants.UserValidations;

namespace LoadVantage.Infrastructure.Data.Models
{
	public class User : BaseUser
	{

		public User(string companyName)
            : base(companyName)
        {
            Id = Guid.NewGuid();
        }

        public User()
        {
            Id = Guid.NewGuid();
        }

		public override string GetRoleName() => Role.ToString();
	}
}


