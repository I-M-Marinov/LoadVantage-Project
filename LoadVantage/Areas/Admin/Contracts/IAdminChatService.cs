using LoadVantage.Areas.Admin.Models.AdminChat;

namespace LoadVantage.Areas.Admin.Contracts
{
	public interface IAdminChatService
	{
		Task<AdminChatViewModel> BuildChatViewModel(Guid userId);
	}
}
