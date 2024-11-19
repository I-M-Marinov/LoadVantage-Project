using LoadVantage.Core.Models.Chat;
using LoadVantage.Infrastructure.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadVantage.Core.Contracts
{
	public interface IChatService
	{
		Task<IEnumerable<ChatMessageViewModel>> GetMessagesAsync(Guid receiverId, Guid senderId);
		Task<List<UserChatViewModel>> GetChatUsersAsync(Guid currentUserId, bool includeNewChat = false, Guid? newChatUserId = null);
		Task SendMessageAsync(Guid senderId, Guid receiverId, string content);
		Task<(List<ChatMessage> Messages, int UnreadCount)> GetUnreadMessagesAsync(Guid userId);
		Task MarkMessagesAsReadAsync(Guid senderId, Guid receiverId);
	}
}
