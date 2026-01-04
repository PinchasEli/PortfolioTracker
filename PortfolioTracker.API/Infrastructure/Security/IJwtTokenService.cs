using Domain.Entities;

namespace Infrastructure.Security;

public interface IJwtTokenService
{
    string GenerateToken(User user);
}

