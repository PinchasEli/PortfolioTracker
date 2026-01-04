using Microsoft.AspNetCore.Mvc;
using API.DTOs.Auth;
using API.Validators;
using Application.Auth;

namespace API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Login endpoint - Returns JWT token on successful authentication
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <returns>JWT token and user information</returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var validator = new LoginRequestValidator();
        var validationResult = validator.Validate(request);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var result = await _authService.LoginAsync(request);

        if (!result.Success)
            return Unauthorized(new { message = result.ErrorMessage });

        return Ok(result.Data);
    }
}

