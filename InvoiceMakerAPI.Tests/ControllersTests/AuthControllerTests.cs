using System.Security.Claims;
using InvoiceMakerAPI.Controllers;
using InvoiceMakerAPI.DTOs;
using InvoiceMakerAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace InvoiceMakerAPI.Tests.ControllersTests
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _authServiceMock = new();

        [Fact]
        public async Task Register_ReturnsOk_WhenRegistrationSucceeds()
        {
            var request = new RegisterDTO
            {
                FullName = "Test User",
                Email = "test@example.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!"
            };
            var result = new AuthResult { Succeeded = true, Token = "mock-token" };
            _authServiceMock.Setup(service => service.RegisterAsync(request)).ReturnsAsync(result);

            var response = await CreateController().Register(request);

            var okResult = Assert.IsType<OkObjectResult>(response);
            Assert.Same(result, okResult.Value);
            _authServiceMock.Verify(service => service.RegisterAsync(request), Times.Once);
        }

        [Fact]
        public async Task Register_ReturnsBadRequest_WhenRegistrationFails()
        {
            var request = new RegisterDTO
            {
                FullName = "Test User",
                Email = "test@example.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!"
            };
            var errors = new[] { "Email is already registered" };
            _authServiceMock
                .Setup(service => service.RegisterAsync(request))
                .ReturnsAsync(new AuthResult { Succeeded = false, Errors = errors });

            var response = await CreateController().Register(request);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(response);
            Assert.Equal(errors, Assert.IsAssignableFrom<IEnumerable<string>>(badRequestResult.Value));
            _authServiceMock.Verify(service => service.RegisterAsync(request), Times.Once);
        }

        [Fact]
        public async Task Login_ReturnsOk_WhenCredentialsAreValid()
        {
            var request = new LoginDTO { Email = "test@example.com", Password = "Password123!" };
            var result = new AuthResult { Succeeded = true, Token = "mock-token" };
            _authServiceMock.Setup(service => service.LoginAsync(request)).ReturnsAsync(result);

            var response = await CreateController().Login(request);

            var okResult = Assert.IsType<OkObjectResult>(response);
            Assert.Same(result, okResult.Value);
            _authServiceMock.Verify(service => service.LoginAsync(request), Times.Once);
        }

        [Fact]
        public async Task Login_ReturnsUnauthorized_WhenCredentialsAreInvalid()
        {
            var request = new LoginDTO { Email = "test@example.com", Password = "wrong-password" };
            var errors = new[] { "Invalid credentials" };
            _authServiceMock
                .Setup(service => service.LoginAsync(request))
                .ReturnsAsync(new AuthResult { Succeeded = false, Errors = errors });

            var response = await CreateController().Login(request);

            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(response);
            Assert.Equal(errors, Assert.IsAssignableFrom<IEnumerable<string>>(unauthorizedResult.Value));
            _authServiceMock.Verify(service => service.LoginAsync(request), Times.Once);
        }

        [Fact]
        public async Task ChangePassword_ReturnsUnauthorized_WhenUserIdClaimIsMissing()
        {
            var request = new ChangePasswordDTO
            {
                Email = "test@example.com",
                CurrentPassword = "OldPassword123!",
                NewPassword = "NewPassword123!"
            };

            var response = await CreateController().ChangePassword(request);

            Assert.IsType<UnauthorizedResult>(response);
            _authServiceMock.Verify(
                service => service.ChangePasswordAsync(It.IsAny<string>(), It.IsAny<ChangePasswordDTO>()),
                Times.Never);
        }

        [Fact]
        public async Task ChangePassword_ReturnsBadRequest_WhenServiceReportsFailure()
        {
            const string userId = "user-123";
            var request = new ChangePasswordDTO
            {
                Email = "test@example.com",
                CurrentPassword = "wrong-password",
                NewPassword = "NewPassword123!"
            };
            var errors = new[] { "Current password is incorrect" };
            _authServiceMock
                .Setup(service => service.ChangePasswordAsync(userId, request))
                .ReturnsAsync(new AuthResult { Succeeded = false, Errors = errors });

            var response = await CreateController(userId).ChangePassword(request);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(response);
            Assert.Equal(errors, Assert.IsAssignableFrom<IEnumerable<string>>(badRequestResult.Value));
            _authServiceMock.Verify(service => service.ChangePasswordAsync(userId, request), Times.Once);
        }

        [Fact]
        public async Task ChangePassword_ReturnsSuccessMessage_WhenPasswordIsChanged()
        {
            const string userId = "user-123";
            var request = new ChangePasswordDTO
            {
                Email = "test@example.com",
                CurrentPassword = "OldPassword123!",
                NewPassword = "NewPassword123!"
            };
            _authServiceMock
                .Setup(service => service.ChangePasswordAsync(userId, request))
                .ReturnsAsync(new AuthResult { Succeeded = true });

            var response = await CreateController(userId).ChangePassword(request);

            var okResult = Assert.IsType<OkObjectResult>(response);
            Assert.Equal("Password changed successfully", okResult.Value);
            _authServiceMock.Verify(service => service.ChangePasswordAsync(userId, request), Times.Once);
        }

        private AuthController CreateController(string? userId = null)
        {
            var controller = new AuthController(_authServiceMock.Object);
            var claims = userId is null
                ? Array.Empty<Claim>()
                : new[] { new Claim(ClaimTypes.NameIdentifier, userId) };
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"))
                }
            };

            return controller;
        }
    }
}
