using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

using ContactAppNLayer.Api.Controllers;
using ContactAppNLayer.Services.Interfaces;
using ContactAppNLayer.Models.DTOs;

namespace ContactAppNLayer.Api.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mockAuthService = new Mock<IAuthService>();
            _controller = new AuthController(_mockAuthService.Object);
        }

        [Fact]
        public async Task Login_ReturnsOk_WhenCredentialsAreValid()
        {
            // Arrange
            var request = new LoginRequest
            {
                Username = "john",
                Password = "password123"
            };

            var loginResponse = new LoginResponse
            {
                Token = "fake-jwt-token"
            };

            _mockAuthService.Setup(s => s.LoginAsync(request)).ReturnsAsync(loginResponse);

            // Act
            var result = await _controller.Login(request);

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(200);

            dynamic response = okResult.Value;
            ((string)response.message).Should().Be("Login successfully");
            ((string)response.data.Token).Should().Be("fake-jwt-token");
        }

        [Fact]
        public async Task Login_ReturnsUnauthorized_WhenCredentialsInvalid()
        {
            // Arrange
            var request = new LoginRequest
            {
                Username = "wronguser",
                Password = "wrongpass"
            };

            _mockAuthService.Setup(s => s.LoginAsync(request)).ReturnsAsync((LoginResponse?)null);

            // Act
            var result = await _controller.Login(request);

            // Assert
            var unauthorized = result as UnauthorizedObjectResult;
            unauthorized.Should().NotBeNull();
            unauthorized!.StatusCode.Should().Be(401);

            dynamic response = unauthorized.Value;
            ((string)response.message).Should().Be("Invalid Username and password");
        }

        [Fact]
        public async Task Register_ReturnsOk_WhenUserIsNew()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Username = "newuser",
                Password = "pass123",
             //   Role = "User"
            };

            _mockAuthService.Setup(s => s.RegisterAsync(request)).ReturnsAsync(true);

            // Act
            var result = await _controller.Register(request);

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(200);

            dynamic response = okResult.Value;
            ((string)response.message).Should().Be("User registered successfully!");
        }

        [Fact]
        public async Task Register_ReturnsConflict_WhenUserExists()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Username = "existinguser",
                Password = "pass123",
               // Role = "User"
            };

            _mockAuthService.Setup(s => s.RegisterAsync(request)).ReturnsAsync(false);

            // Act
            var result = await _controller.Register(request);

            // Assert
            var conflict = result as ConflictObjectResult;
            conflict.Should().NotBeNull();
            conflict!.StatusCode.Should().Be(409);

            dynamic response = conflict.Value;
            ((string)response.message).Should().Be("Username already exists");
        }
    }
}
