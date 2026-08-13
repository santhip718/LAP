using LAP.Application.DTO.CourseContent;
using LAP.Application.Feature.CourseContent.Command;
using LAP.Application.Interface;
using LAP.Application.Interface.IContext;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using LAP.Shared.Exceptions;
using Moq;

namespace LAP.UnitTest.Features.CourseContentHandlers;

public class UpdateContentCompletionStatusHandlerTest
{
    private readonly Mock<ICourseContentService> _courseContentServiceMock;
    private readonly Mock<ITransactionService> _transactionServiceMock;
    private readonly Mock<IRequestContext> _requestContextMock;
    private readonly Mock<ICustomLogger<UpdateContentCompletionStatusHandler>> _loggerMock;
    private readonly UpdateContentCompletionStatusHandler _handler;

    public UpdateContentCompletionStatusHandlerTest()
    {
        _courseContentServiceMock = new Mock<ICourseContentService>();
        _transactionServiceMock = new Mock<ITransactionService>();
        _requestContextMock = new Mock<IRequestContext>();
        _loggerMock = new Mock<ICustomLogger<UpdateContentCompletionStatusHandler>>();

        _transactionServiceMock
            .Setup(x =>
                x.ExecuteInTransactionAsync(
                    It.IsAny<Func<Task<UpdateContentCompletionStatusResponse>>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns(
                (Func<Task<UpdateContentCompletionStatusResponse>> op, CancellationToken _) => op()
            );

        _handler = new UpdateContentCompletionStatusHandler(
            _courseContentServiceMock.Object,
            _transactionServiceMock.Object,
            _requestContextMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldCreateNewProgress_WhenNoExistingProgress()
    {
        var contentId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var enrollmentId = Guid.NewGuid();
        var command = new UpdateContentCompletionStatusCommand(
            contentId,
            new UpdateContentCompletionStatusRequest { IsCompleted = true }
        );
        var content = new CourseContent
        {
            Id = contentId,
            MetaTopic = new CourseMetaTopic { CourseId = courseId },
        };

        _requestContextMock.Setup(x => x.UserId).Returns(userId);
        _courseContentServiceMock
            .Setup(x => x.GetContentWithMetaTopicAsync(contentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(content);
        _courseContentServiceMock
            .Setup(x =>
                x.GetEnrollmentByUserAndCourseAsync(userId, courseId, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(new Enrollment { Id = enrollmentId });
        _courseContentServiceMock
            .Setup(x => x.GetProgressAsync(enrollmentId, contentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserCourseProgress?)null);
        _courseContentServiceMock
            .Setup(x => x.GetTotalContentCountAsync(courseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(5);
        _courseContentServiceMock
            .Setup(x =>
                x.GetCompletedContentCountAsync(enrollmentId, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(1);
        _courseContentServiceMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal(contentId, result.CourseContentId);
        Assert.True(result.IsCompleted);
        Assert.Equal(40m, result.CourseProgressPercentage); // (1 existing + 1 new) / 5 = 40%
    }

    [Fact]
    public async Task Handle_ShouldCalculateCorrectPercentage_WhenFirstContentCompleted()
    {
        var contentId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var enrollmentId = Guid.NewGuid();
        var command = new UpdateContentCompletionStatusCommand(
            contentId,
            new UpdateContentCompletionStatusRequest { IsCompleted = true }
        );
        var content = new CourseContent
        {
            Id = contentId,
            MetaTopic = new CourseMetaTopic { CourseId = courseId },
        };

        _requestContextMock.Setup(x => x.UserId).Returns(userId);
        _courseContentServiceMock
            .Setup(x => x.GetContentWithMetaTopicAsync(contentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(content);
        _courseContentServiceMock
            .Setup(x =>
                x.GetEnrollmentByUserAndCourseAsync(userId, courseId, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(new Enrollment { Id = enrollmentId });
        _courseContentServiceMock
            .Setup(x => x.GetProgressAsync(enrollmentId, contentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserCourseProgress?)null);
        _courseContentServiceMock
            .Setup(x => x.GetTotalContentCountAsync(courseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(2);
        _courseContentServiceMock
            .Setup(x =>
                x.GetCompletedContentCountAsync(enrollmentId, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(0);
        _courseContentServiceMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsCompleted);
        Assert.Equal(50m, result.CourseProgressPercentage); // (0 existing + 1 new) / 2 = 50%
    }

    [Fact]
    public async Task Handle_ShouldUpdateExistingProgress_WhenProgressExists()
    {
        var contentId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var enrollmentId = Guid.NewGuid();
        var command = new UpdateContentCompletionStatusCommand(
            contentId,
            new UpdateContentCompletionStatusRequest { IsCompleted = true }
        );
        var content = new CourseContent
        {
            Id = contentId,
            MetaTopic = new CourseMetaTopic { CourseId = courseId },
        };
        var existingProgress = new UserCourseProgress
        {
            EnrollmentId = enrollmentId,
            CourseContentId = contentId,
            IsCompleted = false,
        };

        _requestContextMock.Setup(x => x.UserId).Returns(userId);
        _courseContentServiceMock
            .Setup(x => x.GetContentWithMetaTopicAsync(contentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(content);
        _courseContentServiceMock
            .Setup(x =>
                x.GetEnrollmentByUserAndCourseAsync(userId, courseId, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(new Enrollment { Id = enrollmentId });
        _courseContentServiceMock
            .Setup(x => x.GetProgressAsync(enrollmentId, contentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProgress);
        _courseContentServiceMock
            .Setup(x => x.GetTotalContentCountAsync(courseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(5);
        _courseContentServiceMock
            .Setup(x =>
                x.GetCompletedContentCountAsync(enrollmentId, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(2);
        _courseContentServiceMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsCompleted);
        Assert.True(existingProgress.IsCompleted);
        Assert.Equal(60m, result.CourseProgressPercentage); // (2 existing + 1 toggled) / 5 = 60%
    }

    [Fact]
    public async Task Handle_ShouldReducePercentage_WhenTogglingFromCompleteToIncomplete()
    {
        var contentId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var enrollmentId = Guid.NewGuid();
        var command = new UpdateContentCompletionStatusCommand(
            contentId,
            new UpdateContentCompletionStatusRequest { IsCompleted = false }
        );
        var content = new CourseContent
        {
            Id = contentId,
            MetaTopic = new CourseMetaTopic { CourseId = courseId },
        };
        var existingProgress = new UserCourseProgress
        {
            EnrollmentId = enrollmentId,
            CourseContentId = contentId,
            IsCompleted = true,
        };

        _requestContextMock.Setup(x => x.UserId).Returns(userId);
        _courseContentServiceMock
            .Setup(x => x.GetContentWithMetaTopicAsync(contentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(content);
        _courseContentServiceMock
            .Setup(x =>
                x.GetEnrollmentByUserAndCourseAsync(userId, courseId, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(new Enrollment { Id = enrollmentId });
        _courseContentServiceMock
            .Setup(x => x.GetProgressAsync(enrollmentId, contentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProgress);
        _courseContentServiceMock
            .Setup(x => x.GetTotalContentCountAsync(courseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(4);
        _courseContentServiceMock
            .Setup(x =>
                x.GetCompletedContentCountAsync(enrollmentId, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(3);
        _courseContentServiceMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsCompleted);
        Assert.Equal(50m, result.CourseProgressPercentage); // (3 existing - 1 toggled) / 4 = 50%
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenContentNotFound()
    {
        var command = new UpdateContentCompletionStatusCommand(
            Guid.NewGuid(),
            new UpdateContentCompletionStatusRequest { IsCompleted = true }
        );

        _requestContextMock.Setup(x => x.UserId).Returns(Guid.NewGuid());
        _courseContentServiceMock
            .Setup(x =>
                x.GetContentWithMetaTopicAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync((CourseContent?)null);

        var ex = await Assert.ThrowsAsync<NotFoundException>(() =>
            _handler.Handle(command, CancellationToken.None)
        );

        Assert.Equal("Course content not found", ex.Message);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenNotEnrolled()
    {
        var contentId = Guid.NewGuid();
        var command = new UpdateContentCompletionStatusCommand(
            contentId,
            new UpdateContentCompletionStatusRequest { IsCompleted = true }
        );
        var content = new CourseContent
        {
            Id = contentId,
            MetaTopic = new CourseMetaTopic { CourseId = Guid.NewGuid() },
        };

        _requestContextMock.Setup(x => x.UserId).Returns(Guid.NewGuid());
        _courseContentServiceMock
            .Setup(x => x.GetContentWithMetaTopicAsync(contentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(content);
        _courseContentServiceMock
            .Setup(x =>
                x.GetEnrollmentByUserAndCourseAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync((Enrollment?)null);

        var ex = await Assert.ThrowsAsync<BadRequestException>(() =>
            _handler.Handle(command, CancellationToken.None)
        );

        Assert.Equal("Not enrolled", ex.Message);
    }
}
