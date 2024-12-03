using LoadVantage.Infrastructure.Data.Contracts;
using Ganss.Xss;

namespace LoadVantage.Infrastructure.Data.Services
{
	public class HtmlSanitizerService : IHtmlSanitizerService
	{
		private readonly HtmlSanitizer _sanitizer;

		public HtmlSanitizerService()
		{
			_sanitizer = new HtmlSanitizer();
		}

		public string Sanitize(string input)
		{
			return _sanitizer.Sanitize(input);
		}
	}
}
