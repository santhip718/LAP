using LAP.Application.Interface;
using LAP.Application.Interface.IRepository;
using LAP.Application.Service;
using LAP.Domain.Entity;
using LAP.UnitTest.Helpers;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace LAP.UnitTest.Service;

public class ForumServiceTests
{
    private readonly Mock<IRepositoryWrapper> _repoMock;
    private readonly Mock<IForumRepository> _forumRepoMock;
    private readonly Mock<IBaseRepository<ForumMessage>> _forumBaseRepoMock;
    private readonly Mock<ICustomLogger<ForumService>> _loggerMock;
    private readonly ForumService _service;

    public ForumServiceTests()
    {
        _repoMock = new Mock<IRepositoryWrapper>();
        _forumRepoMock = new Mock<IForumRepository>();
        _forumBaseRepoMock = new Mock<IBaseRepository<ForumMessage>>();
        _loggerMock = new Mock<ICustomLogger<ForumService>>();

        _repoMock.Setup(r => r.Forum).Returns(_forumRepoMock.Object);
        _repoMock.Setup(r => r.Repository<ForumMessage>()).Returns(_forumBaseRepoMock.Object);

        _service = new ForumService(_repoMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetMessageByCourseIdAsync_ShouldReturnList()
    {
        var courseId = Guid.NewGuid();
        var messages = new List<ForumMessage> { new ForumMessage() };
        _forumRepoMock.Setup(r => r.GetByConditionNoTracking(It.IsAny<Expression<Func<ForumMessage, bool>>>()))
            .Returns(messages.AsAsyncQueryable());

        var result = await _service.GetMessageByCourseIdAsync(courseId);

        Assert.Equal(messages, result);
    }

    [Fact]
    public async Task AddMessageAsync_ShouldReturnAddedMessage()
    {
        var message = new ForumMessage { Id = Guid.NewGuid(), CourseId = Guid.NewGuid() };
        _forumRepoMock.Setup(r => r.AddAsync(message, It.IsAny<CancellationToken>()))
            .ReturnsAsync(message);

        var result = await _service.AddMessageAsync(message);

        Assert.Equal(message, result);
    }

}
