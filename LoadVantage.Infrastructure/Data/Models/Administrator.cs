using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using static LoadVantage.Common.GeneralConstants.UserRoles;
using static LoadVantage.Common.ValidationConstants;

namespace LoadVantage.Infrastructure.Data.Models
{
    public class Administrator: User
    {
        public override string Position => AdminPositionName; // Sets position to Administrator

    }
}
