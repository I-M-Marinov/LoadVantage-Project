using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using LoadVantage.Core.Contracts;
using LoadVantage.Core.Hubs;
using LoadVantage.Core.Models.Chat;
using LoadVantage.Extensions;
using LoadVantage.Infrastructure.Data.Models;
using static LoadVantage.Common.GeneralConstants.TempMessages;

namespace LoadVantage.Controllers
{
	[Authorize]
	public class ChatController : Controller
	{
		private readonly IChatService chatService;
		private readonly IUserService userService;


		public ChatController(IChatService _chatService, IUserService _userService)
		{
		    chatService = _chatService;
		    userService = _userService;
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> SendMessage(Guid receiverId, string messageContent)
		{
			var currentUserId = User.GetUserId().Value; // Get the current logged-in user ID

			if (!ModelState.IsValid)
			{
				return RedirectToAction("ChatWindow", new { receiverId });
			}
			
			await chatService.SendMessageAsync(currentUserId, receiverId, messageContent);


			var chatUsers = await chatService.GetChatUsersAsync(currentUserId);
			var messages = await chatService.GetMessagesAsync(currentUserId, receiverId);
			var brokerInfo = await userService.GetChatUserInfoAsync(receiverId);

			var model = new ChatViewModel
			{
				CurrentChatUserId = receiverId,
				Users = chatUsers ?? new List<UserChatViewModel>(),
				Messages = messages.Select(m => new ChatMessageViewModel
				{
					Id = m.Id,
					SenderId = m.SenderId,
					ReceiverId = m.ReceiverId,
					Content = m.Content,
					Timestamp = m.Timestamp,
					IsRead = m.IsRead
					
				}).ToList(),
				BrokerInfo = brokerInfo
			};

			return View("ChatWindow", model);
		}


		[HttpGet]
		public async Task<IActionResult> ChatWithBrokerWindow(Guid brokerId)
		{
			ChatViewModel model = await BuildChatViewModel(brokerId);
			return View("ChatWindow", model);
		}


		[HttpGet]
		public async Task<IActionResult> ChatWindow()
		{
			var currentUserId = User.GetUserId().Value; 

			ChatMessage? lastChat = await chatService.GetLastChatAsync(currentUserId);

			if (lastChat == null)
			{
				ViewData["Message"] = NoRecentChats;
				return View();
			}

			var model = new ChatViewModel();

			if (lastChat.SenderId == currentUserId)
			{
				model = await BuildChatViewModel(lastChat.ReceiverId);

			}
			else if (lastChat.ReceiverId == currentUserId)
			{
				model = await BuildChatViewModel(lastChat.SenderId);

			}

			return View("ChatWindow", model);
		}


		[HttpGet]
		public async Task<IActionResult> GetMessages(Guid chatUserId)
		{
			var currentUserId = User.GetUserId().Value; // Get the current logged-in user ID

			// Fetch messages between the current user and the selected chat user
			var messages = await chatService.GetMessagesAsync(currentUserId, chatUserId);

			// Map messages to the view model
			return PartialView("_ChatMessagesPartialView", messages.Select(m => new ChatMessageViewModel
			{
				Id = m.Id,
				Content = m.Content,
				SenderId = m.SenderId,
				ReceiverId = m.ReceiverId,
				Timestamp = m.Timestamp,
				IsRead = m.IsRead
			}).ToList());
		}



		[HttpGet]
		public async Task<IActionResult> GetUnreadMessages()
		{
			var (unreadMessages, unreadCount) = await chatService.GetUnreadMessagesAsync(User.GetUserId().Value);

			return Json(new { messages = unreadMessages, unreadCount = unreadCount });
		}

		[HttpGet]
		public async Task<IActionResult> GetUserInfo(Guid userId)
		{
			var userInfo = await userService.GetChatUserInfoAsync(userId); // Fetch user details
			return PartialView("_ChatUserInfoPartial", userInfo);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> MarkMessageAsRead(Guid senderId, Guid receiverId)
		{
			await chatService.MarkMessagesAsReadAsync(senderId, receiverId);
			return Ok();
		}

		private async Task<ChatViewModel> BuildChatViewModel(Guid brokerId)
		{
			var currentUserId = User.GetUserId().Value;

			// Fetch chat users, messages, and broker info
			var chatUsers = await chatService.GetChatUsersAsync(currentUserId);
			var userInfo = await userService.GetChatUserInfoAsync(brokerId);
			var messages = await chatService.GetMessagesAsync(currentUserId, brokerId);

			// Build the ChatViewModel
			return new ChatViewModel
			{
				Users = chatUsers,
				CurrentChatUserId = brokerId,
				Messages = messages.Select(m => new ChatMessageViewModel
				{
					Id = m.Id,
					SenderId = m.SenderId,
					ReceiverId = m.ReceiverId,
					Content = m.Content,
					Timestamp = m.Timestamp,
					IsRead = m.IsRead
				}).ToList(),
				BrokerInfo = userInfo
			};
		}
	}
}
