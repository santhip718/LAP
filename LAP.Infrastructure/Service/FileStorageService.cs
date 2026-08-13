using LAP.Application.Constant;
using LAP.Application.Interface;
using LAP.Application.Interface.IService;
using LAP.Application.Options;
using Microsoft.Extensions.Options;

namespace LAP.Infrastructure.Services;

/// <summary>
/// Implementation of <see cref="IFileStorageService"/> that saves files to the local file system.
/// </summary>
public class FileStorageService : IFileStorageService
{
    private readonly FileStorageOptions _options;
    private readonly ICustomLogger<FileStorageService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileStorageService"/> class.
    /// </summary>
    /// <param name="options">The file storage configuration options.</param>
    /// <param name="logger">The application logger.</param>
    public FileStorageService(
        IOptions<FileStorageOptions> options,
        ICustomLogger<FileStorageService> logger
    )
    {
        _options = options.Value;
        _logger = logger;
    }

    /// <summary>Gets the root storage directory, creating it if it does not exist.</summary>
    /// <returns>The fully qualified root storage path.</returns>
    private string GetStorageRoot()
    {
        string root = _options.StorageRoot;

        if (!string.IsNullOrEmpty(root) && !Path.IsPathRooted(root))
        {
            root = Path.Combine(Directory.GetCurrentDirectory(), root);
        }

        if (!string.IsNullOrEmpty(root) && !Directory.Exists(root))
        {
            Directory.CreateDirectory(root);
        }

        return root;
    }

    /// <summary>Resolves a relative path to an absolute physical path within the storage root.</summary>
    /// <param name="relativePath">The relative file path.</param>
    /// <returns>The full physical path.</returns>
    private string ResolvePhysicalPath(string relativePath)
    {
        return Path.Combine(GetStorageRoot(), relativePath);
    }

    /// <summary>Builds the relative storage path for a user's profile image.</summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="extension">The file extension including the dot.</param>
    /// <returns>The relative profile image path.</returns>
    private string GetProfileImagePath(Guid userId, string extension)
    {
        return $"profile/{userId}/image{extension}";
    }

    /// <summary>
    /// Saves a file to the local file system.
    /// </summary>
    /// <param name="fileBytes">The byte content of the file to save.</param>
    /// <param name="fileName">The name or relative path of the file including extension.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The relative path where the file was saved.</returns>
    public async Task<string> SaveFileAsync(
        byte[] fileBytes,
        string fileName,
        CancellationToken cancellationToken = default
    )
    {
        string fullPath = ResolvePhysicalPath(fileName);
        _logger.LogDebug("Saving file to {FullPath}", fullPath);

        try
        {
            string? directoryPath = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            await File.WriteAllBytesAsync(fullPath, fileBytes, cancellationToken);
            _logger.LogDebug("File saved successfully: {FileName}", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save file {FileName} to {Path}", fileName, fullPath);
            throw;
        }

        return fileName;
    }

    /// <summary>
    /// Deletes a file from storage.
    /// </summary>
    /// <param name="filePath">The relative or full path of the file to delete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task DeleteFileAsync(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            return Task.CompletedTask;
        }

        string fullPath = ResolvePhysicalPath(filePath);
        _logger.LogDebug("Attempting to delete file at {FullPath}", fullPath);

        if (File.Exists(fullPath))
        {
            try
            {
                File.Delete(fullPath);
                _logger.LogDebug("File deleted successfully: {FilePath}", fullPath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete file {FilePath}", fullPath);
                throw;
            }
        }
        else
        {
            _logger.LogDebug("File not found for deletion: {FullPath}", fullPath);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Retrieves the absolute path of the question template file.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>
    /// The absolute path of the question template file if found;
    /// otherwise <see langword="null"/>.
    /// </returns>
    public Task<string?> GetQuestionTemplateFilePathAsync(
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Resolving question template file path.");

        string root = _options.QuestionTemplatePath;

        if (string.IsNullOrEmpty(root))
        {
            _logger.LogError("QuestionTemplatePath is not configured.");
            return Task.FromResult<string?>(null);
        }

        string filePath = Path.Combine(root, CommonConstants.QuestionTemplateFileName);

        if (File.Exists(filePath))
        {
            _logger.LogDebug("Question template found at {FilePath}", filePath);
            return Task.FromResult<string?>(filePath);
        }

        _logger.LogError("Question template not found at {FilePath}", filePath);
        return Task.FromResult<string?>(null);
    }

    /// <summary>
    /// Retrieves a file from storage and returns its base64 string representation with the appropriate mime type header asynchronously.
    /// </summary>
    /// <param name="fileName">The file name or path.</param>
    /// <returns>The base64 encoded file content, or null if the file does not exist.</returns>
    public async Task<string?> GetBase64Async(string? fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            return null;
        }

        string fullPath = ResolvePhysicalPath(fileName);

        if (!File.Exists(fullPath))
        {
            _logger.LogError("File not found at {FullPath} for base64 conversion.", fullPath);
            return null;
        }

        try
        {
            byte[] fileBytes = await File.ReadAllBytesAsync(fullPath);
            string extension = Path.GetExtension(fullPath).ToLowerInvariant();
            string mimeType = CommonConstants.MIME_TYPE_MAP.TryGetValue(extension, out string? mapped)
                ? mapped
                : "image/jpeg";
            return $"data:{mimeType};base64,{Convert.ToBase64String(fileBytes)}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to read file as base64: {FileName}", fileName);
            return null;
        }
    }

    /// <summary>
    /// Retrieves the profile image for a user as a base64 encoded data URL, or null if it doesn't exist.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>The base64 encoded profile image, or null.</returns>
    public async Task<string?> GetUserProfileImageAsync(string userId)
    {
        foreach (string ext in CommonConstants.ALLOWED_IMAGE_EXTENSIONS)
        {
            string relativePath = $"profile/{userId}/image{ext}";
            string fullPath = ResolvePhysicalPath(relativePath);
            if (File.Exists(fullPath))
            {
                return await GetBase64Async(relativePath);
            }
        }
        return null;
    }

    /// <summary>
    /// Deletes any existing profile image for the user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    public async Task DeleteUserProfileImageAsync(string userId)
    {
        foreach (string ext in CommonConstants.ALLOWED_IMAGE_EXTENSIONS)
        {
            string relativePath = $"profile/{userId}/image{ext}";
            string fullPath = ResolvePhysicalPath(relativePath);
            if (File.Exists(fullPath))
            {
                await DeleteFileAsync(relativePath);
            }
        }
    }
}
