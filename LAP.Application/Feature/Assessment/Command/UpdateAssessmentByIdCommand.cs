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
/// Command to update an existing assessment by its unique identifier.
/// </summary>
/// <param name="Id">The assessment identifier.</param>
/// <param name="Dto">The updated assessment details.</param>
public record UpdateAssessmentByIdCommand(Guid Id, UpdateAssessmentRequestDto Dto)
    : IRequest<SuccessResponse>;

/// <summary>
/// Validator for <see cref="UpdateAssessmentByIdCommand"/>.
/// </summary>
public class UpdateAssessmentByIdValidator : AbstractValidator<UpdateAssessmentByIdCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateAssessmentByIdValidator"/> class.
    /// </summary>
    public UpdateAssessmentByIdValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Assessment ID is required");

        RuleFor(x => x.Dto.Title)
            .NotEmpty()
            .MaximumLength(200)
            .WithMessage("Title is required and cannot exceed 200 characters")
            .When(x => x.Dto.Title != null);

        RuleFor(x => x.Dto.Description)
            .MaximumLength(1000)
            .WithMessage("Description cannot exceed 1000 characters")
            .When(x => x.Dto.Description != null);

        RuleFor(x => x.Dto.TotalMark)
            .GreaterThan(0)
            .WithMessage("Total mark must be greater than 0")
            .When(x => x.Dto.TotalMark != null);

        RuleFor(x => x.Dto.PassingMark)
            .GreaterThan(0)
            .WithMessage("Passing mark must be greater than 0")
            .When(x => x.Dto.PassingMark != null);

        RuleFor(x => x.Dto.PassingMark)
            .LessThanOrEqualTo(x => x.Dto.TotalMark.Value)
            .WithMessage("Passing mark cannot exceed total mark")
            .When(x => x.Dto.PassingMark != null && x.Dto.TotalMark != null);

        RuleFor(x => x.Dto.DurationMinute)
            .GreaterThan(0)
            .WithMessage("Duration must be greater than 0")
            .When(x => x.Dto.DurationMinute != null);
    }
}

/// <summary>
/// Handler for <see cref="UpdateAssessmentByIdCommand"/>.
/// </summary>
public class UpdateAssessmentByIdHandler
    : IRequestHandler<UpdateAssessmentByIdCommand, SuccessResponse>
{
    private readonly IAssessmentService _assessmentService;
    private readonly IMapper _mapper;
    private readonly ICustomLogger<UpdateAssessmentByIdHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateAssessmentByIdHandler"/> class.
    /// </summary>
    public UpdateAssessmentByIdHandler(
        IAssessmentService assessmentService,
        IMapper mapper,
        ICustomLogger<UpdateAssessmentByIdHandler> logger
    )
    {
        _assessmentService = assessmentService;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Handles the assessment update request.
    /// </summary>
    public async Task<SuccessResponse> Handle(
        UpdateAssessmentByIdCommand request,
        CancellationToken cancellationToken
    )
    {
        _logger.LogInfo("Processing update request for assessment {AssessmentId}", request.Id);

        LAP.Domain.Entity.Assessment? assessment = await _assessmentService.GetAssessmentByIdAsync(
            request.Id,
            cancellationToken
        );
        if (assessment == null)
        {
            _logger.LogError("Assessment not found for {AssessmentId}", request.Id);
            throw new NotFoundException(
                "Assessment not found",
                $"The assessment with ID {request.Id} does not exist."
            );
        }

        _mapper.Map(request.Dto, assessment);

        await _assessmentService.UpdateAssessmentAsync(assessment, cancellationToken);

        _logger.LogInfo("Successfully updated assessment ID {AssessmentId}", request.Id);

        return new SuccessResponse { Id = request.Id, Message = "Assessment updated successfully" };
    }
}
