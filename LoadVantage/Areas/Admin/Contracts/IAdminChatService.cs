using LoadVantage.Areas.Admin.Models.Chat;

namespace LoadVantage.Areas.Admin.Contracts
{
	public interface IAdminChatService
	{
		Task<AdminChatViewModel> BuildChatViewModel(Guid userId);
	}
}
