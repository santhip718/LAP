using System.IO;
using FluentValidation;
using LAP.Application.Constant;
using LAP.Application.DTO.Common;
using LAP.Application.Interface;
using LAP.Application.Interface.IContext;
using LAP.Application.Interface.IRepository;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using LAP.Shared.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace LAP.Application.Feature.ProfileImage.Command;

/// <summary>
/// Command to upload or update the authenticated user's profile image.
/// </summary>
/// <param name="File">The profile image file to upload.</param>
public record UploadProfileImageCommand(IFormFile File) : IRequest<SuccessResponse>;

/// <summary>
/// Validates the <see cref="UploadProfileImageCommand"/> request data.
/// </summary>
public class UploadProfileImageValidator : AbstractValidator<UploadProfileImageCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UploadProfileImageValidator"/> class.
    /// </summary>
    public UploadProfileImageValidator()
    {
        RuleFor(x => x.File).NotNull().WithMessage("Profile image file is required.");

        When(
            x => x.File is not null,
            () =>
            {
                RuleFor(x => x.File.Length)
                    .GreaterThan(0)
                    .WithMessage("Profile image file cannot be empty.");

                RuleFor(x => x.File.Length)
                    .LessThanOrEqualTo(CommonConstants.MAX_PROFILE_IMAGE_SIZE)
                    .WithMessage($"Profile image file size must not exceed 5 MB.");

                RuleFor(x => x.File.FileName)
                    .Must(fileName =>
                    {
                        string extension = Path.GetExtension(fileName).ToLowerInvariant();
                        return CommonConstants.ALLOWED_IMAGE_EXTENSIONS.Contains(extension);
                    })
                    .WithMessage(
                        $"Profile image must be one of the following formats: {string.Join(", ", CommonConstants.ALLOWED_IMAGE_EXTENSIONS)}."
                    );
            }
        );
    }
}

/// <summary>
/// Handles the upload of a user's profile image.
/// </summary>
public class UploadProfileImageCommandHandler
    : IRequestHandler<UploadProfileImageCommand, SuccessResponse>
{
    private readonly IRequestContext _requestContext;
    private readonly IUserService _userService;
    private readonly IFileStorageService _fileStorageService;
    private readonly ICustomLogger<UploadProfileImageCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UploadProfileImageCommandHandler"/> class.
    /// </summary>
    /// <param name="requestContext">The current request context.</param>
    /// <param name="userService">The user service.</param>
    /// <param name="fileStorageService">The file storage service.</param>
    /// <param name="logger">The custom logger.</param>
    public UploadProfileImageCommandHandler(
        IRequestContext requestContext,
        IUserService userService,
        IFileStorageService fileStorageService,
        ICustomLogger<UploadProfileImageCommandHandler> logger
    )
    {
        _requestContext = requestContext;
        _userService = userService;
        _fileStorageService = fileStorageService;
        _logger = logger;
    }

    /// <summary>
    /// Uploads or updates the authenticated user's profile image.
    /// </summary>
    /// <param name="request">The profile image upload request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>
    /// Returns a success response confirming the upload.
    /// </returns>
    public async Task<SuccessResponse> Handle(
        UploadProfileImageCommand request,
        CancellationToken cancellationToken
    )
    {
        Guid userId = _requestContext.UserId!.Value;

        _logger.LogInfo("Profile image upload initiated for user {UserId}.", userId);

        (LAP.Domain.Entity.User user, string extension) = await ValidateAndGetUserAsync(
            userId,
            request.File,
            cancellationToken
        );

        await UploadAndPersistImageAsync(user, request.File, extension, cancellationToken);

        _logger.LogInfo("Profile image upload completed for user {UserId}.", userId);

        return new SuccessResponse { Id = userId, Message = "Profile image uploaded successfully" };
    }

    /// <summary>
    /// Validates the user exists and the file conforms to standard extension and size limits.
    /// </summary>
    private async Task<(LAP.Domain.Entity.User user, string extension)> ValidateAndGetUserAsync(
        Guid userId,
        IFormFile file,
        CancellationToken cancellationToken
    )
    {
        _logger.LogInfo("Validating user and profile image file for user {UserId}.", userId);

        LAP.Domain.Entity.User? user = await _userService.GetUserByIdWithPersonAsync(
            userId,
            cancellationToken
        );

        string extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!IsExtensionAllowed(extension))
        {
            _logger.LogError(
                "Invalid profile image upload attempt for user {UserId}. Extension: {Extension}",
                userId,
                extension
            );

            throw new BadRequestException(
                "Invalid file type",
                "Profile image must be one of the following formats: .jpg, .jpeg, .png, .webp."
            );
        }

        if (file.Length > CommonConstants.MAX_PROFILE_IMAGE_SIZE)
        {
            _logger.LogError(
                "Profile image upload failed for user {UserId}. File size {Length} bytes exceeds limit.",
                userId,
                file.Length
            );

            throw new BadRequestException(
                "File too large",
                "Profile image file size must not exceed 5 MB."
            );
        }

        _logger.LogInfo("Profile image validation succeeded for user {UserId}.", userId);

        return (user, extension);
    }

    /// <summary>
    /// Deletes the old profile image, saves the new image file to the configured directory.
    /// </summary>
    private async Task UploadAndPersistImageAsync(
        LAP.Domain.Entity.User user,
        IFormFile file,
        string extension,
        CancellationToken cancellationToken
    )
    {
        _logger.LogInfo("Deleting old profile image if exists for user {UserId}", user.Id);
        await _fileStorageService.DeleteUserProfileImageAsync(user.Id.ToString());

        await using MemoryStream memoryStream = new();
        await file.CopyToAsync(memoryStream, cancellationToken);
        byte[] fileBytes = memoryStream.ToArray();

        string relativePath = Path.Combine("profile", user.Id.ToString(), $"image{extension}");
        _logger.LogInfo(
            "Saving new profile image for user {UserId}. Relative path: {RelativePath}",
            user.Id,
            relativePath
        );
        await _fileStorageService.SaveFileAsync(fileBytes, relativePath, cancellationToken);

        _logger.LogInfo(
            "Successfully saved new profile image in storage for user {UserId}",
            user.Id
        );
    }

    private static bool IsExtensionAllowed(string extension)
    {
        return CommonConstants.ALLOWED_IMAGE_EXTENSIONS.Contains(extension);
    }
}
