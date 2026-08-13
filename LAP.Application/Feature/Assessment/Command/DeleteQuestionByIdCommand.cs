using FluentValidation;
using LAP.Application.DTO.Common;
using LAP.Application.Interface;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using LAP.Shared.Exceptions;
using MediatR;

namespace LAP.Application.Feature.Assessment.Command;

/// <summary>
/// Command to delete a question by its unique identifier.
/// </summary>
/// <param name="Id">The question identifier.</param>
public record DeleteQuestionByIdCommand(Guid Id) : IRequest<SuccessResponse>;

/// <summary>
/// Validator for <see cref="DeleteQuestionByIdCommand"/>.
/// </summary>
public class DeleteQuestionByIdValidator : AbstractValidator<DeleteQuestionByIdCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteQuestionByIdValidator"/> class.
    /// </summary>
    public DeleteQuestionByIdValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Question ID is required");
    }
}

/// <summary>
/// Handler for <see cref="DeleteQuestionByIdCommand"/>.
/// </summary>
public class DeleteQuestionByIdHandler : IRequestHandler<DeleteQuestionByIdCommand, SuccessResponse>
{
    private readonly IAssessmentService _assessmentService;
    private readonly ICustomLogger<DeleteQuestionByIdHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteQuestionByIdHandler"/> class.
    /// </summary>
    public DeleteQuestionByIdHandler(
        IAssessmentService assessmentService,
        ICustomLogger<DeleteQuestionByIdHandler> logger
    )
    {
        _assessmentService = assessmentService;
        _logger = logger;
    }

    /// <summary>
    /// Handles the question deletion request and recalculates the assessment's total marks.
    /// </summary>
    public async Task<SuccessResponse> Handle(
        DeleteQuestionByIdCommand request,
        CancellationToken cancellationToken
    )
    {
        _logger.LogInfo("Processing delete request for question ID {QuestionId}", request.Id);

        Question? question = await _assessmentService.GetQuestionByIdAsync(
            request.Id,
            cancellationToken
        );

        if (question == null)
        {
            _logger.LogError("Question not found for question ID {QuestionId}", request.Id);
            throw new NotFoundException(
                "Question not found",
                $"The question with ID {request.Id} could not be deleted as it may have already been removed."
            );
        }

        Guid assessmentId = question.AssessmentId;

        int rowsAffected = await _assessmentService.DeleteQuestionAsync(
            request.Id,
            cancellationToken
        );

        if (rowsAffected == 0)
        {
            _logger.LogError(
                "Failed to delete question ID {QuestionId}. No rows affected.",
                request.Id
            );
            throw new NotFoundException(
                "Question not found",
                $"The question with ID {request.Id} could not be deleted as it may have already been removed."
            );
        }

        await RecalculateAssessmentTotalMarkAsync(assessmentId, cancellationToken);

        _logger.LogInfo("Successfully deleted question ID {QuestionId}", request.Id);

        return new SuccessResponse { Id = request.Id, Message = "Question deleted successfully" };
    }

    /// <summary>
    /// Recalculates and updates the total marks for the specified assessment based on remaining active questions.
    /// </summary>
    /// <param name="assessmentId">The assessment identifier.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    private async Task RecalculateAssessmentTotalMarkAsync(
        Guid assessmentId,
        CancellationToken cancellationToken
    )
    {
        LAP.Domain.Entity.Assessment? assessment = await _assessmentService.GetAssessmentByIdAsync(
            assessmentId,
            cancellationToken
        );

        if (assessment is null)
        {
            _logger.LogError(
                "Assessment {AssessmentId} not found during total mark recalculation",
                assessmentId
            );
            return;
        }

        int remainingQuestionCount =
            await _assessmentService.CountActiveQuestionByAssessmentIdAsync(
                assessmentId,
                cancellationToken
            );

        assessment.TotalMark = remainingQuestionCount;

        await _assessmentService.UpdateAssessmentAsync(assessment, cancellationToken);

        _logger.LogInfo(
            "Recalculated total marks for assessment {AssessmentId} to {TotalMark}",
            assessmentId,
            remainingQuestionCount
        );
    }
}
