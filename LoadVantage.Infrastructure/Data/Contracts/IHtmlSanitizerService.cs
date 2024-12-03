
namespace LoadVantage.Infrastructure.Data.Contracts
{
	public interface IHtmlSanitizerService
	{
		string Sanitize(string input);
	}
}
