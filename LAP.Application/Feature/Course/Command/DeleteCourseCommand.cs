using FluentValidation;
using LAP.Application.DTO.Common;
using LAP.Application.Interface;
using LAP.Application.Interface.IService;
using LAP.Shared.Exceptions;
using MediatR;

namespace LAP.Application.Feature.Course.Command;

/// <summary>Command to delete a course.</summary>
/// <param name="Id">The unique identifier of the course to delete.</param>
public record DeleteCourseCommand(Guid Id) : IRequest<SuccessResponse>;

/// <summary>Validates <see cref="DeleteCourseCommand"/> rules before processing.</summary>
public class DeleteCourseCommandValidator : AbstractValidator<DeleteCourseCommand>
{
    /// <summary>
    /// Initializes validation rules for the <see cref="DeleteCourseCommand"/>.
    /// </summary>
    public DeleteCourseCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Course ID is required");
    }
}

/// <summary>Handles deletion of a course.</summary>
public class DeleteCourseHandler : IRequestHandler<DeleteCourseCommand, SuccessResponse>
{
    private readonly ICourseService _courseService;
    private readonly ICustomLogger<DeleteCourseHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteCourseHandler"/> class.
    /// </summary>
    /// <param name="courseService">Service used to delete course data.</param>
    /// <param name="logger">Custom application logger.</param>
    public DeleteCourseHandler(
        ICourseService courseService,
        ICustomLogger<DeleteCourseHandler> logger
    )
    {
        _courseService = courseService;
        _logger = logger;
    }

    /// <summary>
    /// Handles the course deletion request.
    /// </summary>
    /// <param name="request">The <see cref="DeleteCourseCommand"/> containing the course identifier.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A <see cref="SuccessResponse"/> containing the deleted course ID and a confirmation message.</returns>
    public async Task<SuccessResponse> Handle(
        DeleteCourseCommand request,
        CancellationToken cancellationToken
    )
    {
        _logger.LogInfo("Deleting course {CourseId}.", request.Id);

        int affected = await _courseService.DeleteCourseAsync(request.Id, cancellationToken);

        if (affected == 0)
        {
            _logger.LogError(
                "Course deletion failed because course {CourseId} was not found.",
                request.Id
            );

            throw new NotFoundException(
                "Course not found",
                $"No course found with id {request.Id}"
            );
        }

        _logger.LogInfo("Course deleted successfully with id {CourseId}.", request.Id);

        return new SuccessResponse { Id = request.Id, Message = "Course deleted successfully" };
    }
}
