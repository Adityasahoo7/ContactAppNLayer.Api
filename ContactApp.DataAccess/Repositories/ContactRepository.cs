using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContactAppNLayer.DataAccess.Interfaces;
using ContactAppNLayer.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ContactAppNLayer.DataAccess.Repositories
{
    public class ContactRepository : IContactRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ContactRepository> _logger;

        public ContactRepository(AppDbContext context,ILogger<ContactRepository> logger)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<List<Contact>> GetAllAsync()
        {
            _logger.LogInformation("(Contact Repository) Fetching all contacts from database.");
            var contacts = await _context.Contacts.ToListAsync();
            _logger.LogInformation("(Contact Repository) Fetched {Count} contacts.", contacts.Count);
            return contacts;
        }

        public async Task<Contact> GetByIdAsync(Guid id)
        {
            _logger.LogInformation("(Contact Repository) Fetching contact with Id: {Id}", id);
            var contact= await _context.Contacts.FindAsync(id);
           
            if(contact == null)
            {
                _logger.LogWarning("(Contact Repository) Contact with Id: {Id} not found.", id);
            }
            else
            {
                _logger.LogInformation("(Contact Repository) Contact with Id: {Id} retrieved successfully.", id);
            }
            return contact;
        }

        public async Task<Contact> AddAsync(Contact contact)
        {
            contact.Id = Guid.NewGuid();
            _logger.LogInformation("(Contact Repository) Adding new contact with Id: {Id}", contact.Id);

            _context.Contacts.Add(contact);
            await _context.SaveChangesAsync();

            _logger.LogInformation("(Contact Repository) Contact with Id: {Id} added successfully.", contact.Id);
            return contact;
        }

        public async Task<Contact> UpdateAsync(Contact contact)
        {
            _logger.LogInformation("(Contact Repository) Updating contact with Id: {Id}", contact.Id);

            _context.Contacts.Update(contact);
            await _context.SaveChangesAsync();


            _logger.LogInformation("(Contact Repository) Contact with Id: {Id} updated successfully.", contact.Id);

            return contact;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            _logger.LogInformation("(Contact Repository) Attempting to delete contact with Id: {Id}", id);

            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null)
            {
                _logger.LogWarning("(Contact Repository) Delete failed. Contact with Id: {Id} not found.", id);
                return false;
            }

            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync();

            _logger.LogInformation("(Contact Repository) Contact with Id: {Id} deleted successfully.", id);

            return true;
        }
    }
}
