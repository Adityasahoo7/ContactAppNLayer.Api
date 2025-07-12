using Microsoft.AspNetCore.Mvc;


using ContactAppNLayer.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using ContactAppNLayer.Models.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace ContactAppNLayer.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactsController : ControllerBase
    {
        private readonly IContactService _service;

        public ContactsController(IContactService service)
        {
            _service = service;
        }

        [Authorize(Roles = "Admin,User")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }


        //[Authorize(Roles ="Admin")]
        [Authorize(Roles = "Admin,User")]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }


        [Authorize(Roles = "Admin,User")]
        [HttpPost]
        public async Task<IActionResult> Add(AddContactRequest request)
        {
            var added = await _service.AddAsync(request);
            return Ok(added);
        }

        [Authorize(Roles = "Admin,User")]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody]UpdateContactRequest request)
        {
            
            var updated = await _service.UpdateAsync(id, request);
            return updated == null ? NotFound() : Ok(updated);
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _service.DeleteAsync(id);
            return result
            ? Ok(new { message = "Deleted Successfully" })
            : NotFound(new { message = "Contact Not Found" });

        }
    }
}
