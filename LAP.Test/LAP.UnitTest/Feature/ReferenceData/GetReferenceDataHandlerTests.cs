using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LAP.Application.Feature.ReferenceData.Query;
using LAP.Application.Interface;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using Moq;
using Xunit;

namespace LAP.UnitTest.Feature.ReferenceData;

public class GetReferenceDataHandlerTests
{
    private readonly Mock<IReferenceCacheService> _referenceCacheServiceMock;
    private readonly Mock<ICustomLogger<GetReferenceDataQueryHandler>> _loggerMock;
    private readonly GetReferenceDataQueryHandler _handler;

    public GetReferenceDataHandlerTests()
    {
        _referenceCacheServiceMock = new Mock<IReferenceCacheService>();
        _loggerMock = new Mock<ICustomLogger<GetReferenceDataQueryHandler>>();
        _handler = new GetReferenceDataQueryHandler(
            _referenceCacheServiceMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task Handle_ExistingRefSet_ReturnsTerms()
    {
        // Arrange
        var refSetName = "Category";
        var refSetId = Guid.NewGuid();
        var refSets = new List<RefSet>
        {
            new RefSet { Id = refSetId, Name = refSetName },
        };
        var refTerms = new List<RefTerm>
        {
            new RefTerm
            {
                Id = Guid.NewGuid(),
                Name = "Term 1",
                RefSetId = refSetId,
            },
            new RefTerm
            {
                Id = Guid.NewGuid(),
                Name = "Term 2",
                RefSetId = Guid.NewGuid(),
            },
        };

        _referenceCacheServiceMock
            .Setup(s => s.GetRefSetAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(refSets);
        _referenceCacheServiceMock
            .Setup(s => s.GetRefTermAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(refTerms);

        // Act
        var result = await _handler.Handle(
            new GetReferenceDataQuery(refSetName),
            CancellationToken.None
        );

        // Assert
        Assert.Single(result);
        Assert.Equal("Term 1", result[0].Name);
    }

    [Fact]
    public async Task Handle_NonExistingRefSet_ReturnsEmptyList()
    {
        // Arrange
        _referenceCacheServiceMock
            .Setup(s => s.GetRefSetAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<RefSet>());
        _referenceCacheServiceMock
            .Setup(s => s.GetRefTermAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<RefTerm>());

        // Act
        var result = await _handler.Handle(
            new GetReferenceDataQuery("Unknown"),
            CancellationToken.None
        );

        // Assert
        Assert.Empty(result);
    }
}
