using LAP.Application.DTO.Common;
using LAP.Application.Interface;
using LAP.Application.Interface.IContext;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using LAP.Shared.Exceptions;
using MediatR;

namespace LAP.Application.Feature.Course.Command;

/// <summary>
/// Command to request enrollment in a specific course.
/// </summary>
/// <param name="CourseId">The unique identifier of the course.</param>
public record RequestEnrollmentCommand(Guid CourseId) : IRequest<SuccessResponse>;

/// <summary>
/// Handler for <see cref="RequestEnrollmentCommand"/>.
/// </summary>
public class RequestEnrollmentHandler : IRequestHandler<RequestEnrollmentCommand, SuccessResponse>
{
    private readonly ICourseService _courseService;
    private readonly IRequestContext _requestContext;
    private readonly ICustomLogger<RequestEnrollmentHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestEnrollmentHandler"/> class.
    /// </summary>
    /// <param name="courseService">The course service.</param>
    /// <param name="requestContext">The request context.</param>
    /// <param name="logger">The custom logger.</param>
    public RequestEnrollmentHandler(
        ICourseService courseService,
        IRequestContext requestContext,
        ICustomLogger<RequestEnrollmentHandler> logger
    )
    {
        _courseService = courseService;
        _requestContext = requestContext;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<SuccessResponse> Handle(
        RequestEnrollmentCommand request,
        CancellationToken cancellationToken
    )
    {
        _logger.LogInfo(
            "Started processing enrollment request for course {CourseId}.",
            request.CourseId
        );

        Guid userId = _requestContext.UserId ?? Guid.Empty;

        LAP.Domain.Entity.Enrollment? enrollment = await _courseService.RequestEnrollmentAsync(
            request.CourseId,
            userId,
            cancellationToken
        );

        if (enrollment == null)
        {
            _logger.LogError(
                "Course {CourseId} not found for enrollment request.",
                request.CourseId
            );
            throw new NotFoundException(
                "Course not found",
                $"Course with ID {request.CourseId} does not exist."
            );
        }

        _logger.LogInfo(
            "Completed enrollment request for course {CourseId}. EnrollmentId: {EnrollmentId}",
            request.CourseId,
            enrollment.Id
        );

        return new SuccessResponse { Id = enrollment.Id, Message = "Enrollment request created" };
    }
}
