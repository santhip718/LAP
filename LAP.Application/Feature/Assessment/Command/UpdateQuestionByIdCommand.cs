using AutoMapper;
using FluentValidation;
using LAP.Application.DTO.Assessment;
using LAP.Application.DTO.Common;
using LAP.Application.Interface;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using LAP.Shared.Exceptions;
using MediatR;

namespace LAP.Application.Feature.Assessment.Command;

/// <summary>
/// Command to update an existing question by its unique identifier.
/// </summary>
/// <param name="Id">The question identifier.</param>
/// <param name="Dto">The updated question details.</param>
public record UpdateQuestionByIdCommand(Guid Id, UpdateQuestionRequestDto Dto)
    : IRequest<SuccessResponse>;

/// <summary>
/// Validator for <see cref="UpdateQuestionByIdCommand"/>.
/// </summary>
public class UpdateQuestionByIdValidator : AbstractValidator<UpdateQuestionByIdCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateQuestionByIdValidator"/> class.
    /// </summary>
    public UpdateQuestionByIdValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Question ID is required");

        RuleFor(x => x.Dto.QuestionText)
            .NotEmpty()
            .MaximumLength(1000)
            .WithMessage("Question text is required and cannot exceed 1000 characters")
            .When(x => x.Dto.QuestionText != null);

        RuleFor(x => x.Dto.Answer)
            .NotEmpty()
            .WithMessage("Answer is required")
            .When(x => x.Dto.Answer != null);

        RuleFor(x => x.Dto.Weight)
            .GreaterThan(0)
            .WithMessage("Weight must be greater than 0")
            .When(x => x.Dto.Weight != null);

        RuleFor(x => x.Dto.QuestionTypeId)
            .NotEmpty()
            .WithMessage("Question Type ID is required")
            .When(x => x.Dto.QuestionTypeId != null);
    }
}

/// <summary>
/// Handler for <see cref="UpdateQuestionByIdCommand"/>.
/// </summary>
public class UpdateQuestionByIdHandler : IRequestHandler<UpdateQuestionByIdCommand, SuccessResponse>
{
    private readonly IAssessmentService _assessmentService;
    private readonly IMapper _mapper;
    private readonly ICustomLogger<UpdateQuestionByIdHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateQuestionByIdHandler"/> class.
    /// </summary>
    public UpdateQuestionByIdHandler(
        IAssessmentService assessmentService,
        IMapper mapper,
        ICustomLogger<UpdateQuestionByIdHandler> logger
    )
    {
        _assessmentService = assessmentService;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Handles the question update request.
    /// </summary>
    public async Task<SuccessResponse> Handle(
        UpdateQuestionByIdCommand request,
        CancellationToken cancellationToken
    )
    {
        _logger.LogInfo("Processing update request for question ID {QuestionId}", request.Id);

        Question? question = await _assessmentService.GetQuestionByIdAsync(
            request.Id,
            cancellationToken
        );
        if (question == null)
        {
            _logger.LogError("Question not found for {QuestionId}", request.Id);
            throw new NotFoundException(
                "Question not found",
                $"The question with ID {request.Id} does not exist."
            );
        }

        _mapper.Map(request.Dto, question);

        if (
            !string.IsNullOrEmpty(request.Dto.MetaTopicId)
            && Guid.TryParse(request.Dto.MetaTopicId, out Guid metaTopicId)
        )
        {
            question.MetaTopicId = metaTopicId;
        }

        await _assessmentService.UpdateQuestionAsync(question, cancellationToken);

        _logger.LogInfo("Successfully updated question ID {QuestionId}", request.Id);

        return new SuccessResponse { Id = request.Id, Message = "Question updated successfully" };
    }
}
