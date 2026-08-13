using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using LAP.Application.DTO.User;
using LAP.Application.Feature.User.Command;
using LAP.Application.Interface;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using LAP.Shared.Exceptions;
using Moq;
using Xunit;
using UserEntity = LAP.Domain.Entity.User;

namespace LAP.UnitTest.Feature.User;

public class UpdateUserHandlerTests
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<IFileStorageService> _fileStorageServiceMock;
    private readonly Mock<ITransactionService> _transactionServiceMock;
    private readonly Mock<ICustomLogger<UpdateUserHandler>> _loggerMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly UpdateUserHandler _handler;

    public UpdateUserHandlerTests()
    {
        _userServiceMock = new Mock<IUserService>();
        _fileStorageServiceMock = new Mock<IFileStorageService>();
        _transactionServiceMock = new Mock<ITransactionService>();
        _loggerMock = new Mock<ICustomLogger<UpdateUserHandler>>();
        _mapperMock = new Mock<IMapper>();

        _transactionServiceMock
            .Setup(x =>
                x.ExecuteInTransactionAsync(
                    It.IsAny<Func<Task<UserDetailDto>>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns<Func<Task<UserDetailDto>>, CancellationToken>(async (op, ct) => await op());

        _handler = new UpdateUserHandler(
            _userServiceMock.Object,
            _fileStorageServiceMock.Object,
            _transactionServiceMock.Object,
            _loggerMock.Object,
            _mapperMock.Object
        );
    }

    [Fact]
    public async Task Handle_ExistingUser_UpdatesAndReturnsDetails()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var dto = new UpdateUserRequestDto { FullName = "New Name" };
        var user = new UserEntity
        {
            Id = userId,
            Person = new Person { FullName = "Old Name" },
        };
        _userServiceMock
            .Setup(s => s.GetUserByIdWithDetailAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var expectedDto = new UserDetailDto();
        _mapperMock.Setup(m => m.Map<UserDetailDto>(user)).Returns(expectedDto);

        // Act
        var result = await _handler.Handle(
            new UpdateUserCommand(userId, dto),
            CancellationToken.None
        );

        // Assert
        Assert.Equal(expectedDto, result);
        Assert.Equal(dto.FullName, user.Person.FullName);
        _userServiceMock.Verify(s => s.UpdateUser(user), Times.Once);
        _transactionServiceMock.Verify(
            s => s.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_NonExistingUser_ThrowsNotFoundException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var dto = new UpdateUserRequestDto { FullName = "Name" };
        _userServiceMock
            .Setup(s => s.GetUserByIdWithDetailAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserEntity?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _handler.Handle(new UpdateUserCommand(userId, dto), CancellationToken.None)
        );
    }
}
