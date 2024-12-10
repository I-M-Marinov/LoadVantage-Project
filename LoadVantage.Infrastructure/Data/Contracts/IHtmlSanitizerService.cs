
namespace LoadVantage.Infrastructure.Data.Contracts
{
	public interface IHtmlSanitizerService
	{
		/// <summary>
		/// Receives a string input, sanitizes it and returns it back. Uses the HtmlSanitizer Library.
		/// </summary>
		string Sanitize(string input);
	}
}
