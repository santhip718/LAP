using LAP.Application.Interface;
using LAP.Application.Interface.IRepository;
using LAP.Application.Interface.IService;
using LAP.Application.Service;
using Moq;

namespace LAP.UnitTest.Services;

public class TransactionServiceTest
{
    private readonly Mock<IRepositoryWrapper> _repoWrapperMock;
    private readonly ITransactionService _transactionService;

    public TransactionServiceTest()
    {
        _repoWrapperMock = new Mock<IRepositoryWrapper>();
        var loggerMock = new Mock<ICustomLogger<TransactionService>>();
        _transactionService = new TransactionService(_repoWrapperMock.Object, loggerMock.Object);
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldReturnResultFromWrapper()
    {
        _repoWrapperMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(5);

        var result = await _transactionService.SaveChangesAsync();

        Assert.Equal(5, result);
    }

    [Fact]
    public async Task ExecuteInTransactionAsync_WithResult_ShouldReturnOperationResult()
    {
        var expected = "transaction-result";
        _repoWrapperMock
            .Setup(x =>
                x.ExecuteInTransactionAsync(
                    It.IsAny<Func<Task<string>>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(expected);

        var result = await _transactionService.ExecuteInTransactionAsync(() =>
            Task.FromResult("transaction-result")
        );

        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task ExecuteInTransactionAsync_WithResult_ShouldPassCancellationToken()
    {
        var cts = new CancellationTokenSource();
        _repoWrapperMock
            .Setup(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task<string>>>(), cts.Token))
            .ReturnsAsync("done");

        var result = await _transactionService.ExecuteInTransactionAsync(
            () => Task.FromResult("done"),
            cts.Token
        );

        Assert.Equal("done", result);
        _repoWrapperMock.Verify(
            x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task<string>>>(), cts.Token),
            Times.Once
        );
    }

    [Fact]
    public async Task ExecuteInTransactionAsync_WithoutResult_ShouldCallWrapper()
    {
        bool executed = false;

        _repoWrapperMock
            .Setup(x =>
                x.ExecuteInTransactionAsync(It.IsAny<Func<Task>>(), It.IsAny<CancellationToken>())
            )
            .Callback<Func<Task>, CancellationToken>(
                (func, ct) =>
                {
                    executed = true;
                }
            )
            .Returns(Task.CompletedTask);

        await _transactionService.ExecuteInTransactionAsync(() =>
        {
            executed = true;
            return Task.CompletedTask;
        });

        _repoWrapperMock.Verify(
            x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task>>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }
}
