using System.Threading.Tasks;
using ContactAppNLayer.Models.Entities;

namespace ContactAppNLayer.DataAccess.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetUserByCredentialsAsync(string username, string password);
        Task<bool> RegisterUserAsync(User user);//Method for Signup a user
    }
}
