using System.IO;
using AutoMapper;
using FluentValidation;
using LAP.Application.DTO.Common;
using LAP.Application.DTO.Course;
using LAP.Application.Interface;
using LAP.Application.Interface.IContext;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using MediatR;
using Microsoft.AspNetCore.Http;
using CourseEntity = LAP.Domain.Entity.Course;

namespace LAP.Application.Feature.Course.Command;

/// <summary>
/// Command for creating a new course with the provided details.
/// </summary>
/// <param name="Dto">The course creation request data transfer object.</param>
public record CreateCourseCommand(CreateCourseRequestDto Dto) : IRequest<SuccessResponse>;

/// <summary>
/// Validates the <see cref="CreateCourseCommand"/> before it is handled.
/// </summary>
public class CreateCourseValidator : AbstractValidator<CreateCourseCommand>
{
    /// <summary>
    /// Initializes validation rules for the <see cref="CreateCourseCommand"/>.
    /// </summary>
    public CreateCourseValidator()
    {
        RuleFor(x => x.Dto.Title)
            .NotEmpty()
            .WithMessage("Title is required")
            .MaximumLength(200)
            .WithMessage("Title cannot exceed 200 characters");
        RuleFor(x => x.Dto.Description).NotEmpty().WithMessage("Description is required");
        RuleFor(x => x.Dto.CategoryId).NotEmpty().WithMessage("Category is required");
        RuleFor(x => x.Dto.DifficultyLevelId)
            .NotEmpty()
            .WithMessage("Difficulty level is required");
        RuleFor(x => x.Dto.DurationMinute)
            .GreaterThan(0)
            .WithMessage("Duration must be greater than 0");
    }
}

/// <summary>
/// Handles the <see cref="CreateCourseCommand"/> by persisting a new course entity
/// and optionally uploading a thumbnail image.
/// </summary>
public class CreateCourseHandler : IRequestHandler<CreateCourseCommand, SuccessResponse>
{
    private readonly ICourseService _courseService;
    private readonly IMapper _mapper;
    private readonly ICustomLogger<CreateCourseHandler> _logger;
    private readonly ITransactionService _transactionService;
    private readonly IRequestContext _requestContext;
    private readonly IFileService _fileService;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateCourseHandler"/> class.
    /// </summary>
    /// <param name="courseService">Service used to persist course data.</param>
    /// <param name="mapper">Mapper used to convert DTO data into course entities.</param>
    /// <param name="logger">Custom application logger.</param>
    /// <param name="transactionService">Service used to manage transactional operations.</param>
    /// <param name="requestContext">Context containing the current request user information.</param>
    /// <param name="fileStorage">Service used to store uploaded course files.</param>
    public CreateCourseHandler(
        ICourseService courseService,
        IMapper mapper,
        ICustomLogger<CreateCourseHandler> logger,
        ITransactionService transactionService,
        IRequestContext requestContext,
        IFileService fileService
    )
    {
        _courseService = courseService;
        _mapper = mapper;
        _logger = logger;
        _transactionService = transactionService;
        _requestContext = requestContext;
        _fileService = fileService;
    }

    /// <summary>
    /// Handles the course creation request by mapping the DTO, uploading the thumbnail
    /// if provided, persisting the entity, and returning the new course identifier.
    /// </summary>
    /// <param name="request">The <see cref="CreateCourseCommand"/> containing the course data.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>
    /// A <see cref="SuccessResponse"/> containing the newly created course ID and a confirmation message.
    /// </returns>
    public async Task<SuccessResponse> Handle(
        CreateCourseCommand request,
        CancellationToken cancellationToken
    )
    {
        CreateCourseRequestDto dto = request.Dto;

        _logger.LogInfo("Course creation initiated for title {Title}.", dto.Title);

        bool nameExists = await _courseService.IsCourseNameExistAsync(
            dto.Title,
            dto.CategoryId,
            cancellationToken: cancellationToken
        );

        if (nameExists)
        {
            _logger.LogError(
                "Course creation failed: course '{Title}' already exists in category {CategoryId}.",
                dto.Title,
                dto.CategoryId
            );

            throw new LAP.Shared.Exceptions.ConflictException(
                "Course already exists",
                $"A course with the name '{dto.Title}' already exists in the selected category."
            );
        }

        CourseEntity course = _mapper.Map<CourseEntity>(dto);

        course.OverallRating = 0;
        course.CreatedByUserId = _requestContext.UserId ?? Guid.Empty;
        course.Id = Guid.NewGuid();

        _logger.LogInfo(
            "Course entity mapped for title {Title} and created by user {UserId}.",
            dto.Title,
            course.CreatedByUserId
        );

        if (dto.ThumbnailImg is not null && dto.ThumbnailImg.Length > 0)
        {
            _logger.LogInfo(
                "Thumbnail upload started for course {Title} and file {FileName}.",
                dto.Title,
                dto.ThumbnailImg.FileName
            );

            course.ThumbnailImgPath = await _fileService.SaveFileAsync(
                dto.ThumbnailImg,
                course.Id.ToString(),
                cancellationToken
            );

            _logger.LogInfo(
                "Thumbnail uploaded successfully for course {Title} and saved path {SavedPath}.",
                dto.Title,
                course.ThumbnailImgPath
            );
        }

        await _courseService.AddCourseAsync(course, cancellationToken);
        await _transactionService.SaveChangesAsync(cancellationToken);

        _logger.LogInfo(
            "Course created successfully with id {CourseId} and title {Title}.",
            course.Id,
            course.Title
        );

        return new SuccessResponse { Id = course.Id, Message = "Course created successfully" };
    }
}
