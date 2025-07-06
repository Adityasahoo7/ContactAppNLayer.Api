using System.Threading.Tasks;
using ContactAppNLayer.Models.DTOs;

namespace ContactAppNLayer.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse?> LoginAsync(LoginRequest request);

        Task<bool> RegisterAsync(RegisterRequest request);
    }
}
