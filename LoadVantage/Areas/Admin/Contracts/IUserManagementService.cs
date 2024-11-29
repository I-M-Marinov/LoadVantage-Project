using LoadVantage.Areas.Admin.Models.User;

namespace LoadVantage.Areas.Admin.Contracts
{
	public interface IUserManagementService
    {
        Task<List<UserManagementViewModel>> GetUsersAsync(int pageNumber, int pageSize); 
        Task<int> GetTotalUsersCountAsync();
        Task<AdminCreateUserViewModel> CreateUserAsync(AdminCreateUserViewModel userViewModel);       
		Task<AdminCreateUserViewModel> UpdateUserAsync(Guid userId, AdminCreateUserViewModel updatedUserViewModel);
		Task<bool> DeleteUserAsync(Guid userId);
        Task<List<UserManagementViewModel>> SearchUsersAsync(string searchTerm);
        Task<List<string>> GetUserRolesAsync();
		Task<bool> AssignRoleToUserAsync(Guid userId, string roleName);
		Task<bool> ChangeUserRoleAsync (Guid userId, string roleName); 
		Task<bool> LockUserAccountAsync(Guid userId); 
		Task<bool> UnlockUserAccountAsync(Guid userId);


	}
}
