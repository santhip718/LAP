using FluentValidation;
using LAP.Application.DTO.Common;
using LAP.Application.Interface;
using LAP.Application.Interface.IContext;
using LAP.Application.Interface.IService;
using LAP.Shared.Exceptions;
using MediatR;
using CourseEntity = LAP.Domain.Entity.Course;
using EnrollmentEntity = LAP.Domain.Entity.Enrollment;

namespace LAP.Application.Feature.Enrollment.Command;

/// <summary>
/// Command to create a new enrollment for a course.
/// </summary>
/// <param name="CourseId">The identifier of the course to enroll in.</param>
public record CreateEnrollmentCommand(Guid CourseId) : IRequest<SuccessResponse>;

/// <summary>
/// Validates the <see cref="CreateEnrollmentCommand"/> before processing.
/// </summary>
public class CreateEnrollmentCommandValidator : AbstractValidator<CreateEnrollmentCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateEnrollmentCommandValidator"/> class.
    /// </summary>
    public CreateEnrollmentCommandValidator()
    {
        RuleFor(x => x.CourseId).NotEmpty().WithMessage("Course ID is required");
    }
}

/// <summary>
/// Handles the <see cref="CreateEnrollmentCommand"/> and creates a new enrollment.
/// </summary>
public class CreateEnrollmentHandler : IRequestHandler<CreateEnrollmentCommand, SuccessResponse>
{
    private readonly IEnrollmentService _enrollmentService;
    private readonly ICourseService _courseService;
    private readonly ITransactionService _transactionService;
    private readonly IRequestContext _requestContext;
    private readonly ICustomLogger<CreateEnrollmentHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateEnrollmentHandler"/> class.
    /// </summary>
    /// <param name="enrollmentService">The enrollment service for enrollment operations.</param>
    /// <param name="courseService">The course service for course lookups.</param>
    /// <param name="transactionService">The transaction service for database transactions.</param>
    /// <param name="requestContext">The request context providing user information.</param>
    /// <param name="logger">The application logger.</param>
    public CreateEnrollmentHandler(
        IEnrollmentService enrollmentService,
        ICourseService courseService,
        ITransactionService transactionService,
        IRequestContext requestContext,
        ICustomLogger<CreateEnrollmentHandler> logger
    )
    {
        _enrollmentService = enrollmentService;
        _courseService = courseService;
        _transactionService = transactionService;
        _requestContext = requestContext;
        _logger = logger;
    }

    /// <summary>
    /// Processes the enrollment creation.
    /// </summary>
    /// <param name="request">The command containing the course identifier.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A success response with the enrollment identifier.</returns>
    public async Task<SuccessResponse> Handle(
        CreateEnrollmentCommand request,
        CancellationToken cancellationToken
    )
    {
        _logger.LogInfo("Creating enrollment for course id: {CourseId}", request.CourseId);

        CourseEntity? course = await _courseService.GetCourseByIdAsync(
            request.CourseId,
            cancellationToken
        );

        if (course is null)
        {
            _logger.LogError("Course not found for id: {CourseId}", request.CourseId);
            throw new NotFoundException(
                "Course not found",
                $"No course found with id {request.CourseId}"
            );
        }

        Guid? userId = _requestContext.UserId;

        if (userId is null)
        {
            _logger.LogError("User ID not found in request context");
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        bool alreadyEnrolled = await _enrollmentService.IsUserEnrolledAsync(
            request.CourseId,
            userId.Value,
            cancellationToken
        );

        if (alreadyEnrolled)
        {
            _logger.LogError(
                "User {UserId} is already enrolled in course {CourseId}.",
                userId.Value,
                request.CourseId
            );
            throw new BadRequestException(
                "Already enrolled",
                $"User {userId.Value} is already enrolled in course {request.CourseId}."
            );
        }

        EnrollmentEntity enrollment = new()
        {
            Id = Guid.NewGuid(),
            CourseId = request.CourseId,
            UserId = userId.Value,
            EnrolledOn = DateTime.UtcNow,
            ProgressPercentage = 0,
            EnrollmentStatus = false,
        };

        await _enrollmentService.AddEnrollmentAsync(enrollment, cancellationToken);
        await _transactionService.SaveChangesAsync(cancellationToken);

        _logger.LogInfo(
            "Enrollment created successfully - Id: {EnrollmentId}, CourseId: {CourseId}, UserId: {UserId}",
            enrollment.Id,
            request.CourseId,
            userId.Value
        );

        return new SuccessResponse
        {
            Id = enrollment.Id,
            Message = "Enrollment created successfully",
        };
    }
}
