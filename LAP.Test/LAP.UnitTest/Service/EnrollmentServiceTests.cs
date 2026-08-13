using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using LAP.Application.Interface;
using LAP.Application.Interface.IRepository;
using LAP.Application.Service;
using LAP.Domain.Entity;
using LAP.UnitTest.Helpers;
using Moq;
using Xunit;

namespace LAP.UnitTest.Service;

public class EnrollmentServiceTests
{
    private readonly Mock<IRepositoryWrapper> _repoMock;
    private readonly Mock<IEnrollmentRepository> _enrollmentRepoMock;
    private readonly Mock<ICustomLogger<EnrollmentService>> _loggerMock;
    private readonly EnrollmentService _service;

    public EnrollmentServiceTests()
    {
        _repoMock = new Mock<IRepositoryWrapper>();
        _enrollmentRepoMock = new Mock<IEnrollmentRepository>();
        _loggerMock = new Mock<ICustomLogger<EnrollmentService>>();

        _repoMock.Setup(r => r.Enrollment).Returns(_enrollmentRepoMock.Object);

        _service = new EnrollmentService(_repoMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetEnrollmentAsync_ShouldReturnList()
    {
        var enrollments = new List<Enrollment> { new Enrollment() };
        _enrollmentRepoMock
            .Setup(r => r.GetByConditionNoTracking(It.IsAny<Expression<Func<Enrollment, bool>>>()))
            .Returns(enrollments.AsAsyncQueryable());

        var result = await _service.GetEnrollmentAsync(null, null, null);

        Assert.Equal(enrollments, result);
    }

    [Fact]
    public async Task GetEnrollmentByIdWithDetailAsync_ShouldReturnEnrollment()
    {
        var id = Guid.NewGuid();
        var enrollment = new Enrollment { Id = id };
        _enrollmentRepoMock
            .Setup(r => r.GetByConditionNoTracking(It.IsAny<Expression<Func<Enrollment, bool>>>()))
            .Returns(new List<Enrollment> { enrollment }.AsAsyncQueryable());

        var result = await _service.GetEnrollmentByIdWithDetailAsync(id);

        Assert.Equal(enrollment, result);
    }

    [Fact]
    public void UpdateEnrollment_ShouldCallRepo()
    {
        var enrollment = new Enrollment { Id = Guid.NewGuid() };

        _service.UpdateEnrollment(enrollment);

        _enrollmentRepoMock.Verify(r => r.Update(enrollment), Times.Once);
    }

    [Fact]
    public async Task AddEnrollmentAsync_ShouldReturnAddedEnrollment()
    {
        var enrollment = new Enrollment { UserId = Guid.NewGuid(), CourseId = Guid.NewGuid() };
        _enrollmentRepoMock
            .Setup(r => r.AddAsync(enrollment, It.IsAny<CancellationToken>()))
            .ReturnsAsync(enrollment);

        var result = await _service.AddEnrollmentAsync(enrollment);

        Assert.Equal(enrollment, result);
    }
}
