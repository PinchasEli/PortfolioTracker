namespace Infrastructure.Security;

public static class PasswordHasher
{
    // Work factor: higher = more secure but slower (11-13 recommended for 2025)
    private const int WorkFactor = 12;

    /// <summary>
    /// Hashes a password using BCrypt with configurable work factor
    /// </summary>
    /// <param name="password">Plain text password</param>
    /// <returns>Hashed password (includes salt)</returns>
    public static string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be empty", nameof(password));

        return BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);
    }

    /// <summary>
    /// Verifies a password against a BCrypt hash
    /// </summary>
    /// <param name="password">Plain text password to verify</param>
    /// <param name="hash">BCrypt hash to verify against</param>
    /// <returns>True if password matches hash</returns>
    public static bool VerifyPassword(string password, string hash)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hash))
            return false;

        try
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
        catch
        {
            // Invalid hash format
            return false;
        }
    }

    /// <summary>
    /// Checks if a hash needs to be rehashed with current work factor
    /// </summary>
    /// <param name="hash">Existing hash</param>
    /// <returns>True if rehashing is recommended</returns>
    public static bool NeedsRehash(string hash)
    {
        try
        {
            return BCrypt.Net.BCrypt.PasswordNeedsRehash(hash, WorkFactor);
        }
        catch
        {
            return true;
        }
    }
}

