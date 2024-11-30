using LoadVantage.Areas.Admin.Models.Profile;
using LoadVantage.Areas.Admin.Services;

namespace LoadVantage.Areas.Admin.Models.User
{
	public class UsersListModel
	{
		public IEnumerable<UserManagementViewModel> Users { get; set; } = new HashSet<UserManagementViewModel>();
		public AdminCreateUserViewModel NewUser { get; set; } = null!;
		public AdminEditUserViewModel EditedUser { get; set; } = null!;
		public AdminProfileViewModel AdminProfile { get; set; } = null!;
		public string SearchTerm { get; set; } = null!;
		public int CurrentPage { get; set; }
		public int PageSize { get; set; }
		public int TotalUsers { get; set; } 

	}
}
