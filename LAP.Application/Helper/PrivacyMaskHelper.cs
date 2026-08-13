/// <summary>
/// Provides helper methods for masking personally identifiable information (PII).
/// </summary>
public static class PrivacyMaskHelper
{
    /// <summary>
    /// Masks the domain portion of an email address to protect sensitive information.
    /// </summary>
    /// <param name="email">The email address to mask.</param>
    /// <returns>
    /// The masked email address with the domain replaced by <c>****</c>.
    /// Returns the original value if the input is null, empty, whitespace, or not a valid email format.
    /// </returns>
    public static string MaskEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            return email;

        var parts = email.Split('@');
        return $"{parts[0]}@****";
    }
}