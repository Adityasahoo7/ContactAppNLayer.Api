using Xunit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using ContactAppNLayer.Models.Entities;
using ContactAppNLayer.DataAccess;
using FluentAssertions;

namespace ContactAppNLayer.DataAccess.Tests
{
    public class AppDbContextTests
    {
        private AppDbContext CreateInMemoryContext()
        {
            // 🧠 Test database banauchi memory re (SQL server use hela nahi)
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task AppDbContext_Should_Create_And_Add_Contact()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var contact = new Contact
            {
                Id = Guid.NewGuid(),
                Fullname = "Test Name",
                Email = "test@email.com",
                Phone = 1234567890,
                Address = "Odisha"
            };

            // Act
            await context.Contacts.AddAsync(contact);
            await context.SaveChangesAsync();

            // Assert
            var result = await context.Contacts.FindAsync(contact.Id);
            result.Should().NotBeNull(); // Contact properly save hela ki
            result.Email.Should().Be("test@email.com");
        }

        [Fact]
        public void AppDbContext_Should_Have_DbSet_For_Contacts()
        {
            // Arrange
            var context = CreateInMemoryContext();

            // Assert: Contacts table jaha `DbSet<Contact>` ru represent hela
            context.Contacts.Should().NotBeNull(); // DbSet initialized achhi ki
        }
    }
}
