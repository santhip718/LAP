using AutoMapper;
using FluentValidation;
using LAP.Application.DTO.CourseContent;
using LAP.Application.Interface;
using LAP.Application.Interface.IContext;
using LAP.Application.Interface.IService;
using LAP.Shared.Exceptions;
using MediatR;
using CourseContentEntity = LAP.Domain.Entity.CourseContent;
using EnrollmentEntity = LAP.Domain.Entity.Enrollment;
using UserCourseProgressEntity = LAP.Domain.Entity.UserCourseProgress;

namespace LAP.Application.Feature.CourseContent.Command;

/// <summary>
/// Command to update the completion status of a course content item for the current user.
/// </summary>
/// <param name="ContentId">The unique identifier of the course content.</param>
/// <param name="Request">The completion status details.</param>
public record UpdateContentCompletionStatusCommand(
    Guid ContentId,
    UpdateContentCompletionStatusRequest Request
) : IRequest<UpdateContentCompletionStatusResponse>;

/// <summary>
/// Validates the <see cref="UpdateContentCompletionStatusCommand"/>.
/// </summary>
public class UpdateContentCompletionStatusValidator
    : AbstractValidator<UpdateContentCompletionStatusCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateContentCompletionStatusValidator"/> class.
    /// </summary>
    public UpdateContentCompletionStatusValidator()
    {
        RuleFor(x => x.ContentId).NotEmpty().WithMessage("Course content identifier is required");
        RuleFor(x => x.Request).NotNull().WithMessage("Completion status details are required");
        When(
            x => x.Request is not null,
            () =>
            {
                RuleFor(x => x.Request.IsCompleted)
                    .NotNull()
                    .WithMessage("Completion status is required");
            }
        );
    }
}

/// <summary>
/// Handles the update of course content completion status.
/// </summary>
public class UpdateContentCompletionStatusHandler
    : IRequestHandler<UpdateContentCompletionStatusCommand, UpdateContentCompletionStatusResponse>
{
    private readonly ICourseContentService _courseContentService;
    private readonly ITransactionService _transactionService;
    private readonly IRequestContext _requestContext;
    private readonly ICustomLogger<UpdateContentCompletionStatusHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateContentCompletionStatusHandler"/> class.
    /// </summary>
    /// <param name="courseContentService">The course content service.</param>
    /// <param name="transactionService">The transaction service.</param>
    /// <param name="requestContext">The request context.</param>
    /// <param name="logger">The custom logger.</param>
    public UpdateContentCompletionStatusHandler(
        ICourseContentService courseContentService,
        ITransactionService transactionService,
        IRequestContext requestContext,
        ICustomLogger<UpdateContentCompletionStatusHandler> logger
    )
    {
        _courseContentService = courseContentService;
        _transactionService = transactionService;
        _requestContext = requestContext;
        _logger = logger;
    }

    /// <summary>
    /// Processes the completion status update request.
    /// </summary>
    /// <param name="request">The update content completion status command.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="UpdateContentCompletionStatusResponse"/> containing the updated status and progress.</returns>
    public async Task<UpdateContentCompletionStatusResponse> Handle(
        UpdateContentCompletionStatusCommand request,
        CancellationToken cancellationToken
    )
    {
        Guid userId = _requestContext.UserId.Value;
        _logger.LogInfo(
            "Started updating completion status for content {ContentId} by user {UserId}.",
            request.ContentId,
            userId
        );

        // Read phase: validate and fetch required data outside transaction
        CourseContentEntity? content = await _courseContentService.GetContentWithMetaTopicAsync(
            request.ContentId,
            cancellationToken
        );

        if (content == null)
        {
            _logger.LogError(
                "Course content {ContentId} not found for completion update.",
                request.ContentId
            );
            throw new NotFoundException(
                "Course content not found",
                $"Course content with ID {request.ContentId} does not exist."
            );
        }

        EnrollmentEntity? enrollment =
            await _courseContentService.GetEnrollmentByUserAndCourseAsync(
                userId,
                content.MetaTopic.CourseId,
                cancellationToken
            );

        if (enrollment == null)
        {
            _logger.LogError(
                "User {UserId} is not enrolled in course {CourseId} for content completion.",
                userId,
                content.MetaTopic.CourseId
            );
            throw new BadRequestException(
                "Not enrolled",
                "You are not enrolled in this course. Please enroll before tracking progress."
            );
        }

        UserCourseProgressEntity? existingProgress = await _courseContentService.GetProgressAsync(
            enrollment.Id,
            content.Id,
            cancellationToken
        );

        bool isNewProgress = existingProgress == null;

        UserCourseProgressEntity progress =
            existingProgress
            ?? new UserCourseProgressEntity
            {
                EnrollmentId = enrollment.Id,
                CourseContentId = content.Id,
            };

        // Capture existing completed status before mutation (progress shares same reference)
        bool wasPreviouslyCompleted = !isNewProgress && existingProgress!.IsCompleted;

        progress.IsCompleted = request.Request.IsCompleted;
        progress.CompletedOn = request.Request.IsCompleted ? DateTime.UtcNow : null;

        int totalContents = await _courseContentService.GetTotalContentCountAsync(
            content.MetaTopic.CourseId,
            cancellationToken
        );

        // Fetch current completed count before the pending change
        int completedCount =
            totalContents > 0
                ? await _courseContentService.GetCompletedContentCountAsync(
                    enrollment.Id,
                    cancellationToken
                )
                : 0;

        // Adjust completed count for the pending progress change
        if (isNewProgress && request.Request.IsCompleted)
        {
            completedCount++;
        }
        else if (!isNewProgress && wasPreviouslyCompleted != request.Request.IsCompleted)
        {
            completedCount = request.Request.IsCompleted ? completedCount + 1 : completedCount - 1;
        }

        decimal progressPercentage =
            totalContents > 0 ? Math.Round((decimal)completedCount / totalContents * 100, 2) : 0;

        return await _transactionService.ExecuteInTransactionAsync(
            async () =>
            {
                if (isNewProgress)
                {
                    await _courseContentService.AddProgressAsync(progress, cancellationToken);
                }
                else
                {
                    await _courseContentService.UpdateProgressAsync(progress, cancellationToken);
                }

                if (request.Request.IsCompleted && progressPercentage >= 100)
                {
                    await _courseContentService.UpdateEnrollmentProgressAsync(
                        enrollment.Id,
                        100,
                        cancellationToken
                    );
                }
                else
                {
                    await _courseContentService.UpdateEnrollmentProgressAsync(
                        enrollment.Id,
                        progressPercentage,
                        cancellationToken
                    );
                }

                await _courseContentService.SaveChangesAsync(cancellationToken);

                _logger.LogInfo(
                    "Completed updating completion status for content {ContentId}. Progress: {Progress}%.",
                    request.ContentId,
                    progressPercentage
                );

                return new UpdateContentCompletionStatusResponse
                {
                    CourseContentId = content.Id,
                    IsCompleted = progress.IsCompleted,
                    CompletedOn = progress.CompletedOn,
                    CourseProgressPercentage = progressPercentage,
                };
            },
            cancellationToken
        );
    }
}
