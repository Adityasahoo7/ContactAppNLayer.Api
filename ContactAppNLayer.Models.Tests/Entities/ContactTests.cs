using Xunit;
using System;
using ContactAppNLayer.Models.Entities;
using FluentAssertions;

namespace ContactAppNLayer.Models.Tests.Entities
{
    public class ContactTests
    {
        // Test: Contact object create hela ki nahi
        [Fact]
        public void Contact_Object_Should_BeCreated()
        {
            // Arrange & Act: Gote contact object create karuchhu
            var contact = new Contact();

            // Assert: Object null nuhe — mane object sahi re create hela
            contact.Should().NotBeNull();
        }

        // Test: All properties set & get hela ki nahi check kariba
        [Fact]
        public void Contact_Properties_Should_Set_And_Get_Correctly()
        {
            // Arrange: Sample data prepare kariba
            var id = Guid.NewGuid();
            var name = "Aditya Sahoo";
            var email = "aditya@example.com";
            var phone = 9876543210;
            var address = "Bhubaneswar, Odisha";

            // Act: Contact object re sabu property set kariba
            var contact = new Contact
            {
                Id = id,
                Fullname = name,
                Email = email,
                Phone = phone,
                Address = address
            };

            // Assert: Sabuthire set kichi property correct re get heuchi ki nahi test kariba
            contact.Id.Should().Be(id);
            contact.Fullname.Should().Be(name);
            contact.Email.Should().Be(email);
            contact.Phone.Should().Be(phone);
            contact.Address.Should().Be(address);
        }

        // Negative Test: Default object re Guid khali nuhe ki check kariba
        [Fact]
        public void Contact_Id_Should_Not_Be_Empty_When_Assigned()
        {
            // Arrange: Gote Guid assign kariba
            var id = Guid.NewGuid();

            // Act: Contact object create kariba
            var contact = new Contact { Id = id };

            // Assert: ID empty (Guid.Empty) nuhe
            contact.Id.Should().NotBe(Guid.Empty);
        }

        // Negative Test: Null Fullname set karile exception asuchi ki test kariba (if validation added)
        [Fact]
        public void Contact_Fullname_Can_Be_Null_Because_No_Validation_Is_Present()
        {
            // Act: Null fullname set kariba
            var contact = new Contact { Fullname = null };

            // Assert: Null assign karile bi exception asu nahi — normal behavior
            contact.Fullname.Should().BeNull();
        }
    }
}
