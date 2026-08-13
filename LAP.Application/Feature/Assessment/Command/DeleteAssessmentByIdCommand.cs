using FluentValidation;
using LAP.Application.DTO.Common;
using LAP.Application.Interface;
using LAP.Application.Interface.IService;
using LAP.Shared.Exceptions;
using MediatR;

namespace LAP.Application.Feature.Assessment.Command;

/// <summary>
/// Command to delete an assessment by its unique identifier.
/// </summary>
/// <param name="Id">The assessment identifier.</param>
public record DeleteAssessmentByIdCommand(Guid Id) : IRequest<SuccessResponse>;

/// <summary>
/// Validator for <see cref="DeleteAssessmentByIdCommand"/>.
/// </summary>
public class DeleteAssessmentByIdValidator : AbstractValidator<DeleteAssessmentByIdCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteAssessmentByIdValidator"/> class.
    /// </summary>
    public DeleteAssessmentByIdValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Assessment ID is required");
    }
}

/// <summary>
/// Handler for <see cref="DeleteAssessmentByIdCommand"/>.
/// </summary>
public class DeleteAssessmentByIdHandler
    : IRequestHandler<DeleteAssessmentByIdCommand, SuccessResponse>
{
    private readonly IAssessmentService _assessmentService;
    private readonly ICustomLogger<DeleteAssessmentByIdHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteAssessmentByIdHandler"/> class.
    /// </summary>
    public DeleteAssessmentByIdHandler(
        IAssessmentService assessmentService,
        ICustomLogger<DeleteAssessmentByIdHandler> logger
    )
    {
        _assessmentService = assessmentService;
        _logger = logger;
    }

    /// <summary>
    /// Handles the assessment deletion request.
    /// </summary>
    public async Task<SuccessResponse> Handle(
        DeleteAssessmentByIdCommand request,
        CancellationToken cancellationToken
    )
    {
        _logger.LogError("Processing delete request for assessment ID {AssessmentId}", request.Id);

        int rowsAffected = await _assessmentService.DeleteAssessmentAsync(
            request.Id,
            cancellationToken
        );

        if (rowsAffected == 0)
        {
            _logger.LogError(
                "Failed to delete assessment ID {AssessmentId}. No rows affected.",
                request.Id
            );
            throw new NotFoundException(
                "Assessment not found",
                $"The assessment with ID {request.Id} could not be deleted as it may have already been removed."
            );
        }

        _logger.LogInfo("Successfully deleted assessment ID {AssessmentId}", request.Id);

        return new SuccessResponse { Id = request.Id, Message = "Assessment deleted successfully" };
    }
}
