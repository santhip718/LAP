using FluentValidation;
using LAP.Application.DTO.Common;
using LAP.Application.Helper;
using LAP.Application.Interface;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using MediatR;

namespace LAP.Application.Feature.ReferenceData.Query;

/// <summary>
/// Query used to retrieve reference terms for a given reference set name.
/// </summary>
/// <param name="RefSetName">The name of the reference set.</param>
public record GetReferenceDataQuery(string RefSetName) : IRequest<List<RefTermDto>>;

/// <summary>
/// Validates the <see cref="GetReferenceDataQuery"/> request.
/// </summary>
public class GetReferenceDataQueryValidator : AbstractValidator<GetReferenceDataQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetReferenceDataQueryValidator"/> class.
    /// </summary>
    public GetReferenceDataQueryValidator()
    {
        RuleFor(x => x.RefSetName).NotEmpty().WithMessage("Reference set name is required");
    }
}

/// <summary>
/// Handles retrieval of reference terms for a specified reference set.
/// </summary>
public class GetReferenceDataQueryHandler : IRequestHandler<GetReferenceDataQuery, List<RefTermDto>>
{
    private readonly IReferenceCacheService _referenceCacheService;
    private readonly ICustomLogger<GetReferenceDataQueryHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetReferenceDataQueryHandler"/> class.
    /// </summary>
    /// <param name="referenceCacheService">
    /// Service used to retrieve cached reference sets and terms.
    /// </param>
    /// <param name="logger">Application logger.</param>
    public GetReferenceDataQueryHandler(
        IReferenceCacheService referenceCacheService,
        ICustomLogger<GetReferenceDataQueryHandler> logger
    )
    {
        _referenceCacheService = referenceCacheService;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves reference terms associated with the specified reference set name.
    /// </summary>
    /// <param name="request">The reference data query request.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A list of matching reference terms.</returns>
    public async Task<List<RefTermDto>> Handle(
        GetReferenceDataQuery request,
        CancellationToken cancellationToken
    )
    {
        _logger.LogInfo("Processing reference data request for {RefSetName}", request.RefSetName);

        IReadOnlyList<RefSet> refSet = await _referenceCacheService.GetRefSetAsync(
            cancellationToken
        );

        IReadOnlyList<RefTerm> refTerm = await _referenceCacheService.GetRefTermAsync(
            cancellationToken
        );

        string normalizedInput = ReferenceDataNormalizer.Normalize(request.RefSetName);

        RefSet? matchedRefSet = refSet.FirstOrDefault(x =>
            ReferenceDataNormalizer.Normalize(x.Name) == normalizedInput
        );

        if (matchedRefSet is null)
        {
            _logger.LogInfo(
                "Reference data request completed. No reference set found for {RefSetName}",
                request.RefSetName
            );

            return new List<RefTermDto>();
        }

        List<RefTermDto> result = refTerm
            .Where(x => x.RefSetId == matchedRefSet.Id)
            .Select(x => new RefTermDto { Id = x.Id, Name = x.Name })
            .ToList();
        _logger.LogInfo(
            "Reference data request completed for {RefSetName}. Returned {Count} terms",
            request.RefSetName,
            result.Count
        );
        return result;
    }
}
