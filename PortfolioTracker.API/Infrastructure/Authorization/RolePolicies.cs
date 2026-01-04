namespace Infrastructure.Authorization;

/// <summary>
/// Defines authorization policy names for role-based access control
/// </summary>
public static class RolePolicies
{
    // Role-based policies
    public const string CustomerOnly = "CustomerOnly";
    public const string AdminOnly = "AdminOnly";
    public const string SuperAdminOnly = "SuperAdminOnly";
    
    // Hierarchical policies (includes higher roles)
    public const string RequireCustomer = "RequireCustomer";      // Customer, Admin, SuperAdmin
    public const string RequireAdmin = "RequireAdmin";            // Admin, SuperAdmin
    public const string RequireSuperAdmin = "RequireSuperAdmin";  // SuperAdmin only
}

