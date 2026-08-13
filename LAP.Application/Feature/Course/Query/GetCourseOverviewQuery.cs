using AutoMapper;
using FluentValidation;
using LAP.Application.DTO.Course;
using LAP.Application.Interface;
using LAP.Application.Interface.IService;
using LAP.Shared.Exceptions;
using MediatR;

namespace LAP.Application.Feature.Course.Query;

/// <summary>
/// Query to retrieve a detailed overview of a course.
/// </summary>
/// <param name="Id">The unique identifier of the course.</param>
public record GetCourseOverviewQuery(Guid Id) : IRequest<CourseOverviewDto>;

/// <summary>
/// Validates the <see cref="GetCourseOverviewQuery"/>.
/// </summary>
public class GetCourseOverviewValidator : AbstractValidator<GetCourseOverviewQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetCourseOverviewValidator"/> class.
    /// </summary>
    public GetCourseOverviewValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Course identifier is required");
    }
}

/// <summary>
/// Handles the retrieval of course overview details.
/// </summary>
public class GetCourseOverviewHandler : IRequestHandler<GetCourseOverviewQuery, CourseOverviewDto>
{
    private readonly ICourseService _courseService;
    private readonly IMapper _mapper;
    private readonly IFileStorageService _fileStorageService;
    private readonly ICustomLogger<GetCourseOverviewHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetCourseOverviewHandler"/> class.
    /// </summary>
    /// <param name="courseService">The course service.</param>
    /// <param name="mapper">The AutoMapper instance.</param>
    /// <param name="fileStorageService">The file storage service for Base64 conversion.</param>
    /// <param name="logger">The custom logger.</param>
    public GetCourseOverviewHandler(
        ICourseService courseService,
        IMapper mapper,
        IFileStorageService fileStorageService,
        ICustomLogger<GetCourseOverviewHandler> logger
    )
    {
        _courseService = courseService;
        _mapper = mapper;
        _fileStorageService = fileStorageService;
        _logger = logger;
    }

    /// <summary>
    /// Processes the overview request.
    /// </summary>
    /// <param name="request">The get course overview query.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="CourseOverviewDto"/> containing the course details.</returns>
    public async Task<CourseOverviewDto> Handle(
        GetCourseOverviewQuery request,
        CancellationToken cancellationToken
    )
    {
        _logger.LogInfo("Started fetching overview for course {CourseId}.", request.Id);

        Domain.Entity.Course? course = await _courseService.GetCourseOverviewAsync(
            request.Id,
            cancellationToken
        );

        if (course == null)
        {
            _logger.LogError("Course {CourseId} not found for overview.", request.Id);
            throw new NotFoundException(
                "Course not found",
                $"Course with ID {request.Id} does not exist."
            );
        }

        CourseOverviewDto result = _mapper.Map<CourseOverviewDto>(course);
        result.Topic = result.Topic.Where(t => t.Contents.Count > 0).ToList();
        result.ThumbnailImg = await _fileStorageService.GetBase64Async(course.ThumbnailImgPath);

        _logger.LogInfo("Completed fetching overview for course {CourseId}.", request.Id);

        return result;
    }
}
