using API.DTOs.Auth;
using Application.Common;
using Infrastructure.Persistence;
using Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Application.Auth;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly JwtSettings _jwtSettings;

    public AuthService(
        ApplicationDbContext context, 
        IJwtTokenService jwtTokenService,
        IOptions<JwtSettings> jwtSettings)
    {
        _context = context;
        _jwtTokenService = jwtTokenService;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<ServiceResult<LoginResponse>> LoginAsync(LoginRequest request)
    {
        // Find user by email
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null)
        {
            return ServiceResult<LoginResponse>.Fail("Invalid email");
        }

        // Check if user is active
        if (!user.Active)
        {
            return ServiceResult<LoginResponse>.Fail("User account is inactive");
        }

        // Verify password
        if (!PasswordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            return ServiceResult<LoginResponse>.Fail("Invalid password");
        }

        // Generate JWT token
        var token = _jwtTokenService.GenerateToken(user);
        var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes);

        var response = new LoginResponse
        {
            Token = token,
            Email = user.Email,
            Role = user.Role.ToString(),
            ExpiresAt = expiresAt
        };

        return ServiceResult<LoginResponse>.Ok(response);
    }
}

