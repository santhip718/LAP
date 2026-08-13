using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using LAP.Application.DTO.Enrollment;
using LAP.Application.Feature.Enrollment.Query;
using LAP.Application.Interface;
using LAP.Application.Interface.IContext;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using Moq;
using Xunit;
using EnrollmentEntity = LAP.Domain.Entity.Enrollment;

namespace LAP.UnitTest.Feature.Enrollment;

public class GetEnrollmentHandlerTests
{
    private readonly Mock<IEnrollmentService> _enrollmentServiceMock;
    private readonly Mock<ICustomLogger<GetEnrollmentHandler>> _loggerMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRequestContext> _requestContextMock;
    private readonly GetEnrollmentHandler _handler;

    public GetEnrollmentHandlerTests()
    {
        _enrollmentServiceMock = new Mock<IEnrollmentService>();
        _loggerMock = new Mock<ICustomLogger<GetEnrollmentHandler>>();
        _mapperMock = new Mock<IMapper>();
        _requestContextMock = new Mock<IRequestContext>();
        _handler = new GetEnrollmentHandler(
            _enrollmentServiceMock.Object,
            _loggerMock.Object,
            _mapperMock.Object,
            _requestContextMock.Object
        );
    }

    [Fact]
    public async Task Handle_ReturnsPaginatedEnrollments()
    {
        // Arrange
        var enrollments = new List<EnrollmentEntity> { new EnrollmentEntity() };
        _enrollmentServiceMock
            .Setup(s =>
                s.GetEnrollmentAsync(
                    It.IsAny<string>(),
                    It.IsAny<Guid?>(),
                    It.IsAny<Guid?>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(enrollments);

        var mapped = new List<EnrollmentDetailDto> { new EnrollmentDetailDto() };
        _mapperMock
            .Setup(m => m.Map<List<EnrollmentDetailDto>>(It.IsAny<List<EnrollmentEntity>>()))
            .Returns(mapped);

        var query = new GetEnrollmentQuery("test", null, 1, 10);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Single(result.Data);
        Assert.Equal(1, result.Total);
    }
}
