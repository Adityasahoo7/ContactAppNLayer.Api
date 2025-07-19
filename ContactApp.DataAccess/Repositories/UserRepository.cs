using System.Threading.Tasks;
using ContactAppNLayer.DataAccess.Interfaces;
using ContactAppNLayer.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ContactAppNLayer.DataAccess.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByCredentialsAsync(string username, string password)
        {

            //return await _context.Users.FirstOrDefaultAsync(
            //  u => u.Username == username && u.Password == password);




            //var user = await _context.Users
            // .FirstOrDefaultAsync(u => u.Username == username &&  password == u.Password);
            //U.username data get from DB and username data get from User input 
            //Using _context.user we connect with db and it is the Dbset


            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

            if(user != null && user.Password == password)
            {
                return user;
            }
            else
            {   
                return null; 
            }

          

           // return user;





            //Not good for huge db 
            //return await Task.FromResult(
            //  _context.Users
            //  .AsEnumerable()
            //  .FirstOrDefault(u =>
            //     string.Equals(u.Username, username, StringComparison.Ordinal) &&
            //        string.Equals(u.Password, password, StringComparison.Ordinal))
            // );

        }
        //Method for signup a user 
        public async Task<bool> RegisterUserAsync(User user)
        {
            var exists = await _context.Users.AnyAsync(u => u.Username == user.Username);
            if (exists) return false; // Already exists

            // DB re insert kara
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}