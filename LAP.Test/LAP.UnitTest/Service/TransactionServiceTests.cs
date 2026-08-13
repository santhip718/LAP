using LAP.Application.Interface;
using LAP.Application.Interface.IRepository;
using LAP.Application.Service;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace LAP.UnitTest.Service;

public class TransactionServiceTests
{
    private readonly Mock<IRepositoryWrapper> _repoMock;
    private readonly Mock<ICustomLogger<TransactionService>> _loggerMock;
    private readonly TransactionService _service;

    public TransactionServiceTests()
    {
        _repoMock = new Mock<IRepositoryWrapper>();
        _loggerMock = new Mock<ICustomLogger<TransactionService>>();

        _service = new TransactionService(_repoMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldCallRepo()
    {
        _repoMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _service.SaveChangesAsync();

        Assert.Equal(1, result);
        _repoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteInTransactionAsync_WithResult_ShouldCallRepo()
    {
        var expectedResult = "success";
        _repoMock.Setup(r => r.ExecuteInTransactionAsync(It.IsAny<Func<Task<string>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var result = await _service.ExecuteInTransactionAsync(() => Task.FromResult(expectedResult));

        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public async Task ExecuteInTransactionAsync_ShouldCallRepo()
    {
        await _service.ExecuteInTransactionAsync(() => Task.CompletedTask);

        _repoMock.Verify(r => r.ExecuteInTransactionAsync(It.IsAny<Func<Task>>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
