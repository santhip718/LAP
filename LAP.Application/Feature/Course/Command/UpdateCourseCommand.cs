using AutoMapper;
using FluentValidation;
using LAP.Application.DTO.Common;
using LAP.Application.DTO.Course;
using LAP.Application.Interface;
using LAP.Application.Interface.IContext;
using LAP.Application.Interface.IService;
using LAP.Shared.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using CourseEntity = LAP.Domain.Entity.Course;

namespace LAP.Application.Feature.Course.Command;

/// <summary>
/// Command for updating an existing course with the provided details.
/// </summary>
/// <param name="Id">The identifier of the course to update.</param>
/// <param name="Dto">The course update request data transfer object.</param>
public record UpdateCourseCommand(Guid Id, UpdateCourseRequestDto Dto) : IRequest<SuccessResponse>;

/// <summary>
/// Validates the <see cref="UpdateCourseCommand"/> before it is handled.
/// </summary>
public class UpdateCourseCommandValidator : AbstractValidator<UpdateCourseCommand>
{
    /// <summary>
    /// Initializes validation rules for the <see cref="UpdateCourseCommand"/>.
    /// </summary>
    public UpdateCourseCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Course ID is required");

        When(
            x => x.Dto.Title is not null,
            () =>
            {
                RuleFor(x => x.Dto.Title)
                    .NotEmpty()
                    .WithMessage("Title is required")
                    .MaximumLength(200)
                    .WithMessage("Title cannot exceed 200 characters");
            }
        );

        When(
            x => x.Dto.Description is not null,
            () =>
            {
                RuleFor(x => x.Dto.Description).NotEmpty().WithMessage("Description is required");
            }
        );

        When(
            x => x.Dto.CategoryId.HasValue,
            () =>
            {
                RuleFor(x => x.Dto.CategoryId).NotEmpty().WithMessage("Category is required");
            }
        );

        When(
            x => x.Dto.DifficultyLevelId.HasValue,
            () =>
            {
                RuleFor(x => x.Dto.DifficultyLevelId)
                    .NotEmpty()
                    .WithMessage("Difficulty level is required");
            }
        );

        When(
            x => x.Dto.DurationMinute.HasValue,
            () =>
            {
                RuleFor(x => x.Dto.DurationMinute)
                    .GreaterThan(0)
                    .WithMessage("Duration must be greater than 0");
            }
        );
    }
}

/// <summary>
/// Handles the <see cref="UpdateCourseCommand"/> by updating the matching course entity.
/// </summary>
public class UpdateCourseHandler : IRequestHandler<UpdateCourseCommand, SuccessResponse>
{
    private readonly ICourseService _courseService;
    private readonly IMapper _mapper;
    private readonly ITransactionService _transactionService;
    private readonly ICustomLogger<UpdateCourseHandler> _logger;
    private readonly IRequestContext _requestContext;
    private readonly IFileService _fileService;

    public UpdateCourseHandler(
        ICourseService courseService,
        IMapper mapper,
        ITransactionService transactionService,
        ICustomLogger<UpdateCourseHandler> logger,
        IRequestContext requestContext,
        IFileService fileService
    )
    {
        _courseService = courseService;
        _mapper = mapper;
        _transactionService = transactionService;
        _logger = logger;
        _requestContext = requestContext;
        _fileService = fileService;
    }

    /// <summary>
    /// Handles the course update request and returns a success response after the course is updated.
    /// </summary>
    /// <param name="request">The <see cref="UpdateCourseCommand"/> containing the course identifier and update data.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>
    /// A <see cref="SuccessResponse"/> containing the updated course ID and a confirmation message.
    /// </returns>
    public async Task<SuccessResponse> Handle(
        UpdateCourseCommand request,
        CancellationToken cancellationToken
    )
    {
        UpdateCourseRequestDto dto = request.Dto;

        _logger.LogInfo("Updating course {CourseId}.", request.Id);

        CourseEntity? course = await _courseService.GetCourseByIdAsync(
            request.Id,
            cancellationToken
        );

        if (course is null)
        {
            _logger.LogError(
                "Course update failed because course {CourseId} was not found.",
                request.Id
            );

            throw new NotFoundException(
                "Course not found",
                $"No course found with id {request.Id}"
            );
        }

        if (dto.Title is not null)
        {
            Guid categoryId = dto.CategoryId ?? course.CategoryId;

            bool nameExists = await _courseService.IsCourseNameExistAsync(
                dto.Title,
                categoryId,
                request.Id,
                cancellationToken
            );

            if (nameExists)
            {
                _logger.LogError(
                    "Course update failed: course '{Title}' already exists in category {CategoryId}.",
                    dto.Title,
                    categoryId
                );

                throw new ConflictException(
                    "Course already exists",
                    $"A course with the name '{dto.Title}' already exists in the selected category."
                );
            }
        }

        _mapper.Map(dto, course);

        if (dto.ThumbnailImg is not null && dto.ThumbnailImg.Length > 0)
        {
            course.ThumbnailImgPath = await _fileService.SaveFileAsync(
                dto.ThumbnailImg,
                course.Id.ToString(),
                cancellationToken
            );
        }

        _courseService.UpdateCourse(course);
        await _transactionService.SaveChangesAsync(cancellationToken);

        _logger.LogInfo("Course updated successfully with id {CourseId}.", request.Id);

        return new SuccessResponse { Id = request.Id, Message = "Course updated successfully" };
    }
}
