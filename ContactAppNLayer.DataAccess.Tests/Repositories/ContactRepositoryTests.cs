
using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using ContactAppNLayer.Models.Entities;
using ContactAppNLayer.DataAccess.Repositories;


namespace ContactAppNLayer.DataAccess.Tests.Repositories
{
    public class ContactRepositoryTests
    {
        private readonly AppDbContext _context;
        private readonly ContactRepository _repository;

        public ContactRepositoryTests()
        {
            // 🧠 Test pain in-memory DB banauchu, jadi real SQL server use heba nahi
            var options = new DbContextOptionsBuilder<AppDbContext>()
           .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // in-memory DB banauchi
           .Options;


            _context = new AppDbContext(options);
            _repository = new ContactRepository(_context);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllContacts_WhenContactsExist()
        {
            // Arrange: Sample 2 contact DB re set karuchu
            _context.Contacts.AddRange(new List<Contact>
            {
                new Contact { Id = Guid.NewGuid(), Fullname = "A", Email = "a@email.com", Phone = 123, Address = "X" },
                new Contact { Id = Guid.NewGuid(), Fullname = "B", Email = "b@email.com", Phone = 456, Address = "Y" }
            });
            await _context.SaveChangesAsync();

            // Act: GetAllAsync call karuchu
            var result = await _repository.GetAllAsync();

            // Assert: 2 contacts asiba
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnContact_WhenExists()
        {
            // Arrange: Gote contact DB re add kara
            var contact = new Contact { Id = Guid.NewGuid(), Fullname = "Test", Email = "test@email.com", Phone = 111, Address = "Test" };
            _context.Contacts.Add(contact);
            await _context.SaveChangesAsync();

            // Act: ID dwara find kara
            var result = await _repository.GetByIdAsync(contact.Id);

            // Assert: Matching ID asiba
            result.Should().NotBeNull();
            result.Id.Should().Be(contact.Id);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
        {
            // Act: Random ID pass karuchu jaha DB re nahi
            var result = await _repository.GetByIdAsync(Guid.NewGuid());

            // Assert: Null return heba
            result.Should().BeNull();
        }

        [Fact]
        public async Task AddAsync_ShouldAddContactAndReturn()
        {
            // Arrange: Add kariba pain gote contact object
            var contact = new Contact { Fullname = "Add", Email = "add@email.com", Phone = 999, Address = "Addr" };

            // Act: Repository re add karuchu
            var result = await _repository.AddAsync(contact);

            // Assert: ID assign hela ki au DB re save hela ki test karuchu
            result.Id.Should().NotBe(Guid.Empty);
            var check = await _context.Contacts.FindAsync(result.Id);
            check.Should().NotBeNull();
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateContact_WhenExists()
        {
            // Arrange: Gote contact create karuchu
            var contact = new Contact { Id = Guid.NewGuid(), Fullname = "Old", Email = "old@email.com", Phone = 100, Address = "OldAddr" };
            _context.Contacts.Add(contact);
            await _context.SaveChangesAsync();

            // Act: Fullname update karuchu
            contact.Fullname = "Updated";
            var result = await _repository.UpdateAsync(contact);

            // Assert: DB re update hela ki check karuchu
            result.Fullname.Should().Be("Updated");
            var updated = await _context.Contacts.FindAsync(contact.Id);
            updated.Fullname.Should().Be("Updated");
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnTrue_WhenExists()
        {
            // Arrange: Gote contact DB re set karuchu
            var contact = new Contact { Id = Guid.NewGuid(), Fullname = "Del", Email = "del@email.com", Phone = 777, Address = "DelAddr" };
            _context.Contacts.Add(contact);
            await _context.SaveChangesAsync();

            // Act: Delete karuchu
            var result = await _repository.DeleteAsync(contact.Id);

            // Assert: Return true au DB re contact delete heijae
            result.Should().BeTrue();
            var deleted = await _context.Contacts.FindAsync(contact.Id);
            deleted.Should().BeNull();
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenNotExists()
        {
            // Act: Delete kara random ID pass kari
            var result = await _repository.DeleteAsync(Guid.NewGuid());

            // Assert: Return false heba
            result.Should().BeFalse();
        }
    }
}
