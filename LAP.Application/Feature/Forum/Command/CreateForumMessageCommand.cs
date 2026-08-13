using FluentValidation;
using LAP.Application.DTO.Common;
using LAP.Application.DTO.Forum;
using LAP.Application.Interface;
using LAP.Application.Interface.IContext;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using LAP.Shared.Exceptions;
using MediatR;

namespace LAP.Application.Feature.Forum.Command;

/// <summary>
/// Command for creating a new forum message in a course.
/// </summary>
/// <param name="CourseId">The identifier of the course where the message will be posted.</param>
/// <param name="Dto">The forum message creation request data transfer object.</param>
public record CreateForumMessageCommand(Guid CourseId, CreateForumMessageRequestDto Dto)
    : IRequest<SuccessResponse>;

/// <summary>
/// Validates the <see cref="CreateForumMessageCommand"/> before it is handled.
/// </summary>
public class CreateForumMessageCommandValidator : AbstractValidator<CreateForumMessageCommand>
{
    /// <summary>
    /// Initializes validation rules for the <see cref="CreateForumMessageCommand"/>.
    /// </summary>
    public CreateForumMessageCommandValidator()
    {
        RuleFor(x => x.CourseId).NotEmpty().WithMessage("Course ID is required");
        RuleFor(x => x.Dto.MessageText)
            .NotEmpty()
            .WithMessage("Message text is required")
            .MaximumLength(5000)
            .WithMessage("Message text cannot exceed 5000 characters");
    }
}

/// <summary>
/// Handles the <see cref="CreateForumMessageCommand"/> by creating a new forum message for the specified course.
/// </summary>
public class CreateForumMessageHandler : IRequestHandler<CreateForumMessageCommand, SuccessResponse>
{
    private readonly IForumService _forumService;
    private readonly ITransactionService _transactionService;
    private readonly IRequestContext _requestContext;
    private readonly ICustomLogger<CreateForumMessageHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateForumMessageHandler"/> class.
    /// </summary>
    /// <param name="forumService">Service used to manage forum operations.</param>
    /// <param name="transactionService">Service used to manage transactional operations.</param>
    /// <param name="requestContext">Context containing the current request user information.</param>
    /// <param name="logger">Custom application logger.</param>
    public CreateForumMessageHandler(
        IForumService forumService,
        ITransactionService transactionService,
        IRequestContext requestContext,
        ICustomLogger<CreateForumMessageHandler> logger
    )
    {
        _forumService = forumService;
        _transactionService = transactionService;
        _requestContext = requestContext;
        _logger = logger;
    }

    /// <summary>
    /// Handles the forum message creation request by validating the course, user, and message content before persisting.
    /// </summary>
    /// <param name="request">The <see cref="CreateForumMessageCommand"/> containing the course identifier and message data.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>
    /// A <see cref="SuccessResponse"/> containing the created message ID and a confirmation message.
    /// </returns>
    public async Task<SuccessResponse> Handle(
        CreateForumMessageCommand request,
        CancellationToken cancellationToken
    )
    {
        _logger.LogInfo("Creating forum message for course {CourseId}.", request.CourseId);

        bool courseExists = await _forumService.CourseExistsAsync(
            request.CourseId,
            cancellationToken
        );

        if (!courseExists)
        {
            _logger.LogError(
                "Forum message creation failed because course {CourseId} was not found.",
                request.CourseId
            );

            throw new NotFoundException(
                "Course not found",
                $"No active course exists with id {request.CourseId}."
            );
        }

        string? trimmed = request.Dto.MessageText?.Trim();

        Guid? userId = _requestContext.UserId;

        ForumMessage message = new()
        {
            Id = Guid.NewGuid(),
            CourseId = request.CourseId,
            UserId = userId.Value,
            MessageText = trimmed,
        };

        await _forumService.AddMessageAsync(message, cancellationToken);
        await _transactionService.SaveChangesAsync(cancellationToken);

        _logger.LogInfo(
            "Forum message {MessageId} created for course {CourseId}.",
            message.Id,
            request.CourseId
        );

        return new SuccessResponse
        {
            Id = message.Id,
            Message = "Forum message posted successfully",
        };
    }
}
