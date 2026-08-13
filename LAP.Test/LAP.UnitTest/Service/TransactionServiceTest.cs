using LAP.Application.Interface;
using LAP.Application.Interface.IRepository;
using LAP.Application.Service;
using Moq;

namespace LAP.UnitTest.Service;

public class TransactionServiceTest
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<ICustomLogger<TransactionService>> _loggerMock;
    private readonly TransactionService _transactionService;

    public TransactionServiceTest()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _loggerMock = new Mock<ICustomLogger<TransactionService>>();
        _transactionService = new TransactionService(
            _repositoryWrapperMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldReturnCount()
    {
        _repositoryWrapperMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(5);

        var result = await _transactionService.SaveChangesAsync();

        Assert.Equal(5, result);
    }

    [Fact]
    public async Task ExecuteInTransactionAsync_WithResult_ShouldExecuteOperation()
    {
        var expected = "result";
        _repositoryWrapperMock
            .Setup(x =>
                x.ExecuteInTransactionAsync(
                    It.IsAny<Func<Task<string>>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns((Func<Task<string>> op, CancellationToken _) => op());

        var result = await _transactionService.ExecuteInTransactionAsync(() =>
            Task.FromResult(expected)
        );

        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task ExecuteInTransactionAsync_WithoutResult_ShouldExecuteOperation()
    {
        var executed = false;
        _repositoryWrapperMock
            .Setup(x =>
                x.ExecuteInTransactionAsync(It.IsAny<Func<Task>>(), It.IsAny<CancellationToken>())
            )
            .Returns((Func<Task> op, CancellationToken _) => op());

        await _transactionService.ExecuteInTransactionAsync(() =>
        {
            executed = true;
            return Task.CompletedTask;
        });

        Assert.True(executed);
    }
}
