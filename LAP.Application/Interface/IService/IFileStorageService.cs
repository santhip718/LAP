namespace LAP.Application.Interface.IService;

/// <summary>
/// Provides file storage operations for saving and retrieving uploaded files.
/// </summary>
public interface IFileStorageService
{
    /// <summary>
    /// Saves a file and returns the storage path.
    /// </summary>
    /// <param name="fileBytes">The file content as a byte array.</param>
    /// <param name="fileName">The name to save the file as.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>The relative or absolute path to the saved file.</returns>
    Task<string> SaveFileAsync(
        byte[] fileBytes,
        string fileName,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Deletes a file from storage.
    /// </summary>
    /// <param name="filePath">The path of the file to delete.</param>
    Task DeleteFileAsync(string filePath);

    /// <summary>
    /// Retrieves the absolute path of the question template file if it exists.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The file path if found; otherwise, <see langword="null"/>.</returns>
    Task<string?> GetQuestionTemplateFilePathAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a file from storage and returns its base64 string representation with the appropriate mime type header asynchronously.
    /// </summary>
    /// <param name="fileName">The file name or path.</param>
    /// <returns>The base64 encoded file content, or null if the file does not exist.</returns>
    Task<string?> GetBase64Async(string? fileName);

    /// <summary>
    /// Retrieves the profile image for a user as a base64 encoded data URL, or null if it doesn't exist.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>The base64 encoded profile image, or null.</returns>
    Task<string?> GetUserProfileImageAsync(string userId);

    /// <summary>
    /// Deletes any existing profile image for the user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    Task DeleteUserProfileImageAsync(string userId);
}
