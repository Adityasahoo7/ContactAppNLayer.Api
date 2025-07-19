using Xunit;
using Moq;
using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using ContactAppNLayer.Services.Implementations;
using ContactAppNLayer.DataAccess.Interfaces;
using ContactAppNLayer.Models.DTOs;
using ContactAppNLayer.Models.Entities;

namespace ContactAppNLayer.Services.Tests.Implementations
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepo;
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _mockUserRepo = new Mock<IUserRepository>();
            _mockConfig = new Mock<IConfiguration>();

            // Setup dummy JWT config values
            _mockConfig.Setup(c => c["Jwt:Key"]).Returns("thisisaverysecurekey1234567890");
            _mockConfig.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");
            _mockConfig.Setup(c => c["Jwt:Audience"]).Returns("TestAudience");

            _authService = new AuthService(_mockUserRepo.Object, _mockConfig.Object);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnToken_WhenValidCredentials()
        {
            // Arrange
            var request = new LoginRequest
            {
                Username = "testuser",
                Password = "password123"
            };

            _mockUserRepo.Setup(r => r.GetUserByCredentialsAsync(request.Username, request.Password))
                .ReturnsAsync(new User
                {
                    Id = Guid.NewGuid(),
                    Username = request.Username,
                    Password = request.Password,
                    Role = "User"
                });

            // Act
            var result = await _authService.LoginAsync(request);

            // Assert
            result.Should().NotBeNull();
            result!.Token.Should().NotBeNullOrEmpty();
            result.Role.Should().Be("User");
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnNull_WhenInvalidCredentials()
        {
            // Arrange
            var request = new LoginRequest
            {
                Username = "invaliduser",
                Password = "wrongpass"
            };

            _mockUserRepo.Setup(r => r.GetUserByCredentialsAsync(request.Username, request.Password))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _authService.LoginAsync(request);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task RegisterAsync_ShouldReturnTrue_WhenNewUser()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Username = "newuser",
                Password = "newpass123"
            };

            _mockUserRepo.Setup(r => r.RegisterUserAsync(It.Is<User>(u => u.Username == request.Username)))
                .ReturnsAsync(true);

            // Act
            var result = await _authService.RegisterAsync(request);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task RegisterAsync_ShouldReturnFalse_WhenUsernameExists()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Username = "existinguser",
                Password = "pass123"
            };

            _mockUserRepo.Setup(r => r.RegisterUserAsync(It.IsAny<User>()))
                .ReturnsAsync(false);

            // Act
            var result = await _authService.RegisterAsync(request);

            // Assert
            result.Should().BeFalse();
        }
    }
}
