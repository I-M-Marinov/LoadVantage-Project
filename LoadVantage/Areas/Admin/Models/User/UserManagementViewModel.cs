﻿using LoadVantage.Areas.Admin.Models.Profile;
using System.ComponentModel.DataAnnotations;

using static LoadVantage.Common.GeneralConstants.UserImage;
using static LoadVantage.Common.ValidationConstants.UserValidations;
using static LoadVantage.Common.ValidationConstants.EditProfile;

namespace LoadVantage.Areas.Admin.Models.User
{
	public class UserManagementViewModel
	{
		public string Id { get; set; } = null!;
		
		public  string Role { get; set; } = null!;

		public  string UserName { get; set; } = null!;

		public  string Email { get; set; } = null!;

		public  string FirstName { get; set; } = null!;

		public  string LastName { get; set; } = null!;

		public  string CompanyName { get; set; } = null!;

		public  string Position { get; set; } = null!;

		public string? Password {get; set; }

		public string PhoneNumber { get; set; } = string.Empty;
		public string? UserImageUrl { get; set; } 
		public string FullName => $"{FirstName} {LastName}";
	}
}