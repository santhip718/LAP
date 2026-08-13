namespace LAP.Shared.Helpers;

/// <summary>
/// Helper class for managing user secrets, including password hashing and verification using BCrypt.
/// </summary>
public static class UserSecretHelper
{
    /// <summary>
    /// Hashes a plain-text password using the BCrypt algorithm.
    /// </summary>
    /// <param name="password">The plain-text password to hash.</param>
    /// <param name="salt">The generated salt used for hashing.</param>
    /// <returns>The hashed password.</returns>
    public static string HashPasswordBcrypt(string password, out string salt)
    {
        salt = BCrypt.Net.BCrypt.GenerateSalt();
        var hash = BCrypt.Net.BCrypt.HashPassword(password, salt);
        return hash;
    }

    /// <summary>
    /// Verifies a plain-text password against a hashed password.
    /// </summary>
    /// <param name="password">The plain-text password to verify.</param>
    /// <param name="hash">The hashed password to check against.</param>
    /// <returns>True if the password matches the hash; otherwise, false.</returns>
    public static bool VerifyPassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}
