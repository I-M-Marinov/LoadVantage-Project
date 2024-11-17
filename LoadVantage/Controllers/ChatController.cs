using LoadVantage.Core.Contracts;
using LoadVantage.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LoadVantage.Controllers
{
	[Authorize]
	public class ChatController : ControllerBase
	{
		private readonly IChatService chatService;

	    public ChatController(IChatService _chatService)
	    {
		    chatService = _chatService;
	    }


		[HttpPost]
		public async Task<IActionResult> SendMessage(string receiverId, string message)
		{
			if (string.IsNullOrWhiteSpace(message))
			{
				return BadRequest("Message cannot be empty.");
			}

			try
			{
				await chatService.SendMessageAsync(User.Identity.Name, receiverId, message);
				return Ok();
			}
			catch (Exception ex)
			{
				// Log the exception here
				return StatusCode(500, "Internal server error: " + ex.Message);
			}
		}

	}
}
