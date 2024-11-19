using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoadVantage.Core.Models.Profile;
using LoadVantage.Infrastructure.Data.Models;

namespace LoadVantage.Core.Models.Chat
{
	public class ChatViewModel
	{
		public List<UserChatViewModel> Users { get; set; } 
		public Guid? CurrentChatUserId { get; set; }
		public List<ChatMessageViewModel> Messages { get; set; }
		public UserChatViewModel BrokerInfo { get; set; }

	}

}
