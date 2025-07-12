using Microsoft.AspNetCore.Mvc;
using ContactAppNLayer.Services.Interfaces;
using ContactAppNLayer.Models.DTOs;

namespace ContactAppNLayer.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        //  Login API

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);
            // return result == null ? Unauthorized("Invalid credentials") : Ok(result);

            if (result == null)
            {
                return Unauthorized(new
                {
                    status = 401,
                    message="Invalid Username and password"
                });
            }
            else
            {
                return Ok(new
                {
                    status=200,
                    message="Login successfully",
                    data=result
                });
            }
        }
        //  Signup API
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request);
            //  return result ? Ok("User registered successfully!") : Conflict("Username already exists");
            if (result)
            {
                return Ok(new
                {
                    status = 200,
                    message = "User registered successfully!"
                });
            }
            else
            {
                return Conflict(new
                {
                    status = 409,
                    message = "Username already exists"
                });
            }
        }
    }
}











