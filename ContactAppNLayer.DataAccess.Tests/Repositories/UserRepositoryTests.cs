using Xunit;
using FluentAssertions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ContactAppNLayer.Models.Entities;
using ContactAppNLayer.DataAccess.Repositories;

namespace ContactAppNLayer.DataAccess.Tests.Repositories
{
    public class UserRepositoryTests
    {
        private DbContextOptions<AppDbContext> _dbOptions;

        public UserRepositoryTests()
        {
            // Use in-memory database for isolated testing
            _dbOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestUserDb")
                .Options;
        }

        [Fact]
        public async Task GetUserByCredentialsAsync_ReturnsUser_WhenCredentialsAreValid()
        {
            // Arrange
            var context = new AppDbContext(_dbOptions);
            context.Users.Add(new User { Username = "john", Password = "pass123", Role = "User" });
            await context.SaveChangesAsync();
            var repo = new UserRepository(context);

            // Act
            var result = await repo.GetUserByCredentialsAsync("john", "pass123");

            // Assert
            result.Should().NotBeNull();
            result!.Username.Should().Be("john");
        }

        [Fact]
        public async Task GetUserByCredentialsAsync_ReturnsNull_WhenPasswordIncorrect()
        {
            // Arrange
            var context = new AppDbContext(_dbOptions);
            context.Users.Add(new User { Username = "jane", Password = "correctpass", Role = "User" });
            await context.SaveChangesAsync();

            var repo = new UserRepository(context);

            // Act
            var result = await repo.GetUserByCredentialsAsync("jane", "wrongpass");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task RegisterUserAsync_ReturnsTrue_WhenUserIsNew()
        {
            // Arrange
            var context = new AppDbContext(_dbOptions);
            var repo = new UserRepository(context);
            var user = new User { Username = "newuser", Password = "123", Role = "User" };

            // Act
            var result = await repo.RegisterUserAsync(user);

            // Assert
            result.Should().BeTrue();
            var dbUser = await context.Users.FirstOrDefaultAsync(u => u.Username == "newuser");
            dbUser.Should().NotBeNull();
        }

        [Fact]
        public async Task RegisterUserAsync_ReturnsFalse_WhenUserAlreadyExists()
        {
            // Arrange
            var context = new AppDbContext(_dbOptions);
            context.Users.Add(new User { Username = "duplicate", Password = "abc", Role = "User" });
            await context.SaveChangesAsync();

            var repo = new UserRepository(context);
            var newUser = new User { Username = "duplicate", Password = "xyz", Role = "User" };

            // Act
            var result = await repo.RegisterUserAsync(newUser);

            // Assert
            result.Should().BeFalse();
        }
    }
}
