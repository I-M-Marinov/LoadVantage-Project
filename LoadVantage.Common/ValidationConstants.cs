﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadVantage.Common
{
	public static class ValidationConstants
	{
		// USER

		public const byte UserNameMinLength = 4;
		public const byte UserNameMaxLength = 100;

		public const byte EmailMinLength = 7;
		public const byte EmailMaxLength = 50;

		public const byte FirstNameMinLength = 5;
		public const byte FirstNameMaxLength = 25;

		public const byte LastNameMinLength = 3;
		public const byte LastNameMaxLength = 25;

		// DISPATCHER 

		public const byte DispatcherCompanyMinLength = 3;
		public const byte DispatcherCompanyMaxLength = 50;

		// BROKER

		public const byte BrokerCompanyMinLength = 3;
		public const byte BrokerCompanyMaxLength = 50;

		// LOAD

		public const byte LoadOriginCityMinLength = 2;
		public const byte LoadOriginCityMaxLength = 30;

		public const byte LoadDestinationCityMinLength = 2;
		public const byte LoadDestinationCityMaxLength = 30;

		public const byte StateMinMaxLength = 2;

		public const decimal LoadPostedPriceMinValue = 1.00m;
		public const decimal LoadPostedPriceMaxValue = 99999.99m;

		// DRIVER 


		public const byte LicenseNumberMinLength = 5;
		public const byte LicenseNumberMaxLength = 15;



	}
}
