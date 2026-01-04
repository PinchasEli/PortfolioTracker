using System.Security.Claims;
using Domain.Enums;

namespace Infrastructure.Authorization;

/// <summary>
/// Service for checking user authorization and roles
/// </summary>
public interface IAuthorizationService
{
    /// <summary>
    /// Get the current user's ID from claims
    /// </summary>
    Guid? GetCurrentUserId();
    
    /// <summary>
    /// Get the current user's email from claims
    /// </summary>
    string? GetCurrentUserEmail();
    
    /// <summary>
    /// Get the current user's role from claims
    /// </summary>
    Role? GetCurrentUserRole();
    
    /// <summary>
    /// Check if current user has a specific role
    /// </summary>
    bool HasRole(Role role);
    
    /// <summary>
    /// Check if current user has any of the specified roles
    /// </summary>
    bool HasAnyRole(params Role[] roles);
    
    /// <summary>
    /// Check if current user is at least the specified role (hierarchical)
    /// </summary>
    bool IsAtLeastRole(Role minimumRole);
    
    /// <summary>
    /// Check if current user is Customer
    /// </summary>
    bool IsCustomer();
    
    /// <summary>
    /// Check if current user is Admin
    /// </summary>
    bool IsAdmin();
    
    /// <summary>
    /// Check if current user is SuperAdmin
    /// </summary>
    bool IsSuperAdmin();
}
