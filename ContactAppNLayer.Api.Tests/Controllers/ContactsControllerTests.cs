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

        [Fact] // Unit test mark kare
        public async Task GetAll_ReturnsOk_WithContacts()
        {
            // Arrange: Dummy contact list banauchhu
            var contacts = new List<ContactDto>
            {
                new ContactDto { Id = Guid.NewGuid(), Fullname = "A", Email = "a@x.com", Phone = 123, Address = "Addr" }
            };

            // Mocked service re GetAllAsync call hele ei list return kariba
            _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(contacts);

            // Act: Controller ra GetAll method ku call karuchhu
            var result = await _controller.GetAll();

            // Assert: Response ta OkObjectResult type ra hela ki nahi ta check karuchhi
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200); // HTTP 200 expected
            (okResult.Value as List<ContactDto>)!.Should().HaveCount(1); // 1 contact thiba expected
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
            // Arrange: Add request & mock return value set karuchhi
            var request = new AddContactRequest { Fullname = "New", Email = "new@e.com", Phone = 999, Address = "Xyz" };
            var added = new ContactDto { Id = Guid.NewGuid(), Fullname = request.Fullname };

            _mockService.Setup(s => s.AddAsync(request)).ReturnsAsync(added);

            // Act
            var result = await _controller.Add(request);

            // Assert: Ok result and valid object expected
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
