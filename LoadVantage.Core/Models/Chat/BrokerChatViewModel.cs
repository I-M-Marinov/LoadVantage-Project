﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadVantage.Core.Models.Chat
{
	public class BrokerChatViewModel
	{
		public Guid Id { get; set; }
		public string FullName { get; set; }
		public string ProfilePictureUrl { get; set; }
		public string PhoneNumber { get; set; }
		public string Company { get; set; }
	}
}