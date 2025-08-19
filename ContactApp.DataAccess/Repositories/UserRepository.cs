using System.Threading.Tasks;
using ContactAppNLayer.DataAccess.Interfaces;
using ContactAppNLayer.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ContactAppNLayer.DataAccess.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(AppDbContext context,ILogger<UserRepository> logger)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<User?> GetUserByCredentialsAsync(string username, string password)
        {
            try
            {
                _logger.LogInformation("(UserRepository) Attempting to fetch user with username: {Username}", username);

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

                if (user == null)
                {
                    _logger.LogWarning("(UserRepository) No user found with username: {Username}", username);
                    return null;
                }
                if (user.Password == password)
                {
                    _logger.LogInformation("(UserRepository) User authentication successful for username: {Username}", username);
                    return user;
                }
                else
                {
                    _logger.LogWarning("(UserRepository) Invalid password attempt for username: {Username}", username);
                    return null;
                }
                //if (user != null && user.Password == password)
                //{
                //    return user;
                //}
                //else
                //{
                //    return null;
                //}
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "(UserRepository) Error occurred while fetching user with username: {Username}", username);
                throw;
            }
          
        }
        //Method for signup a user 
        public async Task<bool> RegisterUserAsync(User user)
        {
            try
            {

                _logger.LogInformation("(UserRepository) Attempting to register new user with username: {Username}", user.Username);

                var exists = await _context.Users.AnyAsync(u => u.Username == user.Username);
                if (exists)
                {
                    _logger.LogWarning("(UserRepository) User registration failed. Username {Username} already exists.", user.Username);
                    return false; // Already exists
                }
                   

                // DB re insert kara
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                _logger.LogInformation("(UserRepository) User with username: {Username} registered successfully.", user.Username);

                return true;
            }catch(Exception ex)
            {
                _logger.LogError(ex, "(UserRepository) Error occurred while registering user with username: {Username}", user.Username);
                throw;
            }
         
        }

    }
}