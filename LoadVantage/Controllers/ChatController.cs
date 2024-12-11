using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using LoadVantage.Core.Contracts;
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
		private readonly IProfileService profileService;


		public ChatController(
			IChatService _chatService, 
			IUserService _userService, 
			IProfileService _profileService)
		{
		    chatService = _chatService;
		    userService = _userService;
		    profileService = _profileService;
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
			var profile = await profileService.GetUserInformation(currentUserId);


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
				UserInfo = brokerInfo,
				Profile = profile
			};

			return View("ChatWindow", model);
		}

		[HttpGet]
		public async Task<IActionResult> ChatWithBrokerWindow(Guid brokerId)
		{
			ChatViewModel model = await chatService.BuildChatViewModel(brokerId);
			return View("ChatWindow", model);
		}

		[HttpGet]
		public async Task<IActionResult> ChatWindow()
		{
			var currentUser = await userService.GetCurrentUserAsync();

			ChatMessage? lastChat = await chatService.GetLastChatAsync(currentUser!.Id);


			if (lastChat == null)
			{
				ViewData["Message"] = NoRecentChats;

				var chatModel = new ChatViewModel();
				chatModel.Profile = await profileService!.GetUserInformation(currentUser!.Id);

				return View("ChatWindow", chatModel);
			}

			var model = new ChatViewModel();

			if (lastChat.SenderId == currentUser.Id)
			{
				model = await chatService.BuildChatViewModel(lastChat.ReceiverId);

			}
			else if (lastChat.ReceiverId == currentUser.Id)
			{
				model = await chatService.BuildChatViewModel(lastChat.SenderId);
			}

			return View("ChatWindow", model);
		}

		[HttpGet]
		public async Task<IActionResult> GetMessages(Guid chatUserId)
		{
			var currentUser = await userService.GetCurrentUserAsync();

			// Fetch messages between the current user and the selected chat user
			var messages = await chatService.GetMessagesAsync(currentUser.Id, chatUserId);

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
			var currentUser = await userService.GetCurrentUserAsync();

			var (unreadMessages, unreadCount) = await chatService.GetUnreadMessagesAsync(currentUser.Id);

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

		
	}
}
