using LAP.Application.DTO.Common;
using LAP.Application.Interface;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using MediatR;

namespace LAP.Application.Feature.Course.Query;

/// <summary>
/// Query to retrieve categories that have at least one active course.
/// </summary>
public record GetActiveCategoryQuery : IRequest<List<RefTermDto>>;

/// <summary>
/// Handles retrieval of active categories by delegating to the course service.
/// </summary>
public class GetActiveCategoryHandler : IRequestHandler<GetActiveCategoryQuery, List<RefTermDto>>
{
    private readonly ICourseService _courseService;
    private readonly ICustomLogger<GetActiveCategoryHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetActiveCategoryHandler"/> class.
    /// </summary>
    /// <param name="courseService">The course service.</param>
    /// <param name="logger">Custom application logger.</param>
    public GetActiveCategoryHandler(
        ICourseService courseService,
        ICustomLogger<GetActiveCategoryHandler> logger
    )
    {
        _courseService = courseService;
        _logger = logger;
    }

    /// <summary>
    /// Handles the query by fetching active categories and mapping them to DTOs.
    /// </summary>
    /// <param name="request">The query.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A list of active categories with their ID and name.</returns>
    public async Task<List<RefTermDto>> Handle(
        GetActiveCategoryQuery request,
        CancellationToken cancellationToken
    )
    {
        _logger.LogInfo("Fetching active categories.");

        List<RefTerm> category = await _courseService.GetActiveCategoryAsync(cancellationToken);

        List<RefTermDto> result = category
            .Select(c => new RefTermDto { Id = c.Id, Name = c.Name })
            .ToList();

        _logger.LogInfo("Found {Count} active categories.", result.Count);

        return result;
    }
}
