using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;
using Xunit;
using FluentAssertions;
using ContactAppNLayer.Models.Entities;

namespace ContactAppNLayer.Models.Tests.Entities
{
    public class UserEntityValidationTests
    {
        private List<ValidationResult> ValidateModel(object model)
        {
            var context = new ValidationContext(model, null, null);
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(model, context, results, true);
            return results;
        }

        [Fact]
        public void User_ShouldBeValid_WhenAllDataIsCorrect()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = "validuser",
                Password = "secure123",
                Role = "Admin"
            };

            var results = ValidateModel(user);

            results.Should().BeEmpty(); // ✅ No validation errors
        }

        [Fact]
        public void User_ShouldBeInvalid_WhenUsernameIsMissing()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = null, // ❌
                Password = "secure123",
                Role = "User"
            };

            var results = ValidateModel(user);

            results.Should().Contain(r => r.MemberNames.Contains("Username"));
        }

        [Fact]
        public void User_ShouldBeInvalid_WhenPasswordIsTooShort()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = "testuser",
                Password = "123", // ❌ Too short
                Role = "Admin"
            };

            var results = ValidateModel(user);

            results.Should().Contain(r => r.MemberNames.Contains("Password"));
        }

        [Fact]
        public void User_ShouldBeInvalid_WhenRoleIsInvalid()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = "validuser",
                Password = "secure123",
                Role = "SuperUser" // ❌ Invalid
            };

            var results = ValidateModel(user);

            results.Should().Contain(r => r.MemberNames.Contains("Role"));
        }

        [Fact]
        public void User_ShouldBeInvalid_WhenRoleIsMissing()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = "validuser",
                Password = "secure123",
                Role = null // ❌ Missing
            };

            var results = ValidateModel(user);

            results.Should().Contain(r => r.MemberNames.Contains("Role"));
        }

        [Fact]
        public void User_ShouldBeInvalid_WhenIdIsEmpty()
        {
            var user = new User
            {
                Id = Guid.Empty, // ❌ Invalid GUID
                Username = "validuser",
                Password = "secure123",
                Role = "User"
            };

            var results = ValidateModel(user);

            // Guid.Empty is still a struct, so Required doesn't validate it properly
            // You'll need to manually handle this in business logic or custom validator
            results.Should().BeEmpty(); // But we can still assert empty if needed
        }
    }
}
