using AutoMapper;
using FluentValidation;
using LAP.Application.DTO.Course;
using LAP.Application.Interface;
using LAP.Application.Interface.IContext;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using LAP.Shared.Exceptions;
using MediatR;

namespace LAP.Application.Feature.Course.Query;

/// <summary>
/// Query to retrieve a user's overall progress in a course.
/// </summary>
/// <param name="Id">The unique identifier of the course.</param>
public record GetCourseProgressQuery(Guid Id) : IRequest<CourseProgressResponseDto>;

/// <summary>
/// Validates the <see cref="GetCourseProgressQuery"/>.
/// </summary>
public class GetCourseProgressValidator : AbstractValidator<GetCourseProgressQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetCourseProgressValidator"/> class.
    /// </summary>
    public GetCourseProgressValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Course identifier is required");
    }
}

/// <summary>
/// Handles the retrieval of course progress for the current user.
/// </summary>
public class GetCourseProgressHandler
    : IRequestHandler<GetCourseProgressQuery, CourseProgressResponseDto>
{
    private readonly ICourseService _courseService;
    private readonly IMapper _mapper;
    private readonly IRequestContext _requestContext;
    private readonly ICustomLogger<GetCourseProgressHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetCourseProgressHandler"/> class.
    /// </summary>
    /// <param name="courseService">The course service.</param>
    /// <param name="mapper">The AutoMapper instance.</param>
    /// <param name="requestContext">The current request context.</param>
    /// <param name="logger">The custom logger.</param>
    public GetCourseProgressHandler(
        ICourseService courseService,
        IMapper mapper,
        IRequestContext requestContext,
        ICustomLogger<GetCourseProgressHandler> logger
    )
    {
        _courseService = courseService;
        _mapper = mapper;
        _requestContext = requestContext;
        _logger = logger;
    }

    /// <summary>
    /// Processes the progress request.
    /// </summary>
    /// <param name="request">The get course progress query.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="CourseProgressResponseDto"/> containing the user's progress in the course.</returns>
    public async Task<CourseProgressResponseDto> Handle(
        GetCourseProgressQuery request,
        CancellationToken cancellationToken
    )
    {
        Guid userId = _requestContext.UserId.Value;

        _logger.LogInfo(
            "Started fetching progress for course {CourseId} and user {UserId}.",
            request.Id,
            userId
        );

        LAP.Domain.Entity.Enrollment? enrollment = await _courseService.GetEnrollmentAsync(
            request.Id,
            userId,
            cancellationToken
        );

        if (enrollment == null)
        {
            _logger.LogError(
                "Enrollment not found for course {CourseId} and user {UserId}.",
                request.Id,
                userId
            );
            throw new NotFoundException(
                "Enrollment not found",
                "You are not enrolled in this course."
            );
        }

        CourseProgressResponseDto result = _mapper.Map<CourseProgressResponseDto>(enrollment);

        _logger.LogInfo(
            "Completed fetching progress for course {CourseId} and user {UserId}. Progress: {Progress}%.",
            request.Id,
            userId,
            result.ProgressPercentage
        );

        return result;
    }
}
