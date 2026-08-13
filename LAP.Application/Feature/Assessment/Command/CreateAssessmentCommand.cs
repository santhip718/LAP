using AutoMapper;
using FluentValidation;
using LAP.Application.Constant;
using LAP.Application.DTO.Assessment;
using LAP.Application.DTO.Common;
using LAP.Application.Interface;
using LAP.Application.Interface.IHelper;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using LAP.Shared.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace LAP.Application.Feature.Assessment.Command;

/// <summary>
/// Command to create a new assessment along with its questions.
/// </summary>
/// <param name="CourseId">The course identifier.</param>
/// <param name="Title">The assessment title.</param>
/// <param name="Description">The assessment description.</param>
/// <param name="PassingMark">The passing marks for the assessment.</param>
/// <param name="DurationMinute">The duration in minutes.</param>
/// <param name="QuestionFile">The uploaded question file (Excel).</param>
public record CreateAssessmentCommand(
    Guid CourseId,
    string Title,
    string? Description,
    int PassingMark,
    int DurationMinute,
    IFormFile QuestionFile
) : IRequest<SuccessResponse>;

/// <summary>
/// Validator for <see cref="CreateAssessmentCommand"/>.
/// </summary>
public class CreateAssessmentValidator : AbstractValidator<CreateAssessmentCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateAssessmentValidator"/> class.
    /// </summary>
    public CreateAssessmentValidator()
    {
        RuleFor(x => x.CourseId).NotEmpty().WithMessage("Course ID is required");

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200)
            .WithMessage("Title is required and cannot exceed 200 characters");

        RuleFor(x => x.PassingMark)
            .GreaterThan(0)
            .WithMessage("Passing mark must be greater than 0 ");

        RuleFor(x => x.DurationMinute)
            .GreaterThan(0)
            .WithMessage("Duration must be greater than 0");

        RuleFor(x => x.QuestionFile)
            .NotNull()
            .WithMessage("Question file is mandatory")
            .Must(file => file.Length > 0)
            .WithMessage("Question file cannot be empty")
            .Must(file =>
            {
                string extension = Path.GetExtension(file.FileName).ToLower();
                return extension == ".xlsx" || extension == ".xls" || extension == ".xlsb";
            })
            .WithMessage("Only Excel files (.xlsx, .xls, .xlsb) are supported");
    }
}

