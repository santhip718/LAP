using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using LAP.Application.Feature.Enrollment.Command;
using LAP.Application.Interface;
using LAP.Application.Interface.IContext;
using LAP.Application.Interface.IRepository;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using LAP.Shared.Exceptions;
using Moq;
using Xunit;
using EnrollmentEntity = LAP.Domain.Entity.Enrollment;

namespace LAP.UnitTest.Feature.Enrollment;

public class CreateEnrollmentHandlerTests
{
    private readonly Mock<IEnrollmentService> _enrollmentServiceMock;
    private readonly Mock<ICourseService> _courseServiceMock;
    private readonly Mock<ITransactionService> _transactionServiceMock;
    private readonly Mock<IRequestContext> _requestContextMock;
    private readonly Mock<ICustomLogger<CreateEnrollmentHandler>> _loggerMock;
    private readonly CreateEnrollmentHandler _handler;

    public CreateEnrollmentHandlerTests()
    {
        _enrollmentServiceMock = new Mock<IEnrollmentService>();
        _courseServiceMock = new Mock<ICourseService>();
        _transactionServiceMock = new Mock<ITransactionService>();
        _requestContextMock = new Mock<IRequestContext>();
        _loggerMock = new Mock<ICustomLogger<CreateEnrollmentHandler>>();

        _transactionServiceMock.Setup(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task<LAP.Application.DTO.Common.SuccessResponse>>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<Task<LAP.Application.DTO.Common.SuccessResponse>>, CancellationToken>(async (op, ct) => await op());

        _handler = new CreateEnrollmentHandler(
            _enrollmentServiceMock.Object,
            _courseServiceMock.Object,
            _transactionServiceMock.Object,
            _requestContextMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_CreatesEnrollment()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var course = new LAP.Domain.Entity.Course { Id = courseId };

        _courseServiceMock.Setup(s => s.GetCourseByIdAsync(courseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);
        _requestContextMock.Setup(r => r.UserId).Returns(userId);
        _enrollmentServiceMock.Setup(s => s.IsUserEnrolledAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(new CreateEnrollmentCommand(courseId), CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, result.Id);
        _enrollmentServiceMock.Verify(s => s.AddEnrollmentAsync(It.IsAny<EnrollmentEntity>(), It.IsAny<CancellationToken>()), Times.Once);
        _transactionServiceMock.Verify(s => s.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_CourseNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        _courseServiceMock.Setup(s => s.GetCourseByIdAsync(courseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((LAP.Domain.Entity.Course?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(new CreateEnrollmentCommand(courseId), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_UserAlreadyEnrolled_ThrowsBadRequestException()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        _courseServiceMock.Setup(s => s.GetCourseByIdAsync(courseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new LAP.Domain.Entity.Course());
        _requestContextMock.Setup(r => r.UserId).Returns(userId);
        _enrollmentServiceMock.Setup(s => s.IsUserEnrolledAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _handler.Handle(new CreateEnrollmentCommand(courseId), CancellationToken.None));
    }
}
