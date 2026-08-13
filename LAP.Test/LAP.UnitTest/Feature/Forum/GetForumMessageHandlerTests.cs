using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using LAP.Application.DTO.Forum;
using LAP.Application.Feature.Forum.Query;
using LAP.Application.Interface;
using LAP.Application.Interface.IRepository;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using LAP.Shared.Exceptions;
using Moq;
using Xunit;

namespace LAP.UnitTest.Feature.Forum;

public class GetForumMessageHandlerTests
{
    private readonly Mock<IForumService> _forumServiceMock;
    private readonly Mock<ICustomLogger<GetForumMessageHandler>> _loggerMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetForumMessageHandler _handler;

    public GetForumMessageHandlerTests()
    {
        _forumServiceMock = new Mock<IForumService>();
        _loggerMock = new Mock<ICustomLogger<GetForumMessageHandler>>();
        _mapperMock = new Mock<IMapper>();

        _handler = new GetForumMessageHandler(
            _forumServiceMock.Object,
            _loggerMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingCourse_ReturnsMessages()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var messages = new List<ForumMessage> { new ForumMessage() };
        _forumServiceMock.Setup(s => s.CourseExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _forumServiceMock.Setup(s => s.GetMessageByCourseIdAsync(courseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(messages);

        var mapped = new List<ForumMessageDto> { new ForumMessageDto() };
        _mapperMock.Setup(m => m.Map<List<ForumMessageDto>>(messages)).Returns(mapped);

        // Act
        var result = await _handler.Handle(new GetForumMessageQuery(courseId), CancellationToken.None);

        // Assert
        Assert.Equal(mapped, result);
    }

    [Fact]
    public async Task Handle_CourseNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        _forumServiceMock.Setup(s => s.CourseExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(new GetForumMessageQuery(courseId), CancellationToken.None));
    }
}
