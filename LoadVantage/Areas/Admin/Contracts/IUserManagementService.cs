using LoadVantage.Areas.Admin.Models.User;

namespace LoadVantage.Areas.Admin.Contracts
{
	public interface IUserManagementService
    {
        Task<List<UserManagementViewModel>> GetUsersAsync(int pageNumber, int pageSize); 
        Task<int> GetTotalUsersCountAsync();
        Task<AdminCreateUserViewModel> CreateAdministratorAsync(AdminCreateUserViewModel model);
		Task<AdminCreateUserViewModel> CreateUserAsync(AdminCreateUserViewModel userViewModel);       
		Task<bool> DeactivateUserAsync(Guid userId);
		Task<bool> ReactivateUserAsync(Guid userId);
		Task<bool> UpdateUserAsync(AdminEditUserViewModel model);
		Task<List<UserManagementViewModel>> SearchUsersAsync(string searchTerm);
        Task<List<string>> GetUserRolesAsync();
	


	}
}
