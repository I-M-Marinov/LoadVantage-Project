using LoadVantage.Core.Contracts;
using LoadVantage.Core.Hubs;
using LoadVantage.Core.Services;
using LoadVantage.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using LoadVantage.Core.Models.Chat;
using LoadVantage.Extensions;
using LoadVantage.Infrastructure.Data;

namespace LoadVantage.Controllers
{
	[Authorize]
	public class ChatController : Controller
	{
		private readonly IChatService chatService;
		private readonly IHubContext<ChatHub> chatHub;
		private readonly IUserService userService;
		private readonly LoadVantageDbContext context;


		public ChatController(IChatService _chatService, IHubContext<ChatHub> _chatHubContext, IUserService _userService, LoadVantageDbContext _context)
		{
		    chatService = _chatService;
		    chatHub = _chatHubContext;
		    userService = _userService;
		    context = _context;
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
			var brokerInfo = await userService.GetChatBrokerInfoAsync(receiverId);

			var model = new ChatViewModel
			{
				CurrentChatUserId = receiverId,
				Users = chatUsers,
				Messages = messages.Select(m => new ChatMessageViewModel
				{
					SenderId = m.SenderId,
					ReceiverId = m.ReceiverId,
					Content = m.Content,
					Timestamp = m.Timestamp
				}).ToList(),
				BrokerInfo = brokerInfo
			};

			return View("ChatWindow", model); // Return the updated chat view
		}



		[HttpGet]
		public async Task<IActionResult> ChatWindow(Guid brokerId)
		{
			var currentUserId = User.GetUserId().Value;

			var chatUsers = await chatService.GetChatUsersAsync(currentUserId);

			var brokerInfo = await userService.GetChatBrokerInfoAsync(brokerId);
			var messages = await chatService.GetMessagesAsync(currentUserId, brokerId);


			var model = new ChatViewModel
			{
				Users = chatUsers, 
				CurrentChatUserId = null,
				Messages = messages.Select(m => new ChatMessageViewModel
				{
					Content = m.Content,
					SenderId = m.SenderId,
					ReceiverId = m.ReceiverId,
					Timestamp = m.Timestamp,
					IsRead = m.IsRead
				}).ToList(),
				BrokerInfo = brokerInfo 
			};

			return View(model);
		}


		[HttpGet]
		public async Task<IActionResult> GetMessages(Guid chatUserId)
		{
			var currentUserId = User.GetUserId().Value;

			// Fetch messages between the current user and the selected chat user
			var messages = await chatService.GetMessagesAsync(currentUserId, chatUserId);

			// Map messages to the view model
			var messageViewModels = messages.Select(m => new ChatMessageViewModel
			{
				SenderId = m.SenderId,
				ReceiverId = m.ReceiverId,
				Content = m.Content,
				Timestamp = m.Timestamp
			}).ToList();

			return PartialView("_ChatMessagesPartialView", messageViewModels);
		}

	}
}
