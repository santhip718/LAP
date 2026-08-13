using FluentValidation;
using LAP.Application.DTO.Common;
using LAP.Application.Interface;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using LAP.Shared.Exceptions;
using MediatR;

namespace LAP.Application.Feature.User.Command;

/// <summary>Command to delete a user.</summary>
/// <param name="Id">The unique identifier of the user to delete.</param>
public record DeleteUserCommand(Guid Id) : IRequest<SuccessResponse>;

/// <summary>Validates <see cref="DeleteUserCommand"/> rules before processing.</summary>
public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    /// <summary>
    /// Initializes validation rules for the <see cref="DeleteUserCommand"/>.
    /// </summary>
    public DeleteUserCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("User ID is required");
    }
}

/// <summary>Handles deletion of a user.</summary>
public class DeleteUserHandler : IRequestHandler<DeleteUserCommand, SuccessResponse>
{
    private readonly IUserService _userService;
    private readonly ICustomLogger<DeleteUserHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteUserHandler"/> class.
    /// </summary>
    /// <param name="userService">Service used to delete user data.</param>
    /// <param name="logger">Custom application logger.</param>
    public DeleteUserHandler(IUserService userService, ICustomLogger<DeleteUserHandler> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Handles the user deletion request.
    /// </summary>
    /// <param name="request">The <see cref="DeleteUserCommand"/> containing the user identifier.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A <see cref="SuccessResponse"/> containing the deleted user ID and a confirmation message.</returns>
    public async Task<SuccessResponse> Handle(
        DeleteUserCommand request,
        CancellationToken cancellationToken
    )
    {
        _logger.LogInfo("Deleting user {UserId}.", request.Id);

        int affected = await _userService.DeleteUserAsync(request.Id, cancellationToken);

        if (affected == 0)
        {
            _logger.LogError(
                "User deletion failed because user {UserId} was not found.",
                request.Id
            );

            throw new NotFoundException("User not found", $"No user found with id {request.Id}");
        }

        _logger.LogInfo("User {UserId} deleted successfully.", request.Id);

        return new SuccessResponse { Id = request.Id, Message = "User deleted successfully" };
    }
}
