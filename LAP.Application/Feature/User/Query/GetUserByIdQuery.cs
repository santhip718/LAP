using AutoMapper;
using FluentValidation;
using LAP.Application.DTO.User;
using LAP.Application.Interface;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using LAP.Shared.Exceptions;
using MediatR;

namespace LAP.Application.Feature.User.Query;

/// <summary>Query to retrieve a user by ID with full details.</summary>
/// <param name="Id">The unique identifier of the user.</param>
public record GetUserByIdQuery(Guid Id) : IRequest<UserEnrichedDto>;

/// <summary>Validates <see cref="GetUserByIdQuery"/> rules before processing.</summary>
public class GetUserByIdQueryValidator : AbstractValidator<GetUserByIdQuery>
{
    /// <summary>
    /// Initializes validation rules for the <see cref="GetUserByIdQuery"/>.
    /// </summary>
    public GetUserByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("User ID is required");
    }
}

/// <summary>Handles retrieval of a single user by ID and maps to detail DTO.</summary>
public class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, UserEnrichedDto>
{
    private readonly IUserService _userService;
    private readonly IFileStorageService _fileStorageService;
    private readonly ICustomLogger<GetUserByIdHandler> _logger;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetUserByIdHandler"/> class.
    /// </summary>
    /// <param name="userService">Service used to retrieve user data.</param>
    /// <param name="fileStorageService">Service used for file operations.</param>
    /// <param name="logger">Custom application logger.</param>
    /// <param name="mapper">AutoMapper instance for mapping user entities to DTOs.</param>
    public GetUserByIdHandler(
        IUserService userService,
        IFileStorageService fileStorageService,
        ICustomLogger<GetUserByIdHandler> logger,
        IMapper mapper
    )
    {
        _userService = userService;
        _fileStorageService = fileStorageService;
        _logger = logger;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the user retrieval request and returns the user details with enrollment information.
    /// </summary>
    /// <param name="request">The <see cref="GetUserByIdQuery"/> containing the user identifier.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A <see cref="UserEnrichedDto"/> containing the user details and enrolled courses.</returns>
    public async Task<UserEnrichedDto> Handle(
        GetUserByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        _logger.LogInfo("Fetching user by id {UserId}.", request.Id);

        Domain.Entity.User? user = await _userService.GetUserByIdWithEnrollmentsAsync(
            request.Id,
            cancellationToken
        );

        if (user is null)
        {
            _logger.LogError(
                "User retrieval failed because user {UserId} was not found.",
                request.Id
            );

            throw new NotFoundException("User not found", $"No user found with id {request.Id}");
        }

        UserEnrichedDto dto = _mapper.Map<UserEnrichedDto>(user);
        dto.ProfileImage = await _fileStorageService.GetUserProfileImageAsync(user.Id.ToString());
        return dto;
    }
}
