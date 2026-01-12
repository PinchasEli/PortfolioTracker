using API.DTOs.Auth;
using API.DTOs.Customers;
using API.DTOs.Users;
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
            .Include(u => u.Customer)
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
            ExpiresAt = expiresAt,
            User = new UserResponse
            {
                Id = user.Id,
                Email = user.Email,
                Role = user.Role,
                Active = user.Active,
                Customer = user.Customer != null ? new CustomerResponse
                {
                    Id = user.Customer.Id,
                    FullName = user.Customer.FullName,
                    Email = user.Customer.User.Email,
                    Role = user.Role.ToString(),
                    Active = user.Active
                } : null
            }
        };

        return ServiceResult<LoginResponse>.Ok(response);
    }
}

