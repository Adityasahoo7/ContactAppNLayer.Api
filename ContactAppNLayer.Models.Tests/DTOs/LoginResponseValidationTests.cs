using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xunit;
using FluentAssertions;
using ContactAppNLayer.Models.DTOs;

namespace ContactAppNLayer.Models.Tests.DTOs
{
    public class LoginResponseValidationTests
    {
        private List<ValidationResult> ValidateModel(object model)
        {
            var context = new ValidationContext(model, null, null);
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(model, context, results, true);
            return results;
        }

        [Fact]
        public void LoginResponse_ShouldBeValid_WhenTokenAndRolePresent()
        {
            var response = new LoginResponse
            {
                Token = "sample.jwt.token",
                Role = "Admin"
            };

            var results = ValidateModel(response);

            results.Should().BeEmpty(); // No errors expected
        }

        [Fact]
        public void LoginResponse_ShouldBeInvalid_WhenTokenIsMissing()
        {
            var response = new LoginResponse
            {
                Token = null, // ❌ missing
                Role = "User"
            };

            var results = ValidateModel(response);

            results.Should().Contain(r => r.MemberNames.Contains("Token"));
        }

        [Fact]
        public void LoginResponse_ShouldBeInvalid_WhenRoleIsMissing()
        {
            var response = new LoginResponse
            {
                Token = "sample.jwt.token",
                Role = null // ❌ missing
            };

            var results = ValidateModel(response);

            results.Should().Contain(r => r.MemberNames.Contains("Role"));
        }
    }
}
