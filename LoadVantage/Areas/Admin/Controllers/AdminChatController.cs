using LoadVantage.Areas.Admin.Contracts;
using LoadVantage.Areas.Admin.Models.AdminChat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using LoadVantage.Core.Contracts;
using LoadVantage.Core.Models.Chat;
using LoadVantage.Extensions;
using LoadVantage.Filters;
using LoadVantage.Infrastructure.Data.Models;
using static LoadVantage.Common.GeneralConstants.TempMessages;

namespace LoadVantage.Areas.Admin.Controllers
{
    [Authorize]
    [AdminOnly]
    [Area("Admin")]

    public class AdminChatController : Controller
    {

        private readonly IAdminChatService adminChatService;
        private readonly IChatService chatService;
        private readonly IUserService userService;
        private readonly IAdminProfileService adminProfileService;


        public AdminChatController(IAdminChatService _adminChatService, IChatService _chatService, IUserService _userService, IAdminProfileService _adminProfileService)
        {
	        adminChatService = _adminChatService;
            chatService = _chatService;
            userService = _userService;
            adminProfileService = _adminProfileService;
        }

        [HttpGet]
        public async Task<IActionResult> AdminChatWindow()
        {
            var currentUser = await userService.GetCurrentUserAsync();

            ChatMessage? lastChat = await chatService.GetLastChatAsync(currentUser!.Id);


            if (lastChat == null)
            {
                ViewData["Message"] = NoRecentChats;

                var chatModel = new AdminChatViewModel();
                chatModel.Profile = await adminProfileService.GetAdminInformation(currentUser!.Id);

                return View("~/Areas/Admin/Views/Admin/Chat/AdminChatWindow.cshtml", chatModel);
            }

            var model = new AdminChatViewModel();

            if (lastChat.SenderId == currentUser.Id)
            {
                model = await adminChatService.BuildChatViewModel(lastChat.ReceiverId);

            }
            else if (lastChat.ReceiverId == currentUser.Id)
            {
                model = await adminChatService.BuildChatViewModel(lastChat.SenderId);
            }

            return View("~/Areas/Admin/Views/Admin/Chat/AdminChatWindow.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMessage(Guid receiverId, string messageContent)
        {
	        var currentUserId = User.GetUserId().Value; // Get the current logged-in user ID

	        if (!ModelState.IsValid)
	        {
		        return RedirectToAction("AdminChatWindow", new { receiverId });
	        }


	        await chatService.SendMessageAsync(currentUserId, receiverId, messageContent);

	        var chatUsers = await chatService.GetChatUsersAsync(currentUserId);
	        var messages = await chatService.GetMessagesAsync(currentUserId, receiverId);
	        var userInfo = await userService.GetChatUserInfoAsync(receiverId);
	        var profile = await adminProfileService.GetAdminInformation(currentUserId);


	        var model = new AdminChatViewModel()
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
		        UserInfo = userInfo,
		        Profile = profile
	        };

	        return View("~/Areas/Admin/Views/Admin/Chat/AdminChatWindow.cshtml", model);
        }

		[HttpGet]
		public async Task<IActionResult> StartChatWithUser(Guid userId)
		{
			AdminChatViewModel model = await adminChatService.BuildChatViewModel(userId);

			return View("~/Areas/Admin/Views/Admin/Chat/AdminChatWindow.cshtml", model);
		}

		[HttpGet]
		public async Task<IActionResult> GetMessages(Guid chatUserId)
		{
			var currentUser = await userService.GetCurrentUserAsync();

			// Fetch messages between the current user and the selected chat user
			var messages = await chatService.GetMessagesAsync(currentUser.Id, chatUserId);

			// Map messages to the view model
			return PartialView("~/Areas/Admin/Views/Admin/Chat/_ChatMessagesPartialView.cshtml", messages.Select(m => new ChatMessageViewModel
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

			var (unreadMessages, unreadCount) = await chatService.GetUnreadMessagesAsync(currentUser!.Id);

			return Json(new { messages = unreadMessages, unreadCount = unreadCount });
		}

		[HttpGet]
		public async Task<IActionResult> GetUserInfo(Guid userId)
		{
			var userInfo = await userService.GetChatUserInfoAsync(userId); // Fetch user details
			return PartialView("~/Areas/Admin/Views/Admin/Chat/_ChatUserInfoPartial.cshtml", userInfo);
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
