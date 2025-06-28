using Xunit; // Unit testing framework
using Moq; // Mocking library
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions; // Assert syntax ko readable banai thae

// Required namespaces from your project
using ContactAppNLayer.Services.Implementations;
using ContactAppNLayer.DataAccess.Interfaces;
using ContactAppNLayer.Models.Entities;
using ContactAppNLayer.Models.DTOs;

namespace ContactAppNLayer.Services.Tests.Implementations
{
    public class ContactServiceTests
    {
        // Mock repository instance create karuchu
        private readonly Mock<IContactRepository> _mockRepo;

        // ContactService instance (under test)
        private readonly ContactService _service;

        // Constructor: mock repo inject karuchu service re
        public ContactServiceTests()
        {
            _mockRepo = new Mock<IContactRepository>();
            _service = new ContactService(_mockRepo.Object); // mock inject
        }

        // ✅ Test case: GetAllAsync call heile sabu contact DTO return karuchi ki
        [Fact]
        public async Task GetAllAsync_WhenContactsExist_ReturnsAllContactDtos()
        {
            // Arrange: 1 contact fake data dei deluchu
            var contacts = new List<Contact>
            {
                new Contact { Id = Guid.NewGuid(), Fullname = "Test", Email = "t@e.com", Phone=123, Address="A" }
            };
            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(contacts); // mock setup

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            result.Should().HaveCount(1); // 1 contact asila ki
            result[0].Fullname.Should().Be("Test"); // naam match karuchhi ki
        }

        // ✅ Test case: GetByIdAsync — contact exist karile DTO return karuchhi ki
        [Fact]
        public async Task GetByIdAsync_WhenContactExists_ReturnsContactDto()
        {
            // Arrange: mock contact prepare
            var id = Guid.NewGuid();
            var contact = new Contact { Id = id, Fullname = "John", Email = "john@example.com", Phone = 12345, Address = "Odisha" };
            _mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(contact);

            // Act
            var result = await _service.GetByIdAsync(id);

            // Assert
            result.Should().NotBeNull(); // result null nuhein
            result.Id.Should().Be(id); // id match karuchhi
        }

        // ❌ Negative test: contact exist na karile null return karuchhi ki
        [Fact]
        public async Task GetByIdAsync_WhenContactNotExists_ReturnsNull()
        {
            // Arrange: null return karuchhi repository ru
            _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Contact)null);

            // Act
            var result = await _service.GetByIdAsync(Guid.NewGuid());

            // Assert
            result.Should().BeNull();
        }

        // ✅ Test case: AddAsync — contact add hela pare thik DTO return hela ki
        [Fact]
        public async Task AddAsync_ShouldAddContactAndReturnDto()
        {
            // Arrange: request DTO set karuchhi
            var req = new AddContactRequest
            {
                Fullname = "New",
                Email = "new@email.com",
                Phone = 999,
                Address = "BBSR"
            };

            // Add karapare repository jo return karuthae — contact entity
            var contactEntity = new Contact
            {
                Id = Guid.NewGuid(),
                Fullname = req.Fullname,
                Email = req.Email,
                Phone = req.Phone,
                Address = req.Address
            };
            _mockRepo.Setup(r => r.AddAsync(It.IsAny<Contact>())).ReturnsAsync(contactEntity);

            // Act
            var dto = await _service.AddAsync(req);

            // Assert
            dto.Fullname.Should().Be(req.Fullname);
            dto.Id.Should().Be(contactEntity.Id);
        }

        // ✅ Positive case: contact exist karile update hela ki
        [Fact]
        public async Task UpdateAsync_WhenContactExists_ShouldUpdateAndReturnDto()
        {
            // Arrange: 1 ta existing contact au 1 ta updated contact
            var id = Guid.NewGuid();
            var existing = new Contact
            {
                Id = id,
                Fullname = "Old",
                Email = "old@email.com",
                Phone = 111,
                Address = "Old"
            };
            var updated = new Contact
            {
                Id = id,
                Fullname = "Updated",
                Email = "updated@email.com",
                Phone = 222,
                Address = "New"
            };

            // Mock setup: first find, then update
            _mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existing);
            _mockRepo.Setup(r => r.UpdateAsync(existing)).ReturnsAsync(updated);

            // Update request data
            var request = new UpdateContactRequest
            {
                Fullname = "Updated",
                Email = "updated@email.com",
                Phone = 222,
                Address = "New"
            };

            // Act
            var result = await _service.UpdateAsync(id, request);

            // Assert
            result.Should().NotBeNull();
            result.Fullname.Should().Be("Updated");
        }

        // ❌ Negative case: update call hela kintu contact pai nalani — null return kariba
        [Fact]
        public async Task UpdateAsync_WhenContactNotExists_ReturnsNull()
        {
            // Arrange: mock GetByIdAsync returns null
            _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Contact)null);

            var request = new UpdateContactRequest
            {
                Fullname = "Test",
                Email = "t@e.com",
                Phone = 123,
                Address = "Addr"
            };

            // Act
            var result = await _service.UpdateAsync(Guid.NewGuid(), request);

            // Assert
            result.Should().BeNull();
        }

        // ✅ Positive case: delete contact — return true
        [Fact]
        public async Task DeleteAsync_WhenContactExists_ReturnsTrue()
        {
            // Arrange
            _mockRepo.Setup(r => r.DeleteAsync(It.IsAny<Guid>())).ReturnsAsync(true);

            // Act
            var result = await _service.DeleteAsync(Guid.NewGuid());

            // Assert
            result.Should().BeTrue();
        }

        // ❌ Negative case: contact exist nai — delete fail heichi — return false
        [Fact]
        public async Task DeleteAsync_WhenContactNotExists_ReturnsFalse()
        {
            // Arrange
            _mockRepo.Setup(r => r.DeleteAsync(It.IsAny<Guid>())).ReturnsAsync(false);

            // Act
            var result = await _service.DeleteAsync(Guid.NewGuid());

            // Assert
            result.Should().BeFalse();
        }
    }
}
