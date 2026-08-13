using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using LAP.Application.DTO.Course;
using LAP.Application.Feature.CourseContent.Command;
using LAP.Application.Interface;
using LAP.Application.Interface.IContext;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using Moq;
using Xunit;

namespace LAP.UnitTest.Feature.CourseContent;

public class AddCourseContentHandlerTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ICustomLogger<AddCourseContentHandler>> _loggerMock;
    private readonly Mock<ITransactionService> _transactionServiceMock;
    private readonly Mock<IFileService> _fileServiceMock;
    private readonly Mock<IRequestContext> _requestContextMock;
    private readonly Mock<ICourseContentService> _courseContentServiceMock;
    private readonly AddCourseContentHandler _handler;

    public AddCourseContentHandlerTests()
    {
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ICustomLogger<AddCourseContentHandler>>();
        _transactionServiceMock = new Mock<ITransactionService>();
        _fileServiceMock = new Mock<IFileService>();
        _requestContextMock = new Mock<IRequestContext>();
        _courseContentServiceMock = new Mock<ICourseContentService>();

        _transactionServiceMock.Setup(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task<LAP.Application.DTO.Common.SuccessResponse>>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<Task<LAP.Application.DTO.Common.SuccessResponse>>, CancellationToken>(async (op, ct) => await op());

        _handler = new AddCourseContentHandler(
            _mapperMock.Object,
            _loggerMock.Object,
            _transactionServiceMock.Object,
            _fileServiceMock.Object,
            _requestContextMock.Object,
            _courseContentServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingMetaTopic_AddsContent()
    {
        // Arrange
        var dto = new CreateCourseContentRequestDto { CourseId = Guid.NewGuid(), MetaTopic = "Topic", Title = "Content" };
        var metaTopic = new CourseMetaTopic { Id = Guid.NewGuid(), Name = "Topic" };
        var courseContent = new LAP.Domain.Entity.CourseContent { Id = Guid.NewGuid() };

        _courseContentServiceMock.Setup(s => s.GetMetaTopicByCourseAndNameAsync(dto.CourseId, "Topic", It.IsAny<CancellationToken>()))
            .ReturnsAsync(metaTopic);

        _mapperMock.Setup(m => m.Map<LAP.Domain.Entity.CourseContent>(dto)).Returns(courseContent);

        // Act
        var result = await _handler.Handle(new AddCourseContentCommand(dto), CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, result.Id);
        _courseContentServiceMock.Verify(s => s.AddAsync(courseContent, It.IsAny<CancellationToken>()), Times.Once);
        _transactionServiceMock.Verify(s => s.ExecuteInTransactionAsync(It.IsAny<Func<Task<LAP.Application.DTO.Common.SuccessResponse>>>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
