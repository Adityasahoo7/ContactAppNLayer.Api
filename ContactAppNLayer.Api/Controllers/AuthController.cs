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
        private readonly ILogger<AuthController> _logger;
        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        //  Login API

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            _logger.LogInformation("Login attempt for user: {Username} at {Time}", request.Username, DateTime.UtcNow);

            try
            {




                var result = await _authService.LoginAsync(request);
                // return result == null ? Unauthorized("Invalid credentials") : Ok(result);


                if (result == null)
                {
                    _logger.LogWarning("Invalid login for user: {Username} at {Time}", request.Username, DateTime.UtcNow);

                    return Unauthorized(new
                    {
                        status = 401,
                        message = "Invalid Username and password"
                    });
                }
                else
                {
                    _logger.LogInformation("Login successful for user: {Username} at {Time}", request.Username, DateTime.UtcNow);

                    return Ok(new
                    {
                        status = 200,
                        message = "Login successfully",
                        data = result
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during login for user: {Username} at {Time}", request.Username, DateTime.UtcNow);
                return StatusCode(500, new
                {
                    status = 500,
                    message = "An error occurred while processing login"
                });
            }
        }
        //  Signup API
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            _logger.LogInformation("Registration attempt for user: {Username} at {Time}", request.Username, DateTime.UtcNow);

            try
            {


                var result = await _authService.RegisterAsync(request);
                //  return result ? Ok("User registered successfully!") : Conflict("Username already exists");
                if (result)
                {
                    _logger.LogInformation("User registered successfully: {Username} at {Time}", request.Username, DateTime.UtcNow);

                    return Ok(new
                    {
                        status = 200,
                        message = "User registered successfully!"
                    });
                }
                else
                {
                    _logger.LogWarning("Registration failed - Username already exists: {Username} at {Time}", request.Username, DateTime.UtcNow);


                    return Conflict(new
                    {
                        status = 409,
                        message = "Username already exists"
                    });
                }

            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred during registration for user: {Username} at {Time}", request.Username, DateTime.UtcNow);
                return StatusCode(500, new
                {
                   status=500,
                   message="An Error Occure while Processing Registration "
                });

            }
        }
    }
}











