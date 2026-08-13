using AutoMapper;
using LAP.Application.Constant;
using LAP.Application.DTO.Course;
using LAP.Application.Interface;
using LAP.Application.Interface.IContext;
using LAP.Application.Interface.IRepository;
using LAP.Application.Interface.IService;
using LAP.Shared.Exceptions;
using MediatR;

namespace LAP.Application.Feature.Course.Query;

/// <summary>
/// Query to retrieve course recommendations for the current user.
/// </summary>
public record GetCourseRecommendationQuery : IRequest<IEnumerable<CourseSummaryDto>>;

/// <summary>
/// Handles the retrieval of recommended courses based on user history.
/// </summary>
public class GetCourseRecommendationHandler
    : IRequestHandler<GetCourseRecommendationQuery, IEnumerable<CourseSummaryDto>>
{
    private readonly ICourseService _courseService;
    private readonly IMapper _mapper;
    private readonly IRequestContext _requestContext;
    private readonly IFileStorageService _fileStorageService;
    private readonly ICustomLogger<GetCourseRecommendationHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetCourseRecommendationHandler"/> class.
    /// </summary>
    /// <param name="courseService">The course service.</param>
    /// <param name="mapper">The AutoMapper instance.</param>
    /// <param name="requestContext">The current request context.</param>
    /// <param name="fileStorageService">The file storage service for Base64 conversion.</param>
    /// <param name="logger">The custom logger.</param>
    public GetCourseRecommendationHandler(
        ICourseService courseService,
        IMapper mapper,
        IRequestContext requestContext,
        IFileStorageService fileStorageService,
        ICustomLogger<GetCourseRecommendationHandler> logger
    )
    {
        _courseService = courseService;
        _mapper = mapper;
        _requestContext = requestContext;
        _fileStorageService = fileStorageService;
        _logger = logger;
    }

    /// <summary>
    /// Processes the recommendation request.
    /// </summary>
    /// <param name="request">The get course recommendation query.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of recommended course summaries.</returns>
    public async Task<IEnumerable<CourseSummaryDto>> Handle(
        GetCourseRecommendationQuery request,
        CancellationToken cancellationToken
    )
    {
        if (_requestContext.UserId is null)
        {
            throw new UnauthorizedException("User not authenticated", "User is not authenticated.");
        }

        Guid userId = _requestContext.UserId.Value;

        _logger.LogInfo("Started fetching course recommendations for user {UserId}.", userId);

        IEnumerable<Domain.Entity.Course> recommendedCourses =
            await _courseService.GetRecommendedCourseAsync(
                userId,
                CommonConstants.TOP_10_RECOMMENDATIONS,
                cancellationToken
            );

        List<CourseSummaryDto> result = _mapper.Map<List<CourseSummaryDto>>(
            recommendedCourses
        );
        foreach (CourseSummaryDto dto in result)
        {
            dto.ThumbnailImg = await _fileStorageService.GetBase64Async(dto.ThumbnailImg);
        }

        _logger.LogInfo("Completed fetching course recommendations for user {UserId}.", userId);

        return result;
    }
}
