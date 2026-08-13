namespace LAP.Application.Helper;

/// <summary>
/// Provides helper methods for reference data processing.
/// </summary>
public static class ReferenceDataNormalizer
{
    /// <summary>
    /// Normalizes a reference set name for comparison.
    /// </summary>
    /// <param name="value">The value to normalize.</param>
    /// <returns>The normalized value.</returns>
    public static string Normalize(string value)
    {
        return value.Trim().Replace(" ", "").Replace("-", "").Replace("_", "").ToLowerInvariant();
    }
}
