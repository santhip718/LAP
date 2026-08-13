using LAP.API.Authorization;
using LAP.Application.Constant;
using LAP.Application.DTO.Assessment;
using LAP.Application.DTO.Common;
using LAP.Application.DTO.Course;
using LAP.Application.DTO.Forum;
using LAP.Application.DTO.Paginated;
using LAP.Application.Feature.Assessment.Query;
using LAP.Application.Feature.Course.Command;
using LAP.Application.Feature.Course.Query;
using LAP.Application.Feature.Enrollment.Command;
using LAP.Application.Feature.Forum.Command;
using LAP.Application.Feature.Forum.Query;
using LAP.Application.Feature.Leaderboard.Query;
using LAP.Application.Interface;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Swashbuckle.AspNetCore.Annotations;

namespace LAP.API.Controller;

/// <summary>
/// Handles course management operations and provides endpoints for querying course information.
/// </summary>
[Authorize]
[Route("api/v1/course")]
public class CourseController : BaseController
{
    private readonly IMediator _mediator;
    private readonly ICustomLogger<CourseController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CourseController"/> class.
    /// </summary>
    /// <param name="mediator">Mediator for dispatching commands and queries.</param>
    /// <param name="logger">Custom application logger.</param>
    public CourseController(IMediator mediator, ICustomLogger<CourseController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves assessment overviews for a specific course.
    /// </summary>
    /// <param name="courseId">The course identifier.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A list of assessment overviews.</returns>
    [HttpGet("{course-id}/assessment/overview")]
    [FeatureAuthorize("VIEW_ASSESSMENT")]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "Assessment overview retrieved successfully.",
        typeof(List<AssessmentOverviewDto>)
    )]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication required.")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Insufficient permissions.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Course not found.")]
    public async Task<IActionResult> GetAssessmentOverviewByCourseId(
        [FromRoute(Name = "course-id")] Guid courseId,
        CancellationToken cancellationToken
    )
    {
        _logger.LogDebug("Assessment overview retrieval started for course: {CourseId}", courseId);

        List<AssessmentOverviewDto> result = await _mediator.Send(
            new GetAssessmentOverviewByCourseIdQuery(courseId),
            cancellationToken
        );

        _logger.LogDebug(
            "Completed assessment overview retrieval for course: {CourseId}",
            courseId
        );
        return Ok(result);
    }

    /// <summary>Creates a new course with the provided details.</summary>
    /// <param name="dto">The course creation request containing title, description, category and other metadata.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>An <see cref="IActionResult"/> containing the created course identifier.</returns>
    [HttpPost]
    [FeatureAuthorize("MANAGE_COURSE")]
    [SwaggerResponse(
        StatusCodes.Status201Created,
        "Course created successfully.",
        typeof(SuccessResponse)
    )]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Validation failed.")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication required.")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Insufficient permissions.")]
    public async Task<IActionResult> Create(
        [FromForm] CreateCourseRequestDto dto,
        CancellationToken cancellationToken
    )
    {
        _logger.LogDebug(
            "Received create course request for title {Title} and category {CategoryId}.",
            dto.Title,
            dto.CategoryId
        );

        SuccessResponse result = await _mediator.Send(
            new CreateCourseCommand(dto),
            cancellationToken
        );

        _logger.LogDebug("Created course {CourseId}.", result.Id);
        return CreatedAtAction(nameof(Create), new { id = result.Id }, result);
    }

    /// <summary>
    /// Updates an existing course with the provided details.
    /// </summary>
    /// <param name="courseId">The course identifier.</param>
    /// <param name="dto">The course update request.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>An <see cref="IActionResult"/> containing the updated course identifier.</returns>
    [HttpPut("{course-id:guid}")]
    [FeatureAuthorize("MANAGE_COURSE")]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "Course updated successfully.",
        typeof(SuccessResponse)
    )]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Validation failed.")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication required.")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Insufficient permissions.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Course not found.")]
    public async Task<IActionResult> Update(
        [FromRoute(Name = "course-id")] Guid courseId,
        [FromForm] UpdateCourseRequestDto dto,
        CancellationToken cancellationToken
    )
    {
        _logger.LogDebug(
            "Received update course request for course {CourseId} and title {Title}.",
            courseId,
            dto.Title
        );

        SuccessResponse result = await _mediator.Send(
            new UpdateCourseCommand(courseId, dto),
            cancellationToken
        );

        _logger.LogDebug("Updated course {CourseId}.", result.Id);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a paginated list of courses with optional filtering and search.
    /// </summary>
    /// <param name="page">The page number (default 1).</param>
    /// <param name="pageSize">The number of records per page (default 20).</param>
    /// <param name="categoryId">Optional category filter.</param>
    /// <param name="difficultyLevelId">Optional difficulty level filter.</param>
    /// <param name="status">Optional status filter.</param>
    /// <param name="search">Optional search string for title.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A paginated list of courses.</returns>
    [HttpGet]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "A paginated list of courses.",
        typeof(PaginatedCoursesDto)
    )]
    [SwaggerResponse(
        StatusCodes.Status400BadRequest,
        "Invalid pagination parameters.",
        typeof(ErrorResponse)
    )]
    [SwaggerResponse(
        StatusCodes.Status401Unauthorized,
        "User is not authenticated.",
        typeof(ErrorResponse)
    )]
    [SwaggerResponse(
        StatusCodes.Status403Forbidden,
        "User does not have permission.",
        typeof(ErrorResponse)
    )]
    [SwaggerResponse(
        StatusCodes.Status500InternalServerError,
        "Internal server error.",
        typeof(ErrorResponse)
    )]
    [AllowAnonymous]
    public async Task<IActionResult> GetCourses(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] Guid? categoryId = null,
        [FromQuery] Guid? difficultyLevelId = null,
        [FromQuery] bool? status = null,
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Received request to get courses.");
        PaginatedCoursesDto result = await _mediator.Send(
            new GetCourseQuery(page, pageSize, categoryId, difficultyLevelId, status, search),
            cancellationToken
        );
        _logger.LogDebug("Completed request to get courses.");
        return Ok(result);
    }

    /// <summary>
    /// Deletes (soft-deletes) a course by its identifier.
    /// </summary>
    /// <param name="courseId">The course identifier.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>An <see cref="IActionResult"/> indicating the outcome.</returns>
    [HttpDelete("{course-id:guid}")]
    [FeatureAuthorize("MANAGE_COURSE")]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "Course deleted successfully.",
        typeof(SuccessResponse)
    )]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication required.")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Insufficient permissions.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Course not found.")]
    public async Task<IActionResult> Delete(
        [FromRoute(Name = "course-id")] Guid courseId,
        CancellationToken cancellationToken
    )
    {
        _logger.LogDebug("Received delete course request for course {CourseId}.", courseId);

        SuccessResponse result = await _mediator.Send(
            new DeleteCourseCommand(courseId),
            cancellationToken
        );

        _logger.LogDebug("Deleted course {CourseId}.", result.Id);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves the leaderboard for a specific course.
    /// </summary>
    /// <param name="courseId">The unique identifier of the course.</param>
    /// <param name="pageSize">The number of items to retrieve.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A list of leaderboard entries for the course.</returns>
    [HttpGet("{course-id}/leaderboard")]
    [FeatureAuthorize("VIEW_LEADERBOARD")]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "Leaderboard retrieved successfully.",
        typeof(List<LeaderboardDto>)
    )]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid course identifier.")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication required.")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Insufficient permissions.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Course not found.")]
    public async Task<IActionResult> GetLeaderboardByCourseId(
        [FromRoute(Name = "course-id")] Guid courseId,
        [FromQuery] int pageSize = CommonConstants.DEFAULT_PAGE_SIZE,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Leaderboard retrieval started for course: {CourseId}", courseId);

        List<LeaderboardDto> result = await _mediator.Send(
            new GetLeaderboardByCourseIdQuery(courseId, pageSize),
            cancellationToken
        );

        _logger.LogDebug("Completed leaderboard retrieval for course: {CourseId}", courseId);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves course recommendations for the current user.
    /// </summary>
    /// <returns>A list of recommended courses.</returns>
    [HttpGet("recommendation")]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "A list of recommended courses.",
        typeof(IEnumerable<CourseSummaryDto>)
    )]
    [SwaggerResponse(
        StatusCodes.Status401Unauthorized,
        "User is not authenticated.",
        typeof(ErrorResponse)
    )]
    [SwaggerResponse(
        StatusCodes.Status403Forbidden,
        "User does not have permission.",
        typeof(ErrorResponse)
    )]
    [SwaggerResponse(
        StatusCodes.Status500InternalServerError,
        "Internal server error.",
        typeof(ErrorResponse)
    )]
    [FeatureAuthorize("VIEW_RECOMMENDATION")]
    public async Task<IActionResult> GetRecommendations(CancellationToken cancellationToken)
    {
        _logger.LogDebug("Received request for course recommendations.");
        IEnumerable<CourseSummaryDto> result = await _mediator.Send(
            new GetCourseRecommendationQuery(),
            cancellationToken
        );
        _logger.LogDebug("Completed request for course recommendations.");
        return Ok(result);
    }

    /// <summary>
    /// Retrieves admin course summary metrics including total, published, draft courses, enrollments and active students.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>An admin course summary with aggregated counts.</returns>
    [HttpGet("admin-summary")]
    [FeatureAuthorize("VIEW_COURSE_ADMINISTRATION")]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "Admin course summary retrieved successfully.",
        typeof(AdminCourseSummaryDto)
    )]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication required.")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Insufficient permissions.")]
    public async Task<IActionResult> GetAdminSummary(CancellationToken cancellationToken)
    {
        _logger.LogDebug("Fetching admin course summary metrics.");

        AdminCourseSummaryDto result = await _mediator.Send(
            new GetAdminCourseSummaryQuery(),
            cancellationToken
        );

        _logger.LogDebug(
            "Retrieved admin course summary with total courses {TotalCourses}, published courses {PublishedCourses}, draft courses {DraftCourses}, enrollments {TotalEnrollments}, and active students {ActiveStudents}.",
            result.TotalCourses,
            result.PublishedCourses,
            result.DraftCourses,
            result.TotalEnrollments,
            result.ActiveStudents
        );

        return Ok(result);
    }

    /// <summary>
    /// Retrieves a detailed overview of a specific course.
    /// </summary>
    /// <param name="id">The unique identifier of the course.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The course overview details.</returns>
    [HttpGet("{id}/overview")]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "The course overview details.",
        typeof(CourseOverviewDto)
    )]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid course ID.", typeof(ErrorResponse))]
    [SwaggerResponse(
        StatusCodes.Status401Unauthorized,
        "User is not authenticated.",
        typeof(ErrorResponse)
    )]
    [SwaggerResponse(
        StatusCodes.Status403Forbidden,
        "User does not have permission.",
        typeof(ErrorResponse)
    )]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Course not found.", typeof(ErrorResponse))]
    [SwaggerResponse(
        StatusCodes.Status500InternalServerError,
        "Internal server error.",
        typeof(ErrorResponse)
    )]
    [FeatureAuthorize("VIEW_COURSE")]
    public async Task<IActionResult> GetOverview(
        [FromRoute(Name = "id")] Guid id,
        CancellationToken cancellationToken
    )
    {
        _logger.LogDebug("Received request for course overview: {Id}", id);
        CourseOverviewDto result = await _mediator.Send(
            new GetCourseOverviewQuery(id),
            cancellationToken
        );
        _logger.LogDebug("Completed request for course overview: {Id}", id);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves all forum messages for a specific course.
    /// </summary>
    /// <param name="courseId">The course identifier.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A list of forum messages for the course.</returns>
    [HttpGet("{course-id:guid}/forum-message")]
    [FeatureAuthorize("VIEW_FORUM")]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "Forum messages retrieved successfully.",
        typeof(List<ForumMessageDto>)
    )]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication required.")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Insufficient permissions.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Course not found.")]
    public async Task<IActionResult> GetForumMessage(
        [FromRoute(Name = "course-id")] Guid courseId,
        CancellationToken cancellationToken
    )
    {
        _logger.LogDebug("Fetching forum messages for course {CourseId}.", courseId);

        List<ForumMessageDto> messages = await _mediator.Send(
            new GetForumMessageQuery(courseId),
            cancellationToken
        );

        _logger.LogDebug(
            "Retrieved {Count} forum messages for course {CourseId}.",
            messages.Count,
            courseId
        );

        return Ok(messages);
    }

    /// <summary>
    /// Posts a new forum message to a course.
    /// </summary>
    /// <param name="courseId">The course identifier.</param>
    /// <param name="dto">The forum message creation request.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>The created forum message.</returns>
    [HttpPost("{course-id:guid}/forum-message")]
    [FeatureAuthorize("PARTICIPATE_FORUM")]
    [SwaggerResponse(
        StatusCodes.Status201Created,
        "Forum message posted successfully.",
        typeof(SuccessResponse)
    )]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Validation failed.")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication required.")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Insufficient permissions.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Course not found.")]
    public async Task<IActionResult> PostForumMessage(
        [FromRoute(Name = "course-id")] Guid courseId,
        [FromBody] CreateForumMessageRequestDto dto,
        CancellationToken cancellationToken
    )
    {
        _logger.LogDebug("Posting forum message for course {CourseId}.", courseId);

        SuccessResponse result = await _mediator.Send(
            new CreateForumMessageCommand(courseId, dto),
            cancellationToken
        );

        _logger.LogDebug(
            "Posted forum message {MessageId} for course {CourseId}.",
            result.Id,
            courseId
        );

        return CreatedAtAction(
            nameof(GetForumMessage),
            new RouteValueDictionary { ["course-id"] = courseId },
            result
        );
    }

    /// <summary>
    /// Enrolls the authenticated user in a course.
    /// </summary>
    /// <param name="courseId">The course identifier.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A success response with the enrollment ID.</returns>
    [HttpPost("{course-id:guid}/enrollment")]
    [FeatureAuthorize("REQUEST_ENROLLMENT")]
    [SwaggerResponse(
        StatusCodes.Status201Created,
        "Enrollment created successfully.",
        typeof(SuccessResponse)
    )]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Validation failed or already enrolled.")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication required.")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Insufficient permissions.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Course not found.")]
    public async Task<IActionResult> Enroll(
        [FromRoute(Name = "course-id")] Guid courseId,
        CancellationToken cancellationToken
    )
    {
        _logger.LogDebug("Enrolling user in course {CourseId}.", courseId);

        SuccessResponse result = await _mediator.Send(
            new CreateEnrollmentCommand(courseId),
            cancellationToken
        );

        _logger.LogDebug(
            "Created enrollment {EnrollmentId} for course {CourseId}.",
            result.Id,
            courseId
        );

        return CreatedAtAction(
            nameof(Enroll),
            new RouteValueDictionary { ["course-id"] = courseId },
            result
        );
    }

    /// <summary>
    /// Retrieves the content structure of a course with user-specific progress.
    /// </summary>
    /// <param name="id">The unique identifier of the course.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The course contents and user progress.</returns>
    [HttpGet("{id}/content")]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "The course contents and user progress.",
        typeof(CourseContentResponseDto)
    )]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid course ID.", typeof(ErrorResponse))]
    [SwaggerResponse(
        StatusCodes.Status401Unauthorized,
        "User is not authenticated.",
        typeof(ErrorResponse)
    )]
    [SwaggerResponse(
        StatusCodes.Status403Forbidden,
        "User does not have permission.",
        typeof(ErrorResponse)
    )]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Course not found.", typeof(ErrorResponse))]
    [SwaggerResponse(
        StatusCodes.Status500InternalServerError,
        "Internal server error.",
        typeof(ErrorResponse)
    )]
    [FeatureAuthorize("VIEW_COURSE_CONTENT")]
    public async Task<IActionResult> GetContents(
        [FromRoute(Name = "id")] Guid id,
        CancellationToken cancellationToken
    )
    {
        _logger.LogDebug("Received request for course contents: {Id}", id);
        CourseContentResponseDto result = await _mediator.Send(
            new GetCourseContentQuery(id),
            cancellationToken
        );
        _logger.LogDebug("Completed request for course contents: {Id}", id);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves the current user's overall progress in a specific course.
    /// </summary>
    /// <param name="id">The unique identifier of the course.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The course progress details.</returns>
    [HttpGet("{id}/progress")]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "The course progress details.",
        typeof(CourseProgressResponseDto)
    )]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid course ID.", typeof(ErrorResponse))]
    [SwaggerResponse(
        StatusCodes.Status401Unauthorized,
        "User is not authenticated.",
        typeof(ErrorResponse)
    )]
    [SwaggerResponse(
        StatusCodes.Status403Forbidden,
        "User does not have permission.",
        typeof(ErrorResponse)
    )]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Enrollment not found.", typeof(ErrorResponse))]
    [SwaggerResponse(
        StatusCodes.Status500InternalServerError,
        "Internal server error.",
        typeof(ErrorResponse)
    )]
    [FeatureAuthorize("VIEW_COURSE_PROGRESS")]
    public async Task<IActionResult> GetProgress(
        [FromRoute(Name = "id")] Guid id,
        CancellationToken cancellationToken
    )
    {
        _logger.LogDebug("Received request for course progress: {Id}", id);
        CourseProgressResponseDto result = await _mediator.Send(
            new GetCourseProgressQuery(id),
            cancellationToken
        );
        _logger.LogDebug("Completed request for course progress: {Id}", id);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves the assessment history for a specific course for the current user.
    /// </summary>
    /// <param name="id">The unique identifier of the course.</param>
    /// <param name="page">The page number (default 1).</param>
    /// <param name="pageSize">The number of records per page (default 20).</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A paginated list of assessment history records.</returns>
    [HttpGet("{id}/assessment-history")]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "A paginated list of assessment history records.",
        typeof(PaginatedAssessmentHistoryDto)
    )]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid course ID.", typeof(ErrorResponse))]
    [SwaggerResponse(
        StatusCodes.Status401Unauthorized,
        "User is not authenticated.",
        typeof(ErrorResponse)
    )]
    [SwaggerResponse(
        StatusCodes.Status403Forbidden,
        "User does not have permission.",
        typeof(ErrorResponse)
    )]
    [SwaggerResponse(
        StatusCodes.Status500InternalServerError,
        "Internal server error.",
        typeof(ErrorResponse)
    )]
    [FeatureAuthorize("VIEW_ASSESSMENT_HISTORY")]
    public async Task<IActionResult> GetAssessmentHistory(
        [FromRoute(Name = "id")] Guid id,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogDebug("Received request for assessment history for course: {CourseId}", id);
        PaginatedAssessmentHistoryDto result = await _mediator.Send(
            new GetCourseAssessmentHistoryQuery(id, page, pageSize),
            cancellationToken
        );
        _logger.LogDebug("Completed request for assessment history for course: {CourseId}", id);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves categories that have at least one active course.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A list of active categories.</returns>
    [HttpGet("active-category")]
    [AllowAnonymous]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "Active categories retrieved successfully.",
        typeof(List<RefTermDto>)
    )]
    [SwaggerResponse(
        StatusCodes.Status500InternalServerError,
        "Internal server error.",
        typeof(ErrorResponse)
    )]
    public async Task<IActionResult> GetActiveCategory(CancellationToken cancellationToken)
    {
        _logger.LogDebug("Received request for active categories.");

        List<RefTermDto> result = await _mediator.Send(
            new GetActiveCategoryQuery(),
            cancellationToken
        );

        _logger.LogDebug("Completed request for active categories.");
        return Ok(result);
    }
}
