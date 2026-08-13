using Microsoft.AspNetCore.Http;

namespace LAP.Application.Interface.IService;

/// <summary>Defines file upload operations including validation and storage delegation.</summary>
public interface IFileService
{
    /// <summary>Validates the file media type and saves it to persistent storage.</summary>
    /// <param name="file">The uploaded file to validate and save.</param>
    /// <param name="entityId">A unique identifier used to name the stored file.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The full storage path where the file was saved.</returns>
    Task<string> SaveFileAsync(
        IFormFile file,
        string entityId,
        CancellationToken cancellationToken = default
    );
}
