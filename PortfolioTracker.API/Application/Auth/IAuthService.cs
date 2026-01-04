using API.DTOs.Auth;
using Application.Common;

namespace Application.Auth;

public interface IAuthService
{
    Task<ServiceResult<LoginResponse>> LoginAsync(LoginRequest request);
}