/// <summary>
/// Handler for <see cref="CreateAssessmentCommand"/>.
/// </summary>
public class CreateAssessmentHandler : IRequestHandler<CreateAssessmentCommand, SuccessResponse>
{
    private readonly IAssessmentService _assessmentService;
    private readonly ITransactionService _transactionService;
    private readonly ICustomLogger<CreateAssessmentHandler> _logger;
    private readonly IQuestionParser _questionParser;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateAssessmentHandler"/> class.
    /// </summary>
    public CreateAssessmentHandler(
        IAssessmentService assessmentService,
        ITransactionService transactionService,
        ICustomLogger<CreateAssessmentHandler> logger,
        IQuestionParser questionParser,
        IMapper mapper
    )
    {
        _assessmentService = assessmentService;
        _transactionService = transactionService;
        _logger = logger;
        _questionParser = questionParser;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the assessment creation request.
    /// </summary>
    public async Task<SuccessResponse> Handle(
        CreateAssessmentCommand request,
        CancellationToken cancellationToken
    )
    {
        _logger.LogInfo(
            "Assessment creation started for course {CourseId} with title {Title}",
            request.CourseId,
            request.Title
        );

        bool activeAssessmentExists = await _assessmentService.ActiveAssessmentExistsForCourseAsync(
            request.CourseId,
            cancellationToken
        );
        if (activeAssessmentExists)
        {
            _logger.LogError(
                "An active assessment already exists for course {CourseId}",
                request.CourseId
            );
            throw new BadRequestException(
                "Active assessment already exists",
                $"Course with ID {request.CourseId} already has an active assessment. Please delete the existing assessment before creating a new one."
            );
        }

        List<QuestionImportDto> importedQuestion = await _questionParser.ParseQuestionAsync(
            request.QuestionFile
        );

        if (importedQuestion == null || importedQuestion.Count == 0)
        {
            _logger.LogError("Question file is empty for course {CourseId}", request.CourseId);
            throw new BadRequestException(
                "Empty question file",
                "The uploaded file contains no questions."
            );
        }

        int totalMark = importedQuestion.Count;
        if (request.PassingMark > totalMark)
        {
            _logger.LogError(
                "Passing mark {PassingMark} exceeds total mark {TotalMark}",
                request.PassingMark,
                totalMark
            );
            throw new BadRequestException(
                "Invalid Passing Mark",
                $"Passing mark cannot exceed total mark ({totalMark})."
            );
        }

        List<CourseMetaTopic> metaTopic = await _assessmentService.GetMetaTopicByCourseIdAsync(
            request.CourseId,
            cancellationToken
        );
        List<RefTerm> questionType = await _assessmentService.GetQuestionTypeAsync(
            cancellationToken
        );

        List<(
            QuestionImportDto Item,
            RefTerm QuestionType,
            CourseMetaTopic MetaTopic
        )> preparedQuestion = PrepareQuestion(importedQuestion, metaTopic, questionType);

        Guid resultId = await _transactionService.ExecuteInTransactionAsync(
            async () =>
            {
                LAP.Domain.Entity.Assessment assessment = new()
                {
                    CourseId = request.CourseId,
                    Title = request.Title,
                    Description = request.Description,
                    TotalMark = totalMark,
                    PassingMark = request.PassingMark,
                    DurationMinute = request.DurationMinute,
                };

                await _assessmentService.AddAssessmentAsync(assessment, cancellationToken);
                await _assessmentService.SaveChangesAsync(cancellationToken);

                foreach (
                    (
                        QuestionImportDto Item,
                        RefTerm QuestionType,
                        CourseMetaTopic MetaTopic
                    ) prepared in preparedQuestion
                )
                {
                    Question question = _mapper.Map<Question>(prepared.Item);
                    question.AssessmentId = assessment.Id;
                    question.MetaTopicId = prepared.MetaTopic.Id;
                    question.QuestionTypeId = prepared.QuestionType.Id;

                    await _assessmentService.AddQuestionAsync(question, cancellationToken);
                }
                await _assessmentService.SaveChangesAsync(cancellationToken);

                return assessment.Id;
            },
            cancellationToken
        );

        _logger.LogInfo(
            "Assessment creation completed successfully for assessment {AssessmentId}",
            resultId
        );

        return new SuccessResponse { Id = resultId, Message = "Assessment created successfully" };
    }

    /// <summary>
    /// Prepares the question list by matching each imported question with its meta topic and question type.
    /// </summary>
    /// <param name="importedQuestion">The list of imported questions from the Excel file.</param>
    /// <param name="metaTopic">The list of available meta topics for the course.</param>
    /// <param name="questionType">The list of available question types.</param>
    /// <returns>A list of tuples containing the imported question, its resolved question type, and meta topic.</returns>
    private List<(
        QuestionImportDto Item,
        RefTerm QuestionType,
        CourseMetaTopic MetaTopic
    )> PrepareQuestion(
        List<QuestionImportDto> importedQuestion,
        List<CourseMetaTopic> metaTopic,
        List<RefTerm> questionType
    )
    {
        List<(
            QuestionImportDto Item,
            RefTerm QuestionType,
            CourseMetaTopic MetaTopic
        )> preparedQuestion =
            new List<(QuestionImportDto Item, RefTerm QuestionType, CourseMetaTopic MetaTopic)>();

        foreach (QuestionImportDto item in importedQuestion)
        {
            CourseMetaTopic? foundMetaTopic = metaTopic.FirstOrDefault(x =>
                x.Name.Equals(item.MetaTopicName, StringComparison.OrdinalIgnoreCase)
            );

            if (foundMetaTopic == null)
            {
                _logger.LogError(
                    "MetaTopic '{MetaTopicName}' not found for course",
                    item.MetaTopicName
                );
                throw new BadRequestException(
                    "Invalid MetaTopic",
                    $"MetaTopic '{item.MetaTopicName}' not found in the specified course."
                );
            }

            RefTerm foundQuestionType = ResolveAndValidateQuestionType(item, questionType);
            preparedQuestion.Add((item, foundQuestionType, foundMetaTopic));
        }

        return preparedQuestion;
    }

    /// <summary>
    /// Resolves the question type by matching name or description, and validates the question options and answer.
    /// </summary>
    /// <param name="item">The imported question data.</param>
    /// <param name="questionTypes">The list of available question types.</param>
    /// <returns>The resolved question type.</returns>
    private RefTerm ResolveAndValidateQuestionType(
        QuestionImportDto item,
        List<RefTerm> questionTypes
    )
    {
        RefTerm? foundQuestionType = questionTypes.FirstOrDefault(x =>
            x.Name.Equals(item.QuestionTypeName, StringComparison.OrdinalIgnoreCase)
            || (
                x.Description?.Equals(item.QuestionTypeName, StringComparison.OrdinalIgnoreCase)
                == true
            )
            || (
                item.QuestionTypeName.Contains(
                    CommonConstants.TRUE_SUBSTRING,
                    StringComparison.OrdinalIgnoreCase
                )
                && item.QuestionTypeName.Contains(
                    CommonConstants.FALSE_SUBSTRING,
                    StringComparison.OrdinalIgnoreCase
                )
                && x.Name == CommonConstants.QUESTION_TYPE_TRUE_FALSE
            )
            || (
                item.QuestionTypeName.Contains(
                    CommonConstants.MULTIPLE_SUBSTRING,
                    StringComparison.OrdinalIgnoreCase
                )
                && item.QuestionTypeName.Contains(
                    CommonConstants.CHOICE_SUBSTRING,
                    StringComparison.OrdinalIgnoreCase
                )
                && x.Name == CommonConstants.QUESTION_TYPE_MCQ
            )
            || (
                item.QuestionTypeName.Contains(
                    CommonConstants.FILL_SUBSTRING,
                    StringComparison.OrdinalIgnoreCase
                )
                && item.QuestionTypeName.Contains(
                    CommonConstants.BLANK_SUBSTRING,
                    StringComparison.OrdinalIgnoreCase
                )
                && x.Name == CommonConstants.QUESTION_TYPE_FILL_IN_BLANK
            )
        );

        if (foundQuestionType == null)
        {
            _logger.LogError(
                "Invalid question type '{QuestionType}' found while creating assessment",
                item.QuestionTypeName
            );
            throw new BadRequestException(
                "Invalid QuestionType",
                $"QuestionType '{item.QuestionTypeName}' is not recognized."
            );
        }

        _logger.LogDebug(
            "Resolved question type '{QuestionType}' for question '{QuestionText}'",
            foundQuestionType.Name,
            item.QuestionText
        );

        if (
            foundQuestionType.Name.Equals(
                CommonConstants.QUESTION_TYPE_MCQ,
                StringComparison.OrdinalIgnoreCase
            )
        )
        {
            if (
                string.IsNullOrWhiteSpace(item.Option1)
                || string.IsNullOrWhiteSpace(item.Option2)
                || string.IsNullOrWhiteSpace(item.Option3)
                || string.IsNullOrWhiteSpace(item.Option4)
            )
            {
                _logger.LogError(
                    "MCQ question '{QuestionText}' does not contain 4 option",
                    item.QuestionText
                );
                throw new BadRequestException(
                    "Invalid MCQ Option",
                    $"Question '{item.QuestionText}' requires 4 option."
                );
            }

            _logger.LogDebug(
                "MCQ question '{QuestionText}' has 4 valid options",
                item.QuestionText
            );

            List<string> option = new List<string>
            {
                item.Option1.Trim(),
                item.Option2.Trim(),
                item.Option3.Trim(),
                item.Option4.Trim(),
            };
            if (!option.Any(o => o.Equals(item.Answer.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                _logger.LogError(
                    "Answer '{Answer}' for question '{QuestionText}' does not match any of the provided option",
                    item.Answer,
                    item.QuestionText
                );
                throw new BadRequestException(
                    "Invalid MCQ Answer",
                    $"Correct answer for question '{item.QuestionText}' must match one of the option."
                );
            }

            _logger.LogDebug(
                "MCQ answer '{Answer}' validated for question '{QuestionText}'",
                item.Answer,
                item.QuestionText
            );
        }
        else if (
            foundQuestionType.Name.Equals(
                CommonConstants.QUESTION_TYPE_TRUE_FALSE,
                StringComparison.OrdinalIgnoreCase
            )
        )
        {
            if (string.IsNullOrWhiteSpace(item.Option1) || string.IsNullOrWhiteSpace(item.Option2))
            {
                _logger.LogError(
                    "Question '{QuestionText}' does not contain the required True/False option",
                    item.QuestionText
                );

                throw new BadRequestException(
                    "Invalid True/False Option",
                    $"Question '{item.QuestionText}' requires 2 option."
                );
            }

            _logger.LogDebug(
                "True/False question '{QuestionText}' has both options validated",
                item.QuestionText
            );
        }
        else if (
            foundQuestionType.Name.Equals(
                CommonConstants.QUESTION_TYPE_FILL_IN_BLANK,
                StringComparison.OrdinalIgnoreCase
            )
        )
        {
            if (string.IsNullOrWhiteSpace(item.Answer))
            {
                _logger.LogError(
                    "Question '{QuestionText}' is missing the correct answer for a fill-in-the-blank question",
                    item.QuestionText
                );
                throw new BadRequestException(
                    "Invalid Fill in blank Answer",
                    $"Question '{item.QuestionText}' requires a correct answer."
                );
            }

            _logger.LogDebug(
                "Fill in blank question '{QuestionText}' has a valid answer provided",
                item.QuestionText
            );
        }

        return foundQuestionType;
    }
}
