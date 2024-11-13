using LoadVantage.Core.Models.Load;

namespace LoadVantage.Hubs
{
	using Microsoft.AspNetCore.SignalR;

	public class LoadHub : Hub
	{
		public async Task SendLoadPostedNotification(LoadViewModel load)
		{
			await Clients.All.SendAsync("ReceiveLoadPostedNotification", load.Id);
		}

		// When any changes Statuses for Loads in the Posted Loads table send a notification to clients to reload it, so they are able to see the actual available ones 
		public async Task SendLoadStatusChangedNotification()
		{
			await Clients.All.SendAsync("ReloadPostedLoadsTable");
		}
	}
}
