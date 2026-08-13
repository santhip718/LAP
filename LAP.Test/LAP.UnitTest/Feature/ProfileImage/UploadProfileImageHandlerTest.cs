using System.IO;
using System.Text;
using LAP.Application.Constant;
using LAP.Application.DTO.Common;
using LAP.Application.Feature.ProfileImage.Command;
using LAP.Application.Interface;
using LAP.Application.Interface.IContext;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using LAP.Shared.Exceptions;
using Microsoft.AspNetCore.Http;
using Moq;

namespace LAP.UnitTest.Feature.ProfileImage;

using User = LAP.Domain.Entity.User;

public class UploadProfileImageHandlerTest
{
    private readonly Mock<IRequestContext> _requestContextMock;
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<IFileStorageService> _fileStorageServiceMock;
    private readonly Mock<ICustomLogger<UploadProfileImageCommandHandler>> _loggerMock;
    private readonly UploadProfileImageCommandHandler _handler;

    public UploadProfileImageHandlerTest()
    {
        _requestContextMock = new Mock<IRequestContext>();
        _userServiceMock = new Mock<IUserService>();
        _fileStorageServiceMock = new Mock<IFileStorageService>();
        _loggerMock = new Mock<ICustomLogger<UploadProfileImageCommandHandler>>();

        _handler = new UploadProfileImageCommandHandler(
            _requestContextMock.Object,
            _userServiceMock.Object,
            _fileStorageServiceMock.Object,
            _loggerMock.Object
        );
    }

    private static IFormFile CreateMockFile(string fileName, byte[] content)
    {
        var stream = new MemoryStream(content);
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(x => x.FileName).Returns(fileName);
        fileMock.Setup(x => x.Length).Returns(content.Length);
        fileMock.Setup(x => x.OpenReadStream()).Returns(stream);
        fileMock
            .Setup(x => x.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Returns<Stream, CancellationToken>((s, _) => stream.CopyToAsync(s));
        return fileMock.Object;
    }

    [Fact]
    public async Task Handle_ShouldUploadProfileImage_WhenValidRequest()
    {
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, Person = new Person() };
        var fileContent = Encoding.UTF8.GetBytes("fake-image-content");
        var file = CreateMockFile("profile.jpg", fileContent);
        var command = new UploadProfileImageCommand(file);

        _requestContextMock.Setup(x => x.UserId).Returns(userId);
        _userServiceMock
            .Setup(x => x.GetUserByIdWithPersonAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _fileStorageServiceMock
            .Setup(x =>
                x.SaveFileAsync(
                    It.IsAny<byte[]>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(Path.Combine(userId.ToString(), "image.jpg"));

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(userId, result.Id);
        Assert.Equal("Profile image uploaded successfully", result.Message);
        _userServiceMock.Verify(x => x.UpdateUser(user), Times.Never);
        _userServiceMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenNotAuthenticated()
    {
        var file = CreateMockFile("profile.jpg", Encoding.UTF8.GetBytes("test"));
        var command = new UploadProfileImageCommand(file);

        _requestContextMock.Setup(x => x.UserId).Returns((Guid?)null);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _handler.Handle(command, CancellationToken.None)
        );
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenUserNotFound()
    {
        var userId = Guid.NewGuid();
        var file = CreateMockFile("profile.jpg", Encoding.UTF8.GetBytes("test"));
        var command = new UploadProfileImageCommand(file);

        _requestContextMock.Setup(x => x.UserId).Returns(userId);
        _userServiceMock
            .Setup(x => x.GetUserByIdWithPersonAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<NullReferenceException>(() =>
            _handler.Handle(command, CancellationToken.None)
        );
    }

    [Fact]
    public async Task Handle_ShouldDeleteOldImage_WhenProfileImageExists()
    {
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            Person = new Person(),
        };
        var fileContent = Encoding.UTF8.GetBytes("new-image-content");
        var file = CreateMockFile("new-image.jpg", fileContent);
        var command = new UploadProfileImageCommand(file);

        _requestContextMock.Setup(x => x.UserId).Returns(userId);
        _userServiceMock
            .Setup(x => x.GetUserByIdWithPersonAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _fileStorageServiceMock
            .Setup(x =>
                x.SaveFileAsync(
                    It.IsAny<byte[]>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(Path.Combine(userId.ToString(), "image.jpg"));

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(userId, result.Id);
        Assert.Equal("Profile image uploaded successfully", result.Message);
        _fileStorageServiceMock.Verify(
            x => x.DeleteUserProfileImageAsync(userId.ToString()),
            Times.Once
        );
        _userServiceMock.Verify(x => x.UpdateUser(user), Times.Never);
        _userServiceMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenExtensionNotAllowed()
    {
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, Person = new Person() };
        var file = CreateMockFile("profile.gif", Encoding.UTF8.GetBytes("test"));
        var command = new UploadProfileImageCommand(file);

        _requestContextMock.Setup(x => x.UserId).Returns(userId);
        _userServiceMock
            .Setup(x => x.GetUserByIdWithPersonAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var ex = await Assert.ThrowsAsync<BadRequestException>(() =>
            _handler.Handle(command, CancellationToken.None)
        );

        Assert.Equal("Invalid file type", ex.Message);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenFileTooLarge()
    {
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, Person = new Person() };
        var oversizedContent = new byte[CommonConstants.MAX_PROFILE_IMAGE_SIZE + 1024];
        var file = CreateMockFile("profile.jpg", oversizedContent);
        var command = new UploadProfileImageCommand(file);

        _requestContextMock.Setup(x => x.UserId).Returns(userId);
        _userServiceMock
            .Setup(x => x.GetUserByIdWithPersonAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var ex = await Assert.ThrowsAsync<BadRequestException>(() =>
            _handler.Handle(command, CancellationToken.None)
        );

        Assert.Equal("File too large", ex.Message);
    }
}
