using Domain.Enums;

namespace Domain.Entities;

public class User : AuditableEntity
{
    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public Role Role { get; set; } = Role.Customer;

    public bool Active { get; set; } = true;

    // Navigation
    public Customer? Customer { get; set; }
}
