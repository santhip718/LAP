using System.IO;
using LAP.Application.Constant;
using LAP.Application.Interface;
using LAP.Application.Interface.IService;
using LAP.Shared.Exceptions;
using Microsoft.AspNetCore.Http;

namespace LAP.Application.Service;

/// <summary>
/// Handles file upload operations including media type validation and delegated storage.
/// </summary>
public class FileService : IFileService
{
    private readonly IFileStorageService _fileStorage;
    private readonly ICustomLogger<FileService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileService"/> class.
    /// </summary>
    /// <param name="fileStorage">The underlying file storage service used to persist file bytes.</param>
    /// <param name="logger">The custom logger for structured logging within the service.</param>
    public FileService(IFileStorageService fileStorage, ICustomLogger<FileService> logger)
    {
        _fileStorage = fileStorage;
        _logger = logger;
    }

    /// <summary>
    /// Validates the file media type, saves it to storage, and returns the saved path.
    /// </summary>
    /// <param name="file">The uploaded file to validate and save.</param>
    /// <param name="entityId">A unique identifier (e.g. course or content id) used to name the stored file.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The full storage path where the file was saved.</returns>
    public async Task<string> SaveFileAsync(
        IFormFile file,
        string entityId,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Saving file for entity {EntityId}.", entityId);

        ValidateMediaType(file);

        byte[] fileBytes = await ReadFileBytesAsync(file, cancellationToken);
        string extension = Path.GetExtension(file.FileName);
        string relativePath = string.Equals(
            extension,
            CommonConstants.PDF_EXTENSION,
            StringComparison.OrdinalIgnoreCase
        )
            ? string.Format(CommonConstants.COURSE_CONTENT_PATH_FORMAT, entityId, extension)
            : string.Format(CommonConstants.COURSE_THUMBNAIL_PATH_FORMAT, entityId, extension);

        string savedPath = await _fileStorage.SaveFileAsync(
            fileBytes,
            relativePath,
            cancellationToken
        );

        _logger.LogDebug(
            "File saved successfully for entity {EntityId} at path {SavedPath}.",
            entityId,
            savedPath
        );

        return savedPath;
    }

    /// <summary>
    /// Validates the file media type.
    /// </summary>
    /// <param name="file">The uploaded file to validate.</param>
    private void ValidateMediaType(IFormFile file)
    {
        _logger.LogDebug("Validating media type for file {FileName}.", file.FileName);

        string extension = Path.GetExtension(file.FileName);

        if (
            string.Equals(
                extension,
                CommonConstants.PDF_EXTENSION,
                StringComparison.OrdinalIgnoreCase
            )
        )
        {
            if (!CommonConstants.ALLOWED_DOCUMENT_TYPES.Contains(file.ContentType))
            {
                _logger.LogError(
                    "Invalid file type for {FileName} with content type {ContentType}.",
                    file.FileName,
                    file.ContentType
                );
                throw new BadRequestException(
                    "Invalid file type",
                    $"PDF files must have content type 'application/pdf', but received '{file.ContentType}'."
                );
            }
        }
        else
        {
            if (!CommonConstants.ALLOWED_IMAGE_TYPES.Contains(file.ContentType))
            {
                _logger.LogError(
                    "Invalid file type for {FileName} with content type {ContentType}.",
                    file.FileName,
                    file.ContentType
                );
                throw new BadRequestException(
                    "Invalid file type",
                    $"Image files must have one of the following content types: {string.Join(", ", CommonConstants.ALLOWED_IMAGE_TYPES)}. Received '{file.ContentType}'."
                );
            }
        }
    }

    /// <summary>
    /// Reads the file bytes asynchronously.
    /// </summary>
    /// <param name="file">The uploaded file to read.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The file bytes as a byte array.</returns>
    private async Task<byte[]> ReadFileBytesAsync(
        IFormFile file,
        CancellationToken cancellationToken
    )
    {
        _logger.LogDebug("Reading bytes for file {FileName}.", file.FileName);

        await using MemoryStream ms = new();
        await file.CopyToAsync(ms, cancellationToken);
        return ms.ToArray();
    }
}
