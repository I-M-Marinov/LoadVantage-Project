using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

using Moq;
using NUnit.Framework;

using LoadVantage.Controllers;
using LoadVantage.Core.Contracts;
using LoadVantage.Core.Models.Account;


using static LoadVantage.Common.GeneralConstants.TempMessages;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace LoadVantage.Tests.IntegrationTests.Web
{
	public class AccountControllerTests
	{
		private Mock<IUserService> _mockUserService;
		private Mock<IAccountService> _mockAccountService;
		private AccountController _controller;

		[SetUp]
		public void SetUp()
		{
			_mockUserService = new Mock<IUserService>();
			_mockAccountService = new Mock<IAccountService>();

			_controller = new AccountController(
				_mockUserService.Object, 
				_mockAccountService.Object);
		}

		[Test]
		public void Register_ReturnsViewWithRegisterViewModel()
		{
			var result = _controller.Register();

			Assert.That(result, Is.TypeOf<ViewResult>());
			var viewResult = result as ViewResult;
			Assert.That(viewResult?.Model, Is.TypeOf<RegisterViewModel>());
		}

		[Test]
		public async Task Register_Post_ReturnsViewWithModel_WhenModelStateIsInvalid()
		{
			var model = new RegisterViewModel(); 
			_controller.ModelState.AddModelError("Error", "Test error"); 

			var result = await _controller.Register(model);

			Assert.That(result, Is.TypeOf<ViewResult>());
			var viewResult = result as ViewResult;
			Assert.That(viewResult?.Model, Is.EqualTo(model));
		}

		[Test]
		public async Task Register_Post_RedirectsToLogin_WhenRegistrationSucceeds()
		{
			var model = new RegisterViewModel();
			var registrationResult = IdentityResult.Success;

			_mockAccountService
				.Setup(s => s.RegisterUserAsync(model))
				.ReturnsAsync((registrationResult, null));

			var tempDataDictionary = new Mock<ITempDataDictionary>();
			tempDataDictionary.Setup(t => t.Peek(It.IsAny<string>())).Returns(LoginWithNewAccount);

			var _controller = new AccountController(_mockUserService.Object, _mockAccountService.Object)
			{
				TempData = tempDataDictionary.Object 
			};

			var result = await _controller.Register(model);

			Assert.That(result, Is.TypeOf<RedirectToActionResult>());
			var redirectResult = result as RedirectToActionResult;

			Assert.That(redirectResult?.ActionName, Is.EqualTo("Login"));

			Assert.That(tempDataDictionary.Object.Peek("SuccessMessage"), Is.EqualTo(LoginWithNewAccount));
		}

		[Test]
		public async Task Register_Post_ReturnsViewWithModel_WhenRegistrationFails_WithErrorKey()
		{
			var model = new RegisterViewModel();
			var registrationResult = IdentityResult.Failed(new IdentityError { Description = "Test error" });

			_mockAccountService
				.Setup(s => s.RegisterUserAsync(model))
				.ReturnsAsync((registrationResult, "TestErrorKey")); 

			var result = await _controller.Register(model);

			Assert.That(result, Is.TypeOf<ViewResult>());
			var viewResult = result as ViewResult;
			Assert.That(viewResult?.Model, Is.EqualTo(model));

			Assert.That(_controller.ModelState.ContainsKey("TestErrorKey"), Is.True);
			Assert.That(_controller.ModelState["TestErrorKey"]?.Errors[0]?.ErrorMessage, Is.EqualTo("Test error"));
		}

		[Test]
		public async Task Register_Post_ReturnsViewWithModel_WhenRegistrationFails_WithoutErrorKey()
		{
			var model = new RegisterViewModel();

			var registrationResult = IdentityResult.Failed(
				new IdentityError { Description = "Error 1" },
				new IdentityError { Description = "Error 2" }
			);

			_mockAccountService.Setup(s => s.RegisterUserAsync(model))
				.ReturnsAsync((registrationResult, null));

			var result = await _controller.Register(model);

			Assert.That(result, Is.TypeOf<ViewResult>());
			var viewResult = result as ViewResult;
			Assert.That(viewResult?.Model, Is.EqualTo(model));

			Assert.That(_controller.ModelState[string.Empty]?.Errors.Count, Is.EqualTo(2));
			Assert.That(_controller.ModelState[string.Empty]?.Errors[0]?.ErrorMessage, Is.EqualTo("Error 1"));
			Assert.That(_controller.ModelState[string.Empty]?.Errors[1]?.ErrorMessage, Is.EqualTo("Error 2"));
		}

		[Test]
		public void Login_Get_ReturnsLoginViewWithModel()
		{

			var result = _controller.Login() as ViewResult;

			Assert.That(result, Is.Not.Null); 
			Assert.That(result?.Model, Is.InstanceOf<LoginViewModel>()); 
		}

		[Test]
		public async Task Login_Post_InvalidModelState_ReturnsViewWithModel()
		{
			var model = new LoginViewModel(); 

			_controller.ModelState.AddModelError("UserName", "Username is required"); 

			var result = await _controller.Login(model) as ViewResult;  

			Assert.That(result, Is.Not.Null); 
			Assert.That(result?.Model, Is.EqualTo(model));
		}

		[Test]
		public async Task Login_Post_SuccessfulLogin_RedirectsToProfile()
		{
			var model = new LoginViewModel { UserName = "user", Password = "password" };
			var signInResult = SignInResult.Success;

			_mockAccountService.Setup(service => service.LoginAsync(model.UserName, model.Password))
				.ReturnsAsync((signInResult, null));

			 
			var result = await _controller.Login(model) as RedirectToActionResult;

			Assert.That(result, Is.Not.Null); 
			Assert.That(result?.ActionName, Is.EqualTo("Index")); 
		}

		[Test]
		public async Task Login_Post_FailedLogin_ReturnsViewWithErrorMessage()
		{
			var model = new LoginViewModel { UserName = "user", Password = "wrongpassword" };
			var errorMessage = "Invalid username or password";

			_mockAccountService.Setup(service => service.LoginAsync(model.UserName, model.Password))
				.ReturnsAsync((SignInResult.Failed, errorMessage));

			var result = await _controller.Login(model) as ViewResult;

			Assert.That(result, Is.Not.Null); 
			Assert.That(result?.Model, Is.EqualTo(model));
			Assert.That(_controller.ModelState.ErrorCount, Is.EqualTo(1)); 
			Assert.That(_controller.ModelState[""].Errors[0].ErrorMessage, Is.EqualTo(errorMessage)); 
		}

		[Test]
		public async Task Logout_UserLogsOut_RedirectsToLoginWithSuccessMessage()
		{

			var mockAccountService = new Mock<IAccountService>();
			mockAccountService.Setup(service => service.LogOutAsync()).Returns(Task.CompletedTask);
			var mockTempData = new Mock<ITempDataDictionary>();

			var _controller = new AccountController(_mockUserService.Object, mockAccountService.Object)
			{
				TempData = mockTempData.Object
			};

			var result = await _controller.Logout() as RedirectToActionResult;

			Assert.That(result, Is.Not.Null); 
			Assert.That(result?.ActionName, Is.EqualTo("Login")); 
			Assert.That(result?.ControllerName, Is.EqualTo("Account")); 

			mockAccountService.Verify(service => service.LogOutAsync(), Times.Once);

		}


	}
}
