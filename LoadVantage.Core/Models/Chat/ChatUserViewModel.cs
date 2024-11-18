using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadVantage.Core.Models.Chat
{
	public class ChatUserViewModel
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string ProfilePictureUrl { get; set; }
	}
}