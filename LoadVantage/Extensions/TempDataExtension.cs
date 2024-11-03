using Microsoft.AspNetCore.Mvc.ViewFeatures;

#nullable disable

namespace LoadVantage.Extensions
{
	public static class TempDataExtension
	{
		private const string SuccessMessageKey = "SuccessMessage";
		private const string ErrorMessageKey = "ErrorMessage";

		// Set success message
		public static void SetSuccessMessage(this ITempDataDictionary tempData, string message)
		{
            if (!string.IsNullOrEmpty(message))
            {
                tempData[SuccessMessageKey] = message;
            }
        }

		// Get and clear success message
		public static string GetSuccessMessage(this ITempDataDictionary tempData)
		{
			if (tempData.TryGetValue(SuccessMessageKey, out var message))
			{
				tempData.Remove(SuccessMessageKey);
				return message?.ToString() ?? string.Empty;
			}
			return string.Empty;
		}

		// Set error message
		public static void SetErrorMessage(this ITempDataDictionary tempData, string message)
		{
            if (!string.IsNullOrEmpty(message))
            {
                tempData[ErrorMessageKey] = message;
            }
        }

		// Get and clear error message
		public static string GetErrorMessage(this ITempDataDictionary tempData)
		{
			if (tempData.TryGetValue(ErrorMessageKey, out var message))
			{
				tempData.Remove(ErrorMessageKey);
				return message?.ToString() ?? string.Empty;
			}
			return string.Empty;
		}
	}
}
