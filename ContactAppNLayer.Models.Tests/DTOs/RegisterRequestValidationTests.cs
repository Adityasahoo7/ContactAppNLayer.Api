using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xunit;
using FluentAssertions;
using ContactAppNLayer.Models.DTOs;

namespace ContactAppNLayer.Models.Tests.DTOs
{
    public class RegisterRequestValidationTests
    {
        private List<ValidationResult> ValidateModel(object model)
        {
            var context = new ValidationContext(model, null, null);
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(model, context, results, true);
            return results;
        }

        [Fact]
        public void RegisterRequest_ShouldBeValid_WhenDataIsCorrect()
        {
            var request = new RegisterRequest
            {
                Username = "validuser",
                Password = "securepass123"
            };

            var results = ValidateModel(request);

            results.Should().BeEmpty(); // ✅ No validation errors
        }

        [Fact]
        public void RegisterRequest_ShouldBeInvalid_WhenUsernameIsMissing()
        {
            var request = new RegisterRequest
            {
                Username = null, // ❌
                Password = "securepass123"
            };

            var results = ValidateModel(request);

            results.Should().Contain(r => r.MemberNames.Contains("Username"));
        }

        [Fact]
        public void RegisterRequest_ShouldBeInvalid_WhenPasswordIsTooShort()
        {
            var request = new RegisterRequest
            {
                Username = "validuser",
                Password = "123" // ❌ Too short
            };

            var results = ValidateModel(request);

            results.Should().Contain(r => r.MemberNames.Contains("Password"));
        }

        [Fact]
        public void RegisterRequest_ShouldBeInvalid_WhenUsernameTooLong()
        {
            var request = new RegisterRequest
            {
                Username = new string('x', 51), // ❌ Exceeds 50
                Password = "validpass123"
            };

            var results = ValidateModel(request);

            results.Should().Contain(r => r.MemberNames.Contains("Username"));
        }
    }
}
