using AutoMapper;
using LAP.Application.DTO.CourseContent;
using LAP.Application.Feature.CourseContent.Query;
using LAP.Application.Interface;
using LAP.Application.Interface.IContext;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using LAP.Shared.Exceptions;
using Moq;

namespace LAP.UnitTest.Features.CourseContentHandlers;

public class GetCourseContentByIdHandlerTest
{
    private readonly Mock<ICourseContentService> _courseContentServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRequestContext> _requestContextMock;
    private readonly Mock<ICustomLogger<GetCourseContentByIdHandler>> _loggerMock;
    private readonly Mock<IFileStorageService> _fileStorageServiceMock;
    private readonly GetCourseContentByIdHandler _handler;

    public GetCourseContentByIdHandlerTest()
    {
        _courseContentServiceMock = new Mock<ICourseContentService>();
        _mapperMock = new Mock<IMapper>();
        _requestContextMock = new Mock<IRequestContext>();
        _loggerMock = new Mock<ICustomLogger<GetCourseContentByIdHandler>>();
        _fileStorageServiceMock = new Mock<IFileStorageService>();
        _handler = new GetCourseContentByIdHandler(
            _courseContentServiceMock.Object,
            _mapperMock.Object,
            _requestContextMock.Object,
            _loggerMock.Object,
            _fileStorageServiceMock.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldReturnContentDetail()
    {
        var contentId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var query = new GetCourseContentByIdQuery(contentId);
        var content = new CourseContent
        {
            Id = contentId,
            Title = "Test Content",
            MetaTopic = new CourseMetaTopic { CourseId = courseId },
        };
        var dto = new CourseContentDetailDto { Id = contentId, Title = "Test Content" };

        _courseContentServiceMock
            .Setup(x => x.GetContentWithMetaTopicAsync(contentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(content);
        _mapperMock.Setup(x => x.Map<CourseContentDetailDto>(content)).Returns(dto);
        _requestContextMock.Setup(x => x.UserId).Returns(Guid.NewGuid());

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.Equal(contentId, result.Id);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenContentNotFound()
    {
        var query = new GetCourseContentByIdQuery(Guid.NewGuid());

        _courseContentServiceMock
            .Setup(x =>
                x.GetContentWithMetaTopicAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync((CourseContent?)null);

        var ex = await Assert.ThrowsAsync<NotFoundException>(() =>
            _handler.Handle(query, CancellationToken.None)
        );

        Assert.Equal("Course content not found", ex.Message);
    }

    [Fact]
    public async Task Handle_ShouldIncludeProgress_WhenEnrolled()
    {
        var contentId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var enrollmentId = Guid.NewGuid();
        var query = new GetCourseContentByIdQuery(contentId);
        var content = new CourseContent
        {
            Id = contentId,
            Title = "Test Content",
            MetaTopic = new CourseMetaTopic { CourseId = courseId },
        };
        var dto = new CourseContentDetailDto { Id = contentId };

        _courseContentServiceMock
            .Setup(x => x.GetContentWithMetaTopicAsync(contentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(content);
        _mapperMock.Setup(x => x.Map<CourseContentDetailDto>(content)).Returns(dto);
        _requestContextMock.Setup(x => x.UserId).Returns(userId);
        _courseContentServiceMock
            .Setup(x =>
                x.GetEnrollmentByUserAndCourseAsync(userId, courseId, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(new Enrollment { Id = enrollmentId });
        _courseContentServiceMock
            .Setup(x => x.GetProgressAsync(enrollmentId, contentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new UserCourseProgress { IsCompleted = true, CompletedOn = DateTime.UtcNow }
            );

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsCompleted);
    }

    [Fact]
    public async Task Handle_ShouldIncludePreviousAndNextContent()
    {
        var contentId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var query = new GetCourseContentByIdQuery(contentId);
        var content = new CourseContent
        {
            Id = contentId,
            MetaTopic = new CourseMetaTopic { CourseId = courseId, SequenceOrder = 1 },
            SequenceOrder = 2,
        };
        var dto = new CourseContentDetailDto { Id = contentId };
        var prevContent = new CourseContent { Id = Guid.NewGuid() };
        var nextContent = new CourseContent { Id = Guid.NewGuid() };

        _courseContentServiceMock
            .Setup(x => x.GetContentWithMetaTopicAsync(contentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(content);
        _mapperMock.Setup(x => x.Map<CourseContentDetailDto>(content)).Returns(dto);
        _requestContextMock.Setup(x => x.UserId).Returns(Guid.NewGuid());
        _courseContentServiceMock
            .Setup(x => x.GetPreviousContentAsync(courseId, 1, 2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(prevContent);
        _courseContentServiceMock
            .Setup(x => x.GetNextContentAsync(courseId, 1, 2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(nextContent);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.Equal(prevContent.Id, result.PreviousContentId);
        Assert.Equal(nextContent.Id, result.NextContentId);
    }
}
