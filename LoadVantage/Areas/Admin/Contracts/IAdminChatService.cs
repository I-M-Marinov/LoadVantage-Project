using LoadVantage.Areas.Admin.Models.Chat;
using LoadVantage.Core.Models.Chat;
using LoadVantage.Infrastructure.Data.Models;

namespace LoadVantage.Areas.Admin.Contracts
{
	public interface IAdminChatService
	{
		Task<AdminChatViewModel> BuildChatViewModel(Guid userId);
	}
}
