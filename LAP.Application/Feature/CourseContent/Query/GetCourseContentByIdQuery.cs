using AutoMapper;
using FluentValidation;
using LAP.Application.DTO.CourseContent;
using LAP.Application.Interface;
using LAP.Application.Interface.IContext;
using LAP.Application.Interface.IService;
using LAP.Shared.Exceptions;
using MediatR;
using CourseContentEntity = LAP.Domain.Entity.CourseContent;
using Enrollment = LAP.Domain.Entity.Enrollment;
using UserCourseProgress = LAP.Domain.Entity.UserCourseProgress;

namespace LAP.Application.Feature.CourseContent.Query;

/// <summary>
/// Query to retrieve complete details of a course content item.
/// </summary>
/// <param name="Id">The unique identifier of the course content.</param>
public record GetCourseContentByIdQuery(Guid Id) : IRequest<CourseContentDetailDto>;

/// <summary>
/// Validates the <see cref="GetCourseContentByIdQuery"/>.
/// </summary>
public class GetCourseContentByIdValidator : AbstractValidator<GetCourseContentByIdQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetCourseContentByIdValidator"/> class.
    /// </summary>
    public GetCourseContentByIdValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Course content identifier is required");
    }
}

/// <summary>
/// Handles the retrieval of course content details.
/// </summary>
public class GetCourseContentByIdHandler
    : IRequestHandler<GetCourseContentByIdQuery, CourseContentDetailDto>
{
    private readonly ICourseContentService _courseContentService;
    private readonly IMapper _mapper;
    private readonly IRequestContext _requestContext;
    private readonly ICustomLogger<GetCourseContentByIdHandler> _logger;
    private readonly IFileStorageService _fileStorageService;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetCourseContentByIdHandler"/> class.
    /// </summary>
    public GetCourseContentByIdHandler(
        ICourseContentService courseContentService,
        IMapper mapper,
        IRequestContext requestContext,
        ICustomLogger<GetCourseContentByIdHandler> logger,
        IFileStorageService fileStorageService
    )
    {
        _courseContentService = courseContentService;
        _mapper = mapper;
        _requestContext = requestContext;
        _logger = logger;
        _fileStorageService = fileStorageService;
    }

    /// <summary>
    /// Processes the course content retrieval request.
    /// </summary>
    /// <param name="request">The get course content by identifier query.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="CourseContentDetailDto"/> containing the detailed content information.</returns>
    public async Task<CourseContentDetailDto> Handle(
        GetCourseContentByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        _logger.LogInfo("Started fetching course content {ContentId}.", request.Id);

        CourseContentEntity? content = await _courseContentService.GetContentWithMetaTopicAsync(
            request.Id,
            cancellationToken
        );

        if (content == null)
        {
            _logger.LogError("Course content {ContentId} not found.", request.Id);
            throw new NotFoundException(
                "Course content not found",
                $"Course content with ID {request.Id} does not exist."
            );
        }

        CourseContentDetailDto result = _mapper.Map<CourseContentDetailDto>(content);

        if (!string.IsNullOrEmpty(content.PdfFilePath))
        {
            result.PdfBase64 = await _fileStorageService.GetBase64Async(content.PdfFilePath);
        }

        Guid userId = _requestContext.UserId.Value;

        LAP.Domain.Entity.Enrollment? enrollment =
            await _courseContentService.GetEnrollmentByUserAndCourseAsync(
                userId,
                content.MetaTopic.CourseId,
                cancellationToken
            );

        if (enrollment != null)
        {
            UserCourseProgress? progress = await _courseContentService.GetProgressAsync(
                enrollment.Id,
                content.Id,
                cancellationToken
            );

            if (progress != null)
            {
                result.IsCompleted = progress.IsCompleted;
                result.CompletedOn = progress.CompletedOn;
            }
        }

        CourseContentEntity? previousContent = await _courseContentService.GetPreviousContentAsync(
            content.MetaTopic.CourseId,
            content.MetaTopic.SequenceOrder,
            content.SequenceOrder,
            cancellationToken
        );

        CourseContentEntity? nextContent = await _courseContentService.GetNextContentAsync(
            content.MetaTopic.CourseId,
            content.MetaTopic.SequenceOrder,
            content.SequenceOrder,
            cancellationToken
        );

        result.PreviousContentId = previousContent?.Id;
        result.NextContentId = nextContent?.Id;

        _logger.LogInfo("Completed fetching course content {ContentId}.", request.Id);

        return result;
    }
}
