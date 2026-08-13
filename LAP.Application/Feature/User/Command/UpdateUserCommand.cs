using AutoMapper;
using FluentValidation;
using LAP.Application.DTO.User;
using LAP.Application.Interface;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using LAP.Shared.Exceptions;
using MediatR;

namespace LAP.Application.Feature.User.Command;

/// <summary>Command to update an existing user's profile details.</summary>
/// <param name="Id">The unique identifier of the user to update.</param>
/// <param name="Dto">The updated user details.</param>
public record UpdateUserCommand(Guid Id, UpdateUserRequestDto Dto) : IRequest<UserDetailDto>;

/// <summary>Validates <see cref="UpdateUserCommand"/> rules before processing.</summary>
public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    /// <summary>
    /// Initializes validation rules for the <see cref="UpdateUserCommand"/>.
    /// </summary>
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("User ID is required");
        RuleFor(x => x.Dto.FullName)
            .NotEmpty()
            .WithMessage("Full name is required")
            .MaximumLength(100)
            .WithMessage("Full name cannot exceed 100 characters");
    }
}

/// <summary>Handles user profile update within a database transaction.</summary>
public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, UserDetailDto>
{
    private readonly IUserService _userService;
    private readonly IFileStorageService _fileStorageService;
    private readonly ITransactionService _transactionService;
    private readonly ICustomLogger<UpdateUserHandler> _logger;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateUserHandler"/> class.
    /// </summary>
    /// <param name="userService">Service used to retrieve and update user data.</param>
    /// <param name="fileStorageService">Service used for file operations.</param>
    /// <param name="transactionService">Service used to manage transactional operations.</param>
    /// <param name="logger">Custom application logger.</param>
    /// <param name="mapper">AutoMapper instance for mapping user entities to DTOs.</param>
    public UpdateUserHandler(
        IUserService userService,
        IFileStorageService fileStorageService,
        ITransactionService transactionService,
        ICustomLogger<UpdateUserHandler> logger,
        IMapper mapper
    )
    {
        _userService = userService;
        _fileStorageService = fileStorageService;
        _transactionService = transactionService;
        _logger = logger;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the user update request by applying changes and returning the updated user details.
    /// </summary>
    /// <param name="request">The <see cref="UpdateUserCommand"/> containing the user identifier and update data.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A <see cref="UserDetailDto"/> representing the updated user profile.</returns>
    public async Task<UserDetailDto> Handle(
        UpdateUserCommand request,
        CancellationToken cancellationToken
    )
    {
        _logger.LogInfo("Updating user {UserId}.", request.Id);

        Domain.Entity.User? user = await _userService.GetUserByIdWithDetailAsync(
            request.Id,
            cancellationToken
        );

        if (user is null)
        {
            _logger.LogError("User update failed because user {UserId} was not found.", request.Id);

            throw new NotFoundException("User not found", $"No user found with id {request.Id}");
        }

        user.Person.FullName = request.Dto.FullName;
        user.Person.MobileNumber = request.Dto.MobileNumber;
        user.Person.DesignationId = request.Dto.DesignationId;
        user.Person.GenderId = request.Dto.GenderId;

        _userService.UpdateUser(user);

        await _transactionService.SaveChangesAsync(cancellationToken);

        _logger.LogInfo("User {UserId} updated successfully.", request.Id);

        Domain.Entity.User? updatedUser = await _userService.GetUserByIdWithDetailAsync(
            request.Id,
            cancellationToken
        );

        UserDetailDto dto = _mapper.Map<UserDetailDto>(updatedUser);
        if (updatedUser != null)
        {
            dto.ProfileImage = await _fileStorageService.GetUserProfileImageAsync(
                updatedUser.Id.ToString()
            );
        }
        return dto;
    }
}
