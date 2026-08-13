using AutoMapper;
using FluentValidation;
using LAP.Application.DTO.Assessment;
using LAP.Application.Interface;
using LAP.Application.Interface.IService;
using LAP.Shared.Exceptions;
using MediatR;

namespace LAP.Application.Feature.Assessment.Query;

/// <summary>
/// Query to retrieve paginated assessment attempt history for a specific user.
/// </summary>
/// <param name="UserId">The identifier of the user.</param>
/// <param name="PageNumber">The page number (1-based).</param>
/// <param name="PageSize">The number of records per page.</param>
public record GetUserAssessmentHistoryQuery(Guid UserId, int PageNumber, int PageSize)
    : IRequest<PaginatedAssessmentHistoryResponseDto>;

/// <summary>
/// Validates the <see cref="GetUserAssessmentHistoryQuery"/> request data.
/// </summary>
public class GetUserAssessmentHistoryValidator : AbstractValidator<GetUserAssessmentHistoryQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetUserAssessmentHistoryValidator"/> class.
    /// </summary>
    public GetUserAssessmentHistoryValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithMessage("User identifier is required");

        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than zero");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("Page size must be between 1 and 100");
    }
}

/// <summary>
/// Handles the retrieval of paginated assessment history for a user.
/// </summary>
public class GetUserAssessmentHistoryHandler
    : IRequestHandler<GetUserAssessmentHistoryQuery, PaginatedAssessmentHistoryResponseDto>
{
    private readonly IAssessmentService _assessmentService;
    private readonly ICustomLogger<GetUserAssessmentHistoryHandler> _logger;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetUserAssessmentHistoryHandler"/> class.
    /// </summary>
    /// <param name="assessmentService">The assessment service.</param>
    /// <param name="logger">The custom logger.</param>
    /// <param name="mapper">The AutoMapper instance.</param>
    public GetUserAssessmentHistoryHandler(
        IAssessmentService assessmentService,
        ICustomLogger<GetUserAssessmentHistoryHandler> logger,
        IMapper mapper
    )
    {
        _assessmentService = assessmentService;
        _logger = logger;
        _mapper = mapper;
    }

    /// <summary>
    /// Retrieves the complete assessment attempt history for the specified user.
    /// </summary>
    /// <param name="request">The get user assessment history query.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A paginated list of assessment history records.</returns>
    public async Task<PaginatedAssessmentHistoryResponseDto> Handle(
        GetUserAssessmentHistoryQuery request,
        CancellationToken cancellationToken
    )
    {
        _logger.LogInfo(
            "Retrieving assessment history for user {UserId} (page {PageNumber}, size {PageSize}).",
            request.UserId,
            request.PageNumber,
            request.PageSize
        );

        Domain.Entity.User? user = await _assessmentService.GetUserByIdAsync(
            request.UserId,
            cancellationToken
        );

        if (user is null)
        {
            _logger.LogError(
                "User {UserId} not found for assessment history retrieval.",
                request.UserId
            );
            throw new NotFoundException(
                "User not found",
                $"User with ID {request.UserId} does not exist."
            );
        }

        (IEnumerable<Domain.Entity.AssessmentHistory> itemList, int totalCount) =
            await _assessmentService.GetPagedAssessmentHistoryAsync(
                request.UserId,
                request.PageNumber,
                request.PageSize,
                cancellationToken
            );

        List<AssessmentHistoryItemDto> historyItemList = _mapper.Map<
            List<AssessmentHistoryItemDto>
        >(itemList);

        PaginatedAssessmentHistoryResponseDto result = new PaginatedAssessmentHistoryResponseDto
        {
            Item = historyItemList,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalRecords = totalCount,
        };

        _logger.LogInfo(
            "Successfully retrieved {ItemCount} assessment history records for user {UserId} (total: {TotalCount}).",
            historyItemList.Count,
            request.UserId,
            totalCount
        );

        return result;
    }
}
