using Microsoft.AspNetCore.Authorization;
using Domain.Enums;
using System.Security.Claims;

namespace Infrastructure.Authorization;

/// <summary>
/// Authorization requirement for role-based access
/// </summary>
public class RoleRequirement : IAuthorizationRequirement
{
    public Role[] AllowedRoles { get; }
    public bool IsHierarchical { get; }

    public RoleRequirement(Role[] allowedRoles, bool isHierarchical = false)
    {
        AllowedRoles = allowedRoles;
        IsHierarchical = isHierarchical;
    }
}

/// <summary>
/// Handler for role-based authorization
/// </summary>
public class RoleRequirementHandler : AuthorizationHandler<RoleRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        RoleRequirement requirement)
    {
        // Get the role claim
        var roleClaim = context.User.FindFirst(ClaimTypes.Role);
        
        if (roleClaim == null)
        {
            return Task.CompletedTask; // Fail - no role claim
        }

        // Parse the role
        if (!Enum.TryParse<Role>(roleClaim.Value, out var userRole))
        {
            return Task.CompletedTask; // Fail - invalid role
        }

        // Check authorization
        if (requirement.IsHierarchical)
        {
            // Hierarchical: User must be at least the minimum required role
            var minimumRole = requirement.AllowedRoles.Min();
            if (userRole >= minimumRole)
            {
                context.Succeed(requirement);
            }
        }
        else
        {
            // Exact match: User must have one of the allowed roles
            if (requirement.AllowedRoles.Contains(userRole))
            {
                context.Succeed(requirement);
            }
        }

        return Task.CompletedTask;
    }
}

