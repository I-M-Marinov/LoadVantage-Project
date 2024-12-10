using LoadVantage.Core.Models.Chat;
using LoadVantage.Infrastructure.Data.Models;

namespace LoadVantage.Core.Contracts
{
	public interface IChatService
	{
		/// <summary>
		/// Retrieve all messages for the user
		/// </summary>
		Task<IEnumerable<ChatMessageViewModel>> GetMessagesAsync(Guid receiverId, Guid senderId);
		/// <summary>
		/// Retrieve the information for the recipient user 
		/// </summary>
		Task<List<UserChatViewModel>> GetChatUsersAsync(Guid currentUserId, bool includeNewChat = false, Guid? newChatUserId = null);
		/// <summary>
		/// Sends a message from the current user to a recipient user 
		/// </summary>
		Task SendMessageAsync(Guid senderId, Guid receiverId, string content);
		/// <summary>
		/// Retrieves a list of all messages and the count of the unread messages 
		/// </summary>
		Task<(List<ChatMessage> Messages, int UnreadCount)> GetUnreadMessagesAsync(Guid userId);
		/// <summary>
		/// Marks messages as "read"
		/// </summary>
		Task MarkMessagesAsReadAsync(Guid senderId, Guid receiverId);
		/// <summary>
		/// Retrieves the last messages that was received for the current user 
		/// </summary>
		Task<ChatMessage?> GetLastChatAsync(Guid userId);
		/// <summary>
		/// Builds all the information visualized in the Chat Window View 
		/// </summary>
		Task<ChatViewModel> BuildChatViewModel(Guid userId);

    }
}
