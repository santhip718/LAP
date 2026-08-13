using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using LAP.Application.DTO.User;
using LAP.Application.Feature.User.Query;
using LAP.Application.Interface;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using LAP.Shared.Exceptions;
using Moq;
using Xunit;
using UserEntity = LAP.Domain.Entity.User;

namespace LAP.UnitTest.Feature.User;

public class GetUserProfileHandlerTests
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<IFileStorageService> _fileStorageServiceMock;
    private readonly Mock<ICustomLogger<GetUserProfileHandler>> _loggerMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetUserProfileHandler _handler;

    public GetUserProfileHandlerTests()
    {
        _userServiceMock = new Mock<IUserService>();
        _fileStorageServiceMock = new Mock<IFileStorageService>();
        _loggerMock = new Mock<ICustomLogger<GetUserProfileHandler>>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetUserProfileHandler(
            _userServiceMock.Object,
            _fileStorageServiceMock.Object,
            _loggerMock.Object,
            _mapperMock.Object
        );
    }

    [Fact]
    public async Task Handle_ExistingUser_ReturnsProfileDto()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new UserEntity { Id = userId };
        _userServiceMock
            .Setup(s => s.GetUserByIdWithEnrollmentsAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var expectedDto = new UserProfileDto();
        _mapperMock.Setup(m => m.Map<UserProfileDto>(user)).Returns(expectedDto);

        _fileStorageServiceMock
            .Setup(f => f.GetUserProfileImageAsync(userId.ToString()))
            .ReturnsAsync("profile-image-url");

        // Act
        var result = await _handler.Handle(new GetUserProfileQuery(userId), CancellationToken.None);

        // Assert
        Assert.Equal(expectedDto, result);
        Assert.Equal("profile-image-url", result.ProfileImage);
    }

    [Fact]
    public async Task Handle_NonExistingUser_ThrowsNotFoundException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userServiceMock
            .Setup(s => s.GetUserByIdWithEnrollmentsAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserEntity?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _handler.Handle(new GetUserProfileQuery(userId), CancellationToken.None)
        );
    }
}
