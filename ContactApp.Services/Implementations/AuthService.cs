using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ContactAppNLayer.DataAccess.Interfaces;
using ContactAppNLayer.Models.DTOs;
using ContactAppNLayer.Models.Entities;
using ContactAppNLayer.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace ContactAppNLayer.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly IConfiguration _config;
        private readonly ILogger<AuthService> _logger;


        public AuthService(IUserRepository userRepo, IConfiguration config,ILogger<AuthService> logger)
        {
            _logger = logger;   
            _userRepo = userRepo;
            _config = config;
        }

        public async Task<LoginResponse?> LoginAsync(LoginRequest request)
        {
           _logger.LogInformation("Login attempt for (AuthService) username: {Username} at {Time}", request.Username, DateTime.UtcNow);
            try
            {
                var user = await _userRepo.GetUserByCredentialsAsync(request.Username, request.Password);
                if (user == null) {

                    _logger.LogWarning("Invalid login(AuthService) attempt. Username: {Username}", request.Username);
                    return null;

                }

                var claims = new[]
                 {
                  new Claim(ClaimTypes.Name, user.Username),
                  new Claim(ClaimTypes.Role, user.Role) // e.g. "Admin", "User"
             };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _config["Jwt:Issuer"],
                    audience: _config["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(15),
                    signingCredentials: creds
                );
                _logger.LogInformation("Login successful(AuthService) for user: {Username}, Role: {Role}", user.Username, user.Role);

                return new LoginResponse
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    Role = user.Role
                };
            }catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred (AuthService) during login for user: {Username}", request.Username);
                throw; // rethrow so controller can handle
            }
           
        }

        public async Task<bool> RegisterAsync(RegisterRequest request)
        {
            _logger.LogInformation("User registration attempt(AuthService) for username: {Username} at {Time}", request.Username, DateTime.UtcNow);

            try
            {
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Username = request.Username,
                    Password = request.Password,
                    Role = "User" // Default role "User"
                };

                var result= await _userRepo.RegisterUserAsync(user);

                if (result)
                {
                    _logger.LogInformation("User registered(AuthService) successfully. Username: {Username}, Id: {Id}", user.Username, user.Id);

                }
                else
                {
                    _logger.LogWarning("User registration(AuthService) failed for username: {Username}", request.Username);

                }

                return result;

            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred during registration(AuthService) for username: {Username}", request.Username);
                throw;
            }


           }

       
    }
}