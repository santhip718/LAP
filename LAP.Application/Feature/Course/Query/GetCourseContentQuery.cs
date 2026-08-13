using AutoMapper;
using FluentValidation;
using LAP.Application.DTO.Course;
using LAP.Application.Interface;
using LAP.Application.Interface.IContext;
using LAP.Application.Interface.IService;
using LAP.Shared.Exceptions;
using MediatR;

namespace LAP.Application.Feature.Course.Query;

/// <summary>
/// Query to retrieve course contents with user-specific progress.
/// </summary>
/// <param name="Id">The unique identifier of the course.</param>
public record GetCourseContentQuery(Guid Id) : IRequest<CourseContentResponseDto>;

/// <summary>
/// Validates the <see cref="GetCourseContentQuery"/>.
/// </summary>
public class GetCourseContentValidator : AbstractValidator<GetCourseContentQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetCourseContentValidator"/> class.
    /// </summary>
    public GetCourseContentValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Course identifier is required");
    }
}

/// <summary>
/// Handles the retrieval of course contents and user progress.
/// </summary>
public class GetCourseContentHandler
    : IRequestHandler<GetCourseContentQuery, CourseContentResponseDto>
{
    private readonly ICourseService _courseService;
    private readonly IMapper _mapper;
    private readonly IRequestContext _requestContext;
    private readonly IFileStorageService _fileStorageService;
    private readonly ICustomLogger<GetCourseContentHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetCourseContentHandler"/> class.
    /// </summary>
    /// <param name="courseService">The course service.</param>
    /// <param name="mapper">The AutoMapper instance.</param>
    /// <param name="requestContext">The current request context.</param>
    /// <param name="fileStorageService">The file storage service for Base64 conversion.</param>
    /// <param name="logger">The custom logger.</param>
    public GetCourseContentHandler(
        ICourseService courseService,
        IMapper mapper,
        IRequestContext requestContext,
        IFileStorageService fileStorageService,
        ICustomLogger<GetCourseContentHandler> logger
    )
    {
        _courseService = courseService;
        _mapper = mapper;
        _requestContext = requestContext;
        _fileStorageService = fileStorageService;
        _logger = logger;
    }

    /// <summary>
    /// Processes the contents request.
    /// </summary>
    /// <param name="request">The get course content query.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="CourseContentResponseDto"/> containing the course contents and progress.</returns>
    public async Task<CourseContentResponseDto> Handle(
        GetCourseContentQuery request,
        CancellationToken cancellationToken
    )
    {
        Guid userId = _requestContext.UserId.Value;

        _logger.LogInfo(
            "Started fetching contents for course {CourseId} and user {UserId}.",
            request.Id,
            userId
        );

        Domain.Entity.Course? course = await _courseService.GetCourseWithProgressAsync(
            request.Id,
            userId,
            cancellationToken
        );

        if (course == null)
        {
            _logger.LogError("Course {CourseId} not found for contents retrieval.", request.Id);
            throw new NotFoundException(
                "Course not found",
                $"Course with ID {request.Id} does not exist."
            );
        }

        CourseContentResponseDto result = _mapper.Map<CourseContentResponseDto>(course);
        result.Topic = result.Topic.Where(t => t.Contents.Count > 0).ToList();
        result.ThumbnailImg = await _fileStorageService.GetBase64Async(course.ThumbnailImgPath);

        _logger.LogInfo(
            "Completed fetching contents for course {CourseId} and user {UserId}.",
            request.Id,
            userId
        );

        return result;
    }
}
