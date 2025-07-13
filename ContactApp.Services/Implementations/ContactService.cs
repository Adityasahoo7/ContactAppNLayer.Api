using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ContactAppNLayer.Models.Entities;
using ContactAppNLayer.DataAccess.Interfaces;
using ContactAppNLayer.Services.Interfaces;
using ContactAppNLayer.Models.DTOs;

namespace ContactAppNLayer.Services.Implementations
{
    public class ContactService : IContactService
    {
        private readonly IContactRepository _repository;

        public ContactService(IContactRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ContactDto>> GetAllAsync(string username, string role)
        {
            var allContacts = await _repository.GetAllAsync();

            // Role-based filtering
            var filteredContacts = role == "Admin"
                ? allContacts
                : allContacts.Where(c => c.CreatedBy == username);

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
            var c = await _repository.GetByIdAsync(id);
            return c == null ? null : new ContactDto
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
            var contact = new Contact
            {
                Fullname = request.Fullname,
                Email = request.Email,
                Phone = request.Phone,
                Address = request.Address,
                CreatedBy = createdBy
            };
            var added = await _repository.AddAsync(contact);

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
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return null;

            existing.Fullname = request.Fullname;
            existing.Email = request.Email;
            existing.Phone = request.Phone;
            existing.Address = request.Address;

            var updated = await _repository.UpdateAsync(existing);

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
            return await _repository.DeleteAsync(id);
        }

        //public Task<ContactDto> AddAsync(AddContactRequest request)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
