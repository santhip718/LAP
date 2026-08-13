using Asp.Versioning;
using LAP.API.Authorization;
using LAP.Application.DTO.Auth;
using LAP.Application.DTO.Common;
using LAP.Application.DTO.Paginated;
using LAP.Application.DTO.User;
using LAP.Application.Feature.ProfileImage.Command;
using LAP.Application.Feature.User.Command;
using LAP.Application.Feature.User.Query;
using LAP.Application.Interface;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LAP.API.Controller;

/// <summary>
/// Handles user management operations, including CRUD, profile retrieval, and password reset.
/// </summary>
[Route("api/v1/user")]
[Authorize]
public class UserController : BaseController
{
    private readonly IMediator _mediator;
    private readonly ICustomLogger<UserController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserController"/> class.
    /// </summary>
    /// <param name="mediator">Mediator for dispatching commands and queries.</param>
    /// <param name="logger">Custom application logger.</param>
    public UserController(IMediator mediator, ICustomLogger<UserController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Uploads or updates the authenticated user's profile image.
    /// </summary>
    /// <param name="file">The profile image file to upload.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>Success confirmation of the profile image upload.</returns>
    [HttpPost("profile-image")]
    [Authorize]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "Profile image uploaded successfully.",
        typeof(SuccessResponse)
    )]
    [SwaggerResponse(
        StatusCodes.Status400BadRequest,
        "Invalid file type or size.",
        typeof(ErrorResponse)
    )]
    [SwaggerResponse(
        StatusCodes.Status401Unauthorized,
        "User is not authenticated.",
        typeof(ErrorResponse)
    )]
    [SwaggerResponse(StatusCodes.Status404NotFound, "User not found.", typeof(ErrorResponse))]
    [SwaggerResponse(
        StatusCodes.Status500InternalServerError,
        "Internal server error.",
        typeof(ErrorResponse)
    )]
    public async Task<IActionResult> UploadProfileImage(
        IFormFile file,
        CancellationToken cancellationToken
    )
    {
        _logger.LogDebug("Received profile image upload request.");

        SuccessResponse result = await _mediator.Send(
            new UploadProfileImageCommand(file),
            cancellationToken
        );

        _logger.LogDebug("Profile image upload request completed successfully.");
        return Ok(result);
    }

    /// <summary>
    /// Retrieves all active users with summary details for administrators.
    /// </summary>
    /// <param name="page">The page number to retrieve.</param>
    /// <param name="pageSize">The number of users to include on each page.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A paginated list of user summaries.</returns>
    [HttpGet]
    [FeatureAuthorize("VIEW_USER")]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "Users retrieved successfully.",
        typeof(PaginatedUsersDto)
    )]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication required.")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Insufficient permissions.")]
    public async Task<IActionResult> GetAllUser(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug(
            "Fetching users for page {Page} and page size {PageSize}.",
            page,
            pageSize
        );

        PaginatedUsersDto result = await _mediator.Send(
            new GetUserQuery(page, pageSize),
            cancellationToken
        );

        _logger.LogDebug("Retrieved {Total} users for page {Page}.", result.Total, result.Page);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a user by ID with full details.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>The user detail data for the requested user.</returns>
    [HttpGet("{id}")]
    [FeatureAuthorize("VIEW_USER")]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "User retrieved successfully.",
        typeof(UserEnrichedDto)
    )]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication required.")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Insufficient permissions.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "User not found.")]
    public async Task<IActionResult> GetUserById(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Fetching user {UserId}.", id);

        UserEnrichedDto result = await _mediator.Send(new GetUserByIdQuery(id), cancellationToken);

        _logger.LogDebug("Retrieved user {UserId}.", id);
        return Ok(result);
    }

    /// <summary>
    /// Updates an existing user's profile details.
    /// </summary>
    /// <param name="id">The unique identifier of the user to update.</param>
    /// <param name="dto">The updated user details.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>The updated user details.</returns>
    [HttpPut("{id}")]
    [FeatureAuthorize("MANAGE_USER")]
    [SwaggerResponse(StatusCodes.Status200OK, "User updated successfully.", typeof(UserDetailDto))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Validation failed.")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication required.")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Insufficient permissions.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "User not found.")]
    public async Task<IActionResult> UpdateUser(
        Guid id,
        [FromBody] UpdateUserRequestDto dto,
        CancellationToken cancellationToken
    )
    {
        _logger.LogDebug("Updating user {UserId}.", id);

        UserDetailDto result = await _mediator.Send(
            new UpdateUserCommand(id, dto),
            cancellationToken
        );

        _logger.LogDebug("Updated user {UserId}.", id);
        return Ok(result);
    }

    /// <summary>
    /// Soft-deletes (deactivates) a user by ID.
    /// </summary>
    /// <param name="id">The unique identifier of the user to deactivate.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A success response indicating the user was deactivated.</returns>
    [HttpDelete("{id}")]
    [FeatureAuthorize("MANAGE_USER")]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "User deleted successfully.",
        typeof(SuccessResponse)
    )]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication required.")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Insufficient permissions.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "User not found.")]
    public async Task<IActionResult> DeleteUser(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Deleting user {UserId}.", id);

        SuccessResponse result = await _mediator.Send(new DeleteUserCommand(id), cancellationToken);

        _logger.LogDebug("Deleted user {UserId}.", id);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves the authenticated user's own profile with enrollment statistics.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>The user profile data with enrollment counts.</returns>
    [HttpGet("{id}/profile")]
    [FeatureAuthorize("VIEW_PROFILE")]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "User profile retrieved successfully.",
        typeof(UserProfileDto)
    )]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication required.")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Insufficient permissions.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "User not found.")]
    public async Task<IActionResult> GetUserProfile(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Fetching profile for user {UserId}.", id);

        UserProfileDto result = await _mediator.Send(
            new GetUserProfileQuery(id),
            cancellationToken
        );

        _logger.LogDebug("Retrieved profile for user {UserId}.", id);
        return Ok(result);
    }
}
