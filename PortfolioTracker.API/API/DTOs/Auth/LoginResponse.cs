using Domain.Enums;
using API.DTOs.Users;

namespace API.DTOs.Auth;


public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public UserResponse User { get; set; } = new UserResponse();
    public DateTime ExpiresAt { get; set; }
}

