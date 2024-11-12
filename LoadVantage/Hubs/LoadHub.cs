namespace LoadVantage.Hubs
{
	using Microsoft.AspNetCore.SignalR;

	public class LoadHub : Hub
	{
		public async Task SendLoadPostedNotification()
		{
			await Clients.All.SendAsync("ReceiveLoadPostedNotification");
		}

		public async Task SendLoadStatusChangedNotification()
		{
			await Clients.All.SendAsync("ReceiveLoadUnpostedNotification");
		}
	}
}
