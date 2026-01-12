using Domain.Enums;
using API.DTOs.Customers;

namespace API.DTOs.Users;

public class UserResponse
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public Role Role { get; set; }
    public bool Active { get; set; }

    public CustomerResponse? Customer { get; set; } = null;
}
