using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ContactAppNLayer.Models.Entities;
using ContactAppNLayer.DataAccess.Interfaces;
using ContactAppNLayer.Services.Interfaces;
using ContactAppNLayer.Models.DTOs;
using Microsoft.Extensions.Logging;

namespace ContactAppNLayer.Services.Implementations
{
    public class ContactService : IContactService
    {
        private readonly IContactRepository _repository;
        private readonly ILogger<ContactService> _logger;

        public ContactService(IContactRepository repository, ILogger<ContactService> logger)
        {
            _logger= logger;    
            _repository = repository;
        }

        public async Task<List<ContactDto>> GetAllAsync(string username, string role)
        {
            _logger.LogInformation("(ContactService) Fetching all contacts for user {Username} with role {Role} at {Time}",
             username, role, DateTime.UtcNow);

            var allContacts = await _repository.GetAllAsync();

            // Role-based filtering
            var filteredContacts = role == "Admin"
                ? allContacts
                : allContacts.Where(c => c.CreatedBy == username);


            _logger.LogInformation("(ContactService) Successfully fetched {Count} contacts for user {Username} at {Time}",
               filteredContacts.Count(), username, DateTime.UtcNow);


            return filteredContacts.Select(c => new ContactDto
            {
                Id = c.Id,
                Fullname = c.Fullname,
                Email = c.Email,
                Phone = c.Phone,
                Address = c.Address
            }).ToList();
        }


        public async Task<ContactDto?> GetByIdAsync(Guid id)
        {
            _logger.LogInformation("(ContactService) Fetching contact with ID {Id} at {Time}", id, DateTime.UtcNow);

            var c = await _repository.GetByIdAsync(id);

            if(c==null)
            {
                _logger.LogWarning("(ContactService) Contact with ID {Id} not found at {Time}", id, DateTime.UtcNow);
                return null;
            }
            _logger.LogInformation("(ContactService) Contact with ID {Id} retrieved successfully at {Time}", id, DateTime.UtcNow);


            return new ContactDto
            {
                Id = c.Id,
                Fullname = c.Fullname,
                Email = c.Email,
                Phone = c.Phone,
                Address = c.Address
            };

          
        }

        public async Task<ContactDto> AddAsync(AddContactRequest request, string createdBy)
        {
            _logger.LogInformation("(ContactService) Adding new contact {Fullname} for user {CreatedBy} at {Time}",
              request.Fullname, createdBy, DateTime.UtcNow);
    
            var contact = new Contact
            {
                Fullname = request.Fullname,
                Email = request.Email,
                Phone = request.Phone,
                Address = request.Address,
                CreatedBy = createdBy
            };
            var added = await _repository.AddAsync(contact);

            _logger.LogInformation("(ContactService) Contact {Fullname} added successfully with ID {Id} at {Time}",
              added.Fullname, added.Id, DateTime.UtcNow);


            return new ContactDto
            {
                Id = added.Id,
                Fullname = added.Fullname,
                Email = added.Email,
                Phone = added.Phone,
                Address = added.Address
            };
        }

        public async Task<ContactDto?> UpdateAsync(Guid id, UpdateContactRequest request)
        {
            _logger.LogInformation("(ContactService) Updating contact with ID {Id} at {Time}", id, DateTime.UtcNow);

            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
            {
                _logger.LogWarning("(ContactService) Contact with ID {Id} not found for update at {Time}", id, DateTime.UtcNow);
                return null;
            }

            existing.Fullname = request.Fullname;
            existing.Email = request.Email;
            existing.Phone = request.Phone;
            existing.Address = request.Address;

            var updated = await _repository.UpdateAsync(existing);

            _logger.LogInformation("(ContactService) Contact with ID {Id} updated successfully at {Time}", id, DateTime.UtcNow);


            return new ContactDto
            {
                Id = updated.Id,
                Fullname = updated.Fullname,
                Email = updated.Email,
                Phone = updated.Phone,
                Address = updated.Address
            };
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            _logger.LogInformation("(ContactService) Deleting contact with ID {Id} at {Time}", id, DateTime.UtcNow);

            var result = await _repository.DeleteAsync(id);

            if (!result)
            {
                _logger.LogInformation("(ContactService) Contact with ID {Id} deleted successfully at {Time}", id, DateTime.UtcNow);

            }
            else
            {
                _logger.LogWarning("(ContactService) Failed to delete contact with ID {Id} at {Time}", id, DateTime.UtcNow);

            }
            return result;
        }
    }
}
