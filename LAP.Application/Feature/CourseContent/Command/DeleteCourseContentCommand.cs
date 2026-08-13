using FluentValidation;
using LAP.Application.DTO.Common;
using LAP.Application.Interface;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using LAP.Shared.Exceptions;
using MediatR;

namespace LAP.Application.Feature.CourseContent.Command;

/// <summary>Command to delete course content.</summary>
/// <param name="Id">The unique identifier of the course content to delete.</param>
public record DeleteCourseContentCommand(Guid Id) : IRequest<SuccessResponse>;

/// <summary>Validates <see cref="DeleteCourseContentCommand"/> rules before processing.</summary>
public class DeleteCourseContentValidator : AbstractValidator<DeleteCourseContentCommand>
{
    /// <summary>
    /// Initializes validation rules for the <see cref="DeleteCourseContentCommand"/>.
    /// </summary>
    public DeleteCourseContentValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Content ID is required");
    }
}

/// <summary>Handles deletion of course content.</summary>
public class DeleteCourseContentHandler
    : IRequestHandler<DeleteCourseContentCommand, SuccessResponse>
{
    private readonly ICustomLogger<DeleteCourseContentHandler> _logger;
    private readonly ICourseContentService _courseContentService;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteCourseContentHandler"/> class.
    /// </summary>
    /// <param name="logger">Custom application logger.</param>
    /// <param name="courseContentService">Service used to delete course content data.</param>
    public DeleteCourseContentHandler(
        ICustomLogger<DeleteCourseContentHandler> logger,
        ICourseContentService courseContentService
    )
    {
        _logger = logger;
        _courseContentService = courseContentService;
    }

    /// <summary>
    /// Handles the course content deletion request.
    /// </summary>
    /// <param name="request">The <see cref="DeleteCourseContentCommand"/> containing the content identifier.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A <see cref="SuccessResponse"/> containing the deleted content ID and a confirmation message.</returns>
    public async Task<SuccessResponse> Handle(
        DeleteCourseContentCommand request,
        CancellationToken cancellationToken
    )
    {
        _logger.LogInfo("Deleting course content {ContentId}.", request.Id);

        int affected = await _courseContentService.DeleteAsync(request.Id, cancellationToken);

        if (affected == 0)
        {
            _logger.LogError(
                "Course content deletion failed because content {ContentId} was not found.",
                request.Id
            );

            throw new NotFoundException(
                "Course content not found",
                $"No course content found with id {request.Id}"
            );
        }

        _logger.LogInfo("Deleted course content {ContentId}.", request.Id);

        return new SuccessResponse
        {
            Id = request.Id,
            Message = "Course content deleted successfully",
        };
    }
}
