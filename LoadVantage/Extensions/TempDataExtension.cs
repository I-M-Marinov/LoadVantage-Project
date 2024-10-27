using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace LoadVantage.Extensions
{
	public static class TempDataExtension
	{
		private const string TempDataMessageKey = "TempDataMessage";

		public static void SetMessage(this ITempDataDictionary tempData, string message)
		{
			tempData[TempDataMessageKey] = message;
		}

		public static string GetMessage(this ITempDataDictionary tempData)
		{
			if (tempData.TryGetValue(TempDataMessageKey, out var message))
			{
				tempData.Remove(TempDataMessageKey); 
				return message?.ToString();
			}
			return string.Empty;
		}
	}
}
