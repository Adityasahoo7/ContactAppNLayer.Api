using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xunit;
using FluentAssertions;
using ContactAppNLayer.Models.DTOs;

namespace ContactAppNLayer.Models.Tests.DTOs
{
    public class LoginRequestValidationTests
    {
        private List<ValidationResult> ValidateModel(object model)
        {
            var context = new ValidationContext(model, null, null);
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(model, context, results, true);
            return results;
        }

        [Fact]
        public void LoginRequest_ShouldBeValid_WhenDataIsCorrect()
        {
            var request = new LoginRequest
            {
                Username = "validuser",
                Password = "validpass123"
            };

            var results = ValidateModel(request);

            results.Should().BeEmpty(); // no validation errors
        }

        [Fact]
        public void LoginRequest_ShouldBeInvalid_WhenUsernameIsEmpty()
        {
            var request = new LoginRequest
            {
                Username = "", // ❌ empty
                Password = "validpass123"
            };

            var results = ValidateModel(request);

            results.Should().Contain(r => r.MemberNames.Contains("Username"));
        }

        [Fact]
        public void LoginRequest_ShouldBeInvalid_WhenPasswordIsTooShort()
        {
            var request = new LoginRequest
            {
                Username = "validuser",
                Password = "123" // ❌ too short
            };

            var results = ValidateModel(request);

            results.Should().Contain(r => r.MemberNames.Contains("Password"));
        }

        [Fact]
        public void LoginRequest_ShouldBeInvalid_WhenUsernameTooLong()
        {
            var request = new LoginRequest
            {
                Username = new string('x', 51), // ❌ exceeds max
                Password = "validpass123"
            };

            var results = ValidateModel(request);

            results.Should().Contain(r => r.MemberNames.Contains("Username"));
        }
    }
}
