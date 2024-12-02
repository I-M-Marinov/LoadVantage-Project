using Microsoft.AspNetCore.Mvc;

namespace LoadVantage.Controllers
{
	public class ErrorController : Controller
	{
		// Default error handler
		public IActionResult Error()
		{
			return View();
		}

		// Specific error handler for status codes
		public IActionResult StatusCode(int statusCode)
		{
			switch (statusCode)
			{
				case 404:
					return View("404"); // Views/Error/404.cshtml
				case 500:
					return View("500"); // Views/Error/500.cshtml
				default:
					return View("Error");
			}
		}

		public IActionResult Test404()
		{
			return View("404"); // This will show the 404 page
		}

		public IActionResult Test500()
		{
			return View("500"); // This will show the 500 page
		}
	}

}
