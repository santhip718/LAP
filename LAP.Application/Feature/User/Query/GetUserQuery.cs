using AutoMapper;
using LAP.Application.DTO.Paginated;
using LAP.Application.DTO.User;
using LAP.Application.Interface;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using MediatR;

namespace LAP.Application.Feature.User.Query;

/// <summary>Query to retrieve paginated list of all active users with full details.</summary>
/// <param name="Page">The page number (1-based).</param>
/// <param name="PageSize">The number of items per page.</param>
public record GetUserQuery(int Page, int PageSize) : IRequest<PaginatedUsersDto>;

/// <summary>Handles retrieval of all users with pagination and maps them to detail DTOs.</summary>
public class GetUserHandler : IRequestHandler<GetUserQuery, PaginatedUsersDto>
{
    private readonly IUserService _userService;
    private readonly ICustomLogger<GetUserHandler> _logger;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetUserHandler"/> class.
    /// </summary>
    /// <param name="userService">Service used to retrieve user data.</param>
    /// <param name="logger">Custom application logger.</param>
    /// <param name="mapper">AutoMapper instance for mapping user entities to DTOs.</param>
    public GetUserHandler(
        IUserService userService,
        ICustomLogger<GetUserHandler> logger,
        IMapper mapper
    )
    {
        _userService = userService;
        _logger = logger;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the user retrieval request and returns a paginated result with user details.
    /// </summary>
    /// <param name="request">The <see cref="GetUserQuery"/> containing pagination parameters.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A <see cref="PaginatedUsersDto"/> containing the user details for the requested page.</returns>
    public async Task<PaginatedUsersDto> Handle(
        GetUserQuery request,
        CancellationToken cancellationToken
    )
    {
        _logger.LogInfo(
            "Fetching all users for page {Page} and page size {PageSize}.",
            request.Page,
            request.PageSize
        );

        List<Domain.Entity.User> allUserList = await _userService.GetAllUserWithDetailAsync(
            cancellationToken
        );

        int total = allUserList.Count;

        List<Domain.Entity.User> pagedUserList = allUserList
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        List<UserDetailDto> mapped = _mapper.Map<List<UserDetailDto>>(pagedUserList);

        _logger.LogInfo(
            "Returning {Count} user(s) for page {Page} out of {Total} total.",
            mapped.Count,
            request.Page,
            total
        );

        return new PaginatedUsersDto
        {
            Data = mapped,
            Total = total,
            Page = request.Page,
            PageSize = request.PageSize,
        };
    }
}
