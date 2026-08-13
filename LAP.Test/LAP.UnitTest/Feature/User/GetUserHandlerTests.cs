using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using LAP.Application.DTO.User;
using LAP.Application.Feature.User.Query;
using LAP.Application.Interface;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using Moq;
using Xunit;
using UserEntity = LAP.Domain.Entity.User;

namespace LAP.UnitTest.Feature.User;

public class GetUserHandlerTests
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<ICustomLogger<GetUserHandler>> _loggerMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetUserHandler _handler;

    public GetUserHandlerTests()
    {
        _userServiceMock = new Mock<IUserService>();
        _loggerMock = new Mock<ICustomLogger<GetUserHandler>>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetUserHandler(
            _userServiceMock.Object,
            _loggerMock.Object,
            _mapperMock.Object
        );
    }

    [Fact]
    public async Task Handle_ReturnsPaginatedUsers()
    {
        // Arrange
        var users = new List<UserEntity> { new UserEntity(), new UserEntity() };
        _userServiceMock
            .Setup(s => s.GetAllUserWithDetailAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

        var mapped = new List<UserDetailDto> { new UserDetailDto(), new UserDetailDto() };
        _mapperMock
            .Setup(m => m.Map<List<UserDetailDto>>(It.IsAny<List<UserEntity>>()))
            .Returns(mapped);

        var query = new GetUserQuery(1, 10);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Total);
        Assert.Equal(1, result.Page);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(mapped, result.Data);
    }
}
