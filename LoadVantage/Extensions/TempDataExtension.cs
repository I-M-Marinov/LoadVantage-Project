using Microsoft.AspNetCore.Mvc.ViewFeatures;

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
				tempData.Add(SuccessMessageKey, message);
            }
        }

        // Get and clear success message
        public static string? GetSuccessMessage(this ITempDataDictionary tempData)
        {
            if (tempData.TryGetValue(SuccessMessageKey, out var message) && message is string strMessage)
            {
                tempData.Remove(SuccessMessageKey);
                return strMessage;
            }

            return null;
        }

        // Set error message
        public static void SetErrorMessage(this ITempDataDictionary tempData, string message)
		{
            if (!string.IsNullOrEmpty(message))
            {
                tempData.Add(ErrorMessageKey, message);
            }
        }

        // Get and clear error message
        public static string? GetErrorMessage(this ITempDataDictionary tempData)
        {
            if (tempData.TryGetValue(ErrorMessageKey, out var message) && message is string strMessage)
            {
                tempData.Remove(ErrorMessageKey);
                return strMessage;
            }
            return null;
        }
    }
}
