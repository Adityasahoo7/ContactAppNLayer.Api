using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using ContactAppNLayer.Models.DTOs;

namespace ContactAppNLayer.Services.Interfaces
{
    public interface IContactService
    {
        Task<List<ContactDto>> GetAllAsync();
        Task<ContactDto?> GetByIdAsync(Guid id);
        Task<ContactDto> AddAsync(AddContactRequest request);
        Task<ContactDto?> UpdateAsync(Guid id, UpdateContactRequest request);
        Task<bool> DeleteAsync(Guid id);
    }
}
