using AutoMapper;
using LAP.Application.DTO.Common;
using LAP.Application.DTO.Course;
using LAP.Application.Feature.Course.Command;
using LAP.Application.Interface;
using LAP.Application.Interface.IContext;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;
using CourseEntity = LAP.Domain.Entity.Course;

namespace LAP.UnitTest.Feature.Course;

public class CreateCourseHandlerTests
{
    private readonly Mock<ICourseService> _courseServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ICustomLogger<CreateCourseHandler>> _loggerMock;
    private readonly Mock<ITransactionService> _transactionServiceMock;
    private readonly Mock<IRequestContext> _requestContextMock;
    private readonly Mock<IFileService> _fileServiceMock;
    private readonly CreateCourseHandler _handler;

    public CreateCourseHandlerTests()
    {
        _courseServiceMock = new Mock<ICourseService>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ICustomLogger<CreateCourseHandler>>();
        _transactionServiceMock = new Mock<ITransactionService>();
        _requestContextMock = new Mock<IRequestContext>();
        _fileServiceMock = new Mock<IFileService>();

        _handler = new CreateCourseHandler(
            _courseServiceMock.Object,
            _mapperMock.Object,
            _loggerMock.Object,
            _transactionServiceMock.Object,
            _requestContextMock.Object,
            _fileServiceMock.Object
        );
    }

    [Fact]
    public async Task Handle_ValidRequest_CreatesCourseAndReturnsSuccess()
    {
        // Arrange
        var dto = new CreateCourseRequestDto
        {
            Title = "New Course",
            Description = "Description",
            CategoryId = Guid.NewGuid(),
            DifficultyLevelId = Guid.NewGuid(),
            DurationMinute = 60
        };
        var command = new CreateCourseCommand(dto);
        var userId = Guid.NewGuid();
        var course = new CourseEntity { Title = dto.Title };

        _requestContextMock.Setup(c => c.UserId).Returns(userId);
        _mapperMock.Setup(m => m.Map<CourseEntity>(dto)).Returns(course);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal("Course created successfully", result.Message);
        Assert.NotEqual(Guid.Empty, result.Id);
        _courseServiceMock.Verify(s => s.AddCourseAsync(It.IsAny<CourseEntity>(), It.IsAny<CancellationToken>()), Times.Once);
        _transactionServiceMock.Verify(s => s.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithThumbnail_UploadsThumbnail()
    {
        // Arrange
        var thumbnailMock = new Mock<IFormFile>();
        var content = "fake image content";
        var fileName = "test.png";
        var ms = new MemoryStream();
        var writer = new StreamWriter(ms);
        writer.Write(content);
        writer.Flush();
        ms.Position = 0;

        thumbnailMock.Setup(_ => _.OpenReadStream()).Returns(ms);
        thumbnailMock.Setup(_ => _.FileName).Returns(fileName);
        thumbnailMock.Setup(_ => _.Length).Returns(ms.Length);
        thumbnailMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Callback<Stream, CancellationToken>((s, c) => ms.CopyTo(s))
            .Returns(Task.CompletedTask);

        var dto = new CreateCourseRequestDto
        {
            Title = "New Course",
            ThumbnailImg = thumbnailMock.Object
        };
        var command = new CreateCourseCommand(dto);
        var course = new CourseEntity { Title = dto.Title };

        _mapperMock.Setup(m => m.Map<CourseEntity>(dto)).Returns(course);
        _fileServiceMock.Setup(f => f.SaveFileAsync(It.IsAny<IFormFile>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("saved/path/test.png");

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _fileServiceMock.Verify(f => f.SaveFileAsync(It.IsAny<IFormFile>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal("saved/path/test.png", course.ThumbnailImgPath);
    }
}
