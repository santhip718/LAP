using System;
using System.Threading;
using System.Threading.Tasks;
using LAP.Application.DTO.Enrollment;
using LAP.Application.Feature.Enrollment.Command;
using LAP.Application.Interface;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using LAP.Shared.Exceptions;
using Moq;
using Xunit;
using EnrollmentEntity = LAP.Domain.Entity.Enrollment;

namespace LAP.UnitTest.Feature.Enrollment;

public class UpdateEnrollmentHandlerTests
{
    private readonly Mock<IEnrollmentService> _enrollmentServiceMock;
    private readonly Mock<ITransactionService> _transactionServiceMock;
    private readonly Mock<ICustomLogger<UpdateEnrollmentHandler>> _loggerMock;
    private readonly UpdateEnrollmentHandler _handler;

    public UpdateEnrollmentHandlerTests()
    {
        _enrollmentServiceMock = new Mock<IEnrollmentService>();
        _transactionServiceMock = new Mock<ITransactionService>();
        _loggerMock = new Mock<ICustomLogger<UpdateEnrollmentHandler>>();

        _transactionServiceMock.Setup(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task<LAP.Application.DTO.Common.SuccessResponse>>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<Task<LAP.Application.DTO.Common.SuccessResponse>>, CancellationToken>(async (op, ct) => await op());

        _handler = new UpdateEnrollmentHandler(_enrollmentServiceMock.Object, _transactionServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingEnrollment_UpdatesStatus()
    {
        // Arrange
        var id = Guid.NewGuid();
        var dto = new UpdateEnrollmentRequestDto { EnrollmentStatus = false };
        var enrollment = new EnrollmentEntity { Id = id, EnrollmentStatus = true };
        
        _enrollmentServiceMock.Setup(s => s.GetEnrollmentByIdWithDetailAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(enrollment);

        // Act
        var result = await _handler.Handle(new UpdateEnrollmentCommand(id, dto), CancellationToken.None);

        // Assert
        Assert.Equal(id, result.Id);
        Assert.False(enrollment.EnrollmentStatus);
        _enrollmentServiceMock.Verify(s => s.UpdateEnrollment(enrollment), Times.Once);
        _transactionServiceMock.Verify(s => s.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistingEnrollment_ThrowsNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();
        _enrollmentServiceMock.Setup(s => s.GetEnrollmentByIdWithDetailAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((EnrollmentEntity?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(new UpdateEnrollmentCommand(id, new UpdateEnrollmentRequestDto()), CancellationToken.None));
    }
}
