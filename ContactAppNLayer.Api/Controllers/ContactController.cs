using Microsoft.AspNetCore.Mvc;


using ContactAppNLayer.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using ContactAppNLayer.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ContactAppNLayer.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactsController : ControllerBase
    {
        private readonly IContactService _service;
        private readonly ILogger<ContactsController> _logger;

        public ContactsController(IContactService service, ILogger<ContactsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [Authorize(Roles = "Admin,User")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            // return Ok(await _service.GetAllAsync());

            var username = User.Identity?.Name;
            var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            _logger.LogInformation("Fetching all contacts for user: {Username}, role: {Role} at {Time}", username, role, DateTime.UtcNow);


            try
            {
                var result = await _service.GetAllAsync(username, role);
                _logger.LogInformation("Fetched {Count} contacts for user: {Username}", result?.Count(), username);

                return Ok(result);
            }catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching contacts for user: {Username}", username);
                return StatusCode(500, "Internal Server error");
            }


        }


        //[Authorize(Roles ="Admin")]
        [Authorize(Roles = "Admin,User")]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {

            _logger.LogInformation("Fetching contact by Id: {Id} at {Time}", id, DateTime.UtcNow);

            try
            {
                var result = await _service.GetByIdAsync(id);
                //return result == null ? NotFound() : Ok(result);
                if (result == null)
                {
                    _logger.LogWarning("Contact not found with Id: {Id}", id);
                    return NotFound(new { message = "Contact not found" });
                }

                _logger.LogInformation("Contact found with Id: {Id}", id);
                return Ok(result);
            } catch (Exception ex) {
                _logger.LogError(ex, "Error occurred while fetching contact by Id: {Id}", id);
                return StatusCode(500, "Internal server error");

            }

          
        }


        [Authorize(Roles = "Admin,User")]
        [HttpPost]
        public async Task<IActionResult> Add(AddContactRequest request)
        {
            var username = User.Identity?.Name;
            _logger.LogInformation("Adding new contact for user: {Username} at {Time}", username, DateTime.UtcNow);

            try
            {
                var added = await _service.AddAsync(request, username!);
                _logger.LogInformation("Contact added successfully: {Fullname}, Id: {Id}", added.Fullname, added.Id);

                return Ok(added);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding contact for user: {Username}", username);
                return StatusCode(500, "Internal server error");
            }
           
        }

        [Authorize(Roles = "Admin,User")]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody]UpdateContactRequest request)
        {
            _logger.LogInformation("Updating contact Id: {Id} at {Time}", id, DateTime.UtcNow);

            try
            {
                var updated = await _service.UpdateAsync(id, request);
                // return updated == null ? NotFound() : Ok(updated);
                if (updated == null)
                {
                    _logger.LogWarning("Contact not found while updating. Id: {Id}", id);
                    return NotFound(new { message = "Contact not found" });
                }

                _logger.LogInformation("Contact updated successfully. Id: {Id}", id);
                return Ok(updated);
            }catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating contact Id: {Id}", id);
                return StatusCode(500, "Internal server error");
            }

            
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {

            _logger.LogInformation("Deleting contact Id: {Id} at {Time}", id, DateTime.UtcNow);

            try
            {
                var result = await _service.DeleteAsync(id);
                //return result
                //? Ok(new { message = "Deleted Successfully" })
                //: NotFound(new { message = "Contact Not Found" });
                if (!result)
                {
                    _logger.LogWarning("Delete failed. Contact not found. Id: {Id}", id);
                    return NotFound(new { message = "Contact not found" });
                }

                _logger.LogInformation("Contact deleted successfully. Id: {Id}", id);
                return Ok(new { message = "Deleted successfully" });
            }catch(Exception ex) {

                _logger.LogError(ex, "Error occurred while deleting contact Id: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
           

        }
    }
}
