using AutoMapper;
using FluentValidation;
using LAP.Application.DTO.User;
using LAP.Application.Interface;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using LAP.Shared.Exceptions;
using MediatR;

namespace LAP.Application.Feature.User.Query;

/// <summary>Query to retrieve the authenticated user's own profile with enrollment statistics.</summary>
/// <param name="Id">The unique identifier of the user.</param>
public record GetUserProfileQuery(Guid Id) : IRequest<UserProfileDto>;

/// <summary>Validates <see cref="GetUserProfileQuery"/> rules before processing.</summary>
public class GetUserProfileQueryValidator : AbstractValidator<GetUserProfileQuery>
{
    /// <summary>
    /// Initializes validation rules for the <see cref="GetUserProfileQuery"/>.
    /// </summary>
    public GetUserProfileQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("User ID is required");
    }
}

/// <summary>Handles retrieval of user profile including enrollment and completion counts.</summary>
public class GetUserProfileHandler : IRequestHandler<GetUserProfileQuery, UserProfileDto>
{
    private readonly IUserService _userService;
    private readonly IFileStorageService _fileStorageService;
    private readonly ICustomLogger<GetUserProfileHandler> _logger;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetUserProfileHandler"/> class.
    /// </summary>
    /// <param name="userService">Service used to retrieve user data.</param>
    /// <param name="fileStorageService">Service used for file operations.</param>
    /// <param name="logger">Custom application logger.</param>
    /// <param name="mapper">AutoMapper instance for mapping user entities to DTOs.</param>
    public GetUserProfileHandler(
        IUserService userService,
        IFileStorageService fileStorageService,
        ICustomLogger<GetUserProfileHandler> logger,
        IMapper mapper
    )
    {
        _userService = userService;
        _fileStorageService = fileStorageService;
        _logger = logger;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the user profile retrieval request and returns the profile with enrollment statistics.
    /// </summary>
    /// <param name="request">The <see cref="GetUserProfileQuery"/> containing the user identifier.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A <see cref="UserProfileDto"/> containing the user profile details and enrollment counts.</returns>
    public async Task<UserProfileDto> Handle(
        GetUserProfileQuery request,
        CancellationToken cancellationToken
    )
    {
        _logger.LogInfo("Fetching profile for user {UserId}.", request.Id);

        Domain.Entity.User? user = await _userService.GetUserByIdWithEnrollmentsAsync(
            request.Id,
            cancellationToken
        );

        if (user is null)
        {
            _logger.LogError(
                "Profile retrieval failed because user {UserId} was not found.",
                request.Id
            );

            throw new NotFoundException("User not found", $"No user found with id {request.Id}");
        }

        UserProfileDto dto = _mapper.Map<UserProfileDto>(user);
        dto.ProfileImage = await _fileStorageService.GetUserProfileImageAsync(user.Id.ToString());
        return dto;
    }
}
