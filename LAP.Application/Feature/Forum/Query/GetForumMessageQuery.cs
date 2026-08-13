using AutoMapper;
using FluentValidation;
using LAP.Application.DTO.Forum;
using LAP.Application.Interface;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using LAP.Shared.Exceptions;
using MediatR;

namespace LAP.Application.Feature.Forum.Query;

/// <summary>
/// Query for retrieving forum messages for a specific course.
/// </summary>
/// <param name="CourseId">The identifier of the course.</param>
public record GetForumMessageQuery(Guid CourseId) : IRequest<List<ForumMessageDto>>;

/// <summary>
/// Validates the <see cref="GetForumMessageQuery"/> before it is handled.
/// </summary>
public class GetForumMessageQueryValidator : AbstractValidator<GetForumMessageQuery>
{
    /// <summary>
    /// Initializes validation rules for the <see cref="GetForumMessageQuery"/>.
    /// </summary>
    public GetForumMessageQueryValidator()
    {
        RuleFor(x => x.CourseId).NotEmpty().WithMessage("Course ID is required");
    }
}

/// <summary>
/// Handles the <see cref="GetForumMessageQuery"/> by retrieving and mapping forum messages for the specified course.
/// </summary>
public class GetForumMessageHandler : IRequestHandler<GetForumMessageQuery, List<ForumMessageDto>>
{
    private readonly IForumService _forumService;
    private readonly ICustomLogger<GetForumMessageHandler> _logger;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetForumMessageHandler"/> class.
    /// </summary>
    /// <param name="forumService">Service used to retrieve forum data.</param>
    /// <param name="logger">Custom application logger.</param>
    /// <param name="mapper">AutoMapper instance for mapping forum message entities to DTOs.</param>
    public GetForumMessageHandler(
        IForumService forumService,
        ICustomLogger<GetForumMessageHandler> logger,
        IMapper mapper
    )
    {
        _forumService = forumService;
        _logger = logger;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the forum message retrieval request and returns a list of message DTOs for the given course.
    /// </summary>
    /// <param name="request">The <see cref="GetForumMessageQuery"/> containing the course identifier.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>
    /// A list of <see cref="ForumMessageDto"/> instances for the specified course.
    /// </returns>
    public async Task<List<ForumMessageDto>> Handle(
        GetForumMessageQuery request,
        CancellationToken cancellationToken
    )
    {
        _logger.LogInfo("Fetching forum messages for course {CourseId}.", request.CourseId);

        bool courseExist = await _forumService.CourseExistsAsync(
            request.CourseId,
            cancellationToken
        );

        if (!courseExist)
        {
            _logger.LogError(
                "Forum message retrieval failed because course {CourseId} was not found.",
                request.CourseId
            );

            throw new NotFoundException(
                "Course not found",
                $"No active course exists with id {request.CourseId}."
            );
        }

        List<ForumMessage> message = await _forumService.GetMessageByCourseIdAsync(
            request.CourseId,
            cancellationToken
        );

        List<ForumMessageDto> result = _mapper.Map<List<ForumMessageDto>>(message);

        _logger.LogInfo(
            "Returning {Count} forum message(s) for course {CourseId}.",
            result.Count,
            request.CourseId
        );

        return result;
    }
}
