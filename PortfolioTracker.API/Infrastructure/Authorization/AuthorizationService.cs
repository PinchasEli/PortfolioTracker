using System.Security.Claims;
using Domain.Enums;

namespace Infrastructure.Authorization;


public class AuthorizationService : IAuthorizationService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthorizationService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public Guid? GetCurrentUserId()
    {
        var userIdClaim = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
    }

    public string? GetCurrentUserEmail()
    {
        return User?.FindFirst(ClaimTypes.Email)?.Value;
    }

    public Role? GetCurrentUserRole()
    {
        var roleClaim = User?.FindFirst(ClaimTypes.Role)?.Value;
        return Enum.TryParse<Role>(roleClaim, out var role) ? role : null;
    }

    public bool HasRole(Role role)
    {
        var currentRole = GetCurrentUserRole();
        return currentRole.HasValue && currentRole.Value == role;
    }

    public bool HasAnyRole(params Role[] roles)
    {
        var currentRole = GetCurrentUserRole();
        return currentRole.HasValue && roles.Contains(currentRole.Value);
    }

    public bool IsAtLeastRole(Role minimumRole)
    {
        var currentRole = GetCurrentUserRole();
        if (!currentRole.HasValue) return false;

        // Hierarchical check: higher role number = more privileges
        return currentRole.Value >= minimumRole;
    }

    public bool IsCustomer()
    {
        return HasRole(Role.Customer);
    }

    public bool IsAdmin()
    {
        return HasRole(Role.Admin);
    }

    public bool IsSuperAdmin()
    {
        return HasRole(Role.SuperAdmin);
    }
}

