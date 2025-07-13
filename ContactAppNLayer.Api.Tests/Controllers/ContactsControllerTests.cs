using Xunit; // xUnit framework pain
using Moq; // Mocking framework pain (dependency fake karibaku)
using FluentAssertions; // Better assertion syntax pain
using Microsoft.AspNetCore.Mvc; // ASP.NET Core MVC types pain
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using ContactAppNLayer.Api.Controllers; // Controller class reference
using ContactAppNLayer.Models.DTOs; // DTO classes
using ContactAppNLayer.Services.Interfaces; // Service interface reference
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ContactAppNLayer.Api.Tests.Controllers
{
    public class ContactsControllerTests
    {
        // Mock object banauchhu service interface pain
        private readonly Mock<IContactService> _mockService;

        // Controller object
        private readonly ContactsController _controller;

        public ContactsControllerTests()
        {
            // Service interface ku mock karuchhi, jaha real implementation nuhen
            _mockService = new Mock<IContactService>();

            // Ei mocked service ku controller re inject karuchhi
            _controller = new ContactsController(_mockService.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOk_WithContacts()
        {
            // Arrange
            var username = "testuser";
            var role = "User";

            var contacts = new List<ContactDto>
    {
        new ContactDto { Id = Guid.NewGuid(), Fullname = "A", Email = "a@x.com", Phone = 123, Address = "Addr" }
    };

            _mockService.Setup(s => s.GetAllAsync(username, role)).ReturnsAsync(contacts);

            // Mock User.Identity and Claims
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.Name, username),
        new Claim(ClaimTypes.Role, role)
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(200);
            (okResult.Value as List<ContactDto>)!.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetById_ReturnsOk_WhenContactExists()
        {
            // Arrange: Dummy contact create karuchhi
            var id = Guid.NewGuid();
            var contact = new ContactDto { Id = id, Fullname = "Test" };

            // Jebe GetByIdAsync(id) call hela, ta pare ei contact return heba
            _mockService.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(contact);

            // Act: Controller ra GetById call karuchhi
            var result = await _controller.GetById(id);

            // Assert: Result ta OkObjectResult type ra hela ki nahi
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenContactNotExists()
        {
            // Arrange: GetByIdAsync ku null return kara
            _mockService.Setup(s => s.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((ContactDto?)null);

            // Act: Controller call karuchhi
            var result = await _controller.GetById(Guid.NewGuid());

            // Assert: NotFound() expected
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Add_ReturnsOk_WhenValidRequest()
        {
            // Arrange
            var username = "testuser";
            var request = new AddContactRequest { Fullname = "New", Email = "new@e.com", Phone = 999, Address = "Xyz" };
            var added = new ContactDto { Id = Guid.NewGuid(), Fullname = request.Fullname };

            _mockService.Setup(s => s.AddAsync(request, username)).ReturnsAsync(added);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.Name, username)
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var result = await _controller.Add(request);

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeOfType<ContactDto>();
        }


        [Fact]
        public async Task Update_ReturnsOk_WhenContactExists()
        {
            // Arrange: Dummy ID and update request
            var id = Guid.NewGuid();
            var request = new UpdateContactRequest { Fullname = "Upd", Email = "u@e.com", Phone = 888, Address = "ZZZ" };
            var updated = new ContactDto { Id = id, Fullname = request.Fullname };

            // Mock setup: contact exist karuchi
            _mockService.Setup(s => s.UpdateAsync(id, request)).ReturnsAsync(updated);

            // Act
            var result = await _controller.Update(id, request);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenContactNotExists()
        {
            // Arrange: Null return kara jebe contact nai
            _mockService.Setup(s => s.UpdateAsync(It.IsAny<Guid>(), It.IsAny<UpdateContactRequest>())).ReturnsAsync((ContactDto?)null);

            var request = new UpdateContactRequest();

            // Act
            var result = await _controller.Update(Guid.NewGuid(), request);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_ReturnsOk_WhenDeleted()
        {
            // Arrange: DeleteAsync true return karuchi
            _mockService.Setup(s => s.DeleteAsync(It.IsAny<Guid>())).ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(Guid.NewGuid());

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenContactNotFound()
        {
            // Arrange: DeleteAsync false return karuchi
            _mockService.Setup(s => s.DeleteAsync(It.IsAny<Guid>())).ReturnsAsync(false);

            // Act
            var result = await _controller.Delete(Guid.NewGuid());

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
