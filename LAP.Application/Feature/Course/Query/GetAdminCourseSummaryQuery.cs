using LAP.Application.DTO.Course;
using LAP.Application.Interface;
using LAP.Application.Interface.IService;
using MediatR;
using CourseEntity = LAP.Domain.Entity.Course;
using EnrollmentEntity = LAP.Domain.Entity.Enrollment;

namespace LAP.Application.Feature.Course.Query;

/// <summary>
/// Query for retrieving admin course summary metrics.
/// </summary>
public record GetAdminCourseSummaryQuery : IRequest<AdminCourseSummaryDto>;

/// <summary>
/// Handles the <see cref="GetAdminCourseSummaryQuery"/> by calculating admin course summary metrics.
/// </summary>
public class GetAdminCourseSummaryHandler
    : IRequestHandler<GetAdminCourseSummaryQuery, AdminCourseSummaryDto>
{
    private readonly ICustomLogger<GetAdminCourseSummaryHandler> _logger;
    private readonly ICourseService _courseService;
    private readonly IEnrollmentService _enrollmentService;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetAdminCourseSummaryHandler"/> class.
    /// </summary>
    /// <param name="logger">Custom application logger.</param>
    /// <param name="courseService">Service used to retrieve course data.</param>
    /// <param name="enrollmentService">Service used to retrieve enrollment data.</param>
    public GetAdminCourseSummaryHandler(
        ICustomLogger<GetAdminCourseSummaryHandler> logger,
        ICourseService courseService,
        IEnrollmentService enrollmentService
    )
    {
        _logger = logger;
        _courseService = courseService;
        _enrollmentService = enrollmentService;
    }

    /// <summary>
    /// Handles the admin course summary query and returns aggregated course and enrollment metrics.
    /// </summary>
    /// <param name="request">The <see cref="GetAdminCourseSummaryQuery"/> requesting admin summary metrics.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>
    /// An <see cref="AdminCourseSummaryDto"/> containing aggregated course and enrollment metrics.
    /// </returns>
    public async Task<AdminCourseSummaryDto> Handle(
        GetAdminCourseSummaryQuery request,
        CancellationToken cancellationToken
    )
    {
        _logger.LogInfo("Fetching admin course summary metrics.");

        List<CourseEntity> course = await _courseService.GetAllCourseAsync(cancellationToken);
        List<EnrollmentEntity> enrollment = await _enrollmentService.GetAllEnrollmentAsync(
            cancellationToken
        );

        _logger.LogInfo(
            "Retrieved {TotalCourses} courses and {TotalEnrollments} enrollments for admin summary.",
            course.Count,
            enrollment.Count
        );

        return new AdminCourseSummaryDto
        {
            TotalCourses = course.Count,
            PublishedCourses = course.Count(c => !c.IsDrafted),
            DraftCourses = course.Count(c => c.IsDrafted),
            TotalEnrollments = enrollment.Count,
            ActiveStudents = enrollment.Select(e => e.UserId).Distinct().Count(),
        };
    }
}
