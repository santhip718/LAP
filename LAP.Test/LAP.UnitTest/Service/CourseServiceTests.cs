using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using LAP.Application.Interface;
using LAP.Application.Interface.IRepository;
using LAP.Application.Service;
using LAP.Domain.Entity;
using Moq;
using Xunit;

namespace LAP.UnitTest.Service;

public class CourseServiceTests
{
    private readonly Mock<IRepositoryWrapper> _repoMock;
    private readonly Mock<ICourseRepository> _courseRepoMock;
    private readonly Mock<ICustomLogger<CourseService>> _loggerMock;
    private readonly CourseService _service;

    public CourseServiceTests()
    {
        _repoMock = new Mock<IRepositoryWrapper>();
        _courseRepoMock = new Mock<ICourseRepository>();
        _loggerMock = new Mock<ICustomLogger<CourseService>>();

        _repoMock.Setup(r => r.Course).Returns(_courseRepoMock.Object);

        _service = new CourseService(_repoMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetCourseByIdAsync_ShouldReturnCourse()
    {
        var id = Guid.NewGuid();
        var course = new Course { Id = id };
        _courseRepoMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);

        var result = await _service.GetCourseByIdAsync(id);

        Assert.Equal(course, result);
    }

    [Fact]
    public async Task AddCourseAsync_ShouldReturnAddedCourse()
    {
        var course = new Course { Title = "Test" };
        _courseRepoMock
            .Setup(r => r.AddAsync(course, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);

        var result = await _service.AddCourseAsync(course);

        Assert.Equal(course, result);
    }

    [Fact]
    public void UpdateCourse_ShouldCallRepo()
    {
        var course = new Course { Id = Guid.NewGuid() };

        _service.UpdateCourse(course);

        _courseRepoMock.Verify(r => r.Update(course), Times.Once);
    }

    [Fact]
    public async Task DeleteCourse_ShouldCallRepo()
    {
        var id = Guid.NewGuid();
        var course = new Course { Id = id };

        _courseRepoMock
            .Setup(r => r.SoftDeleteAsync(It.IsAny<Expression<Func<Course, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _service.DeleteCourseAsync(id);

        _courseRepoMock.Verify(
            r => r.SoftDeleteAsync(It.IsAny<Expression<Func<Course, bool>>>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
        Assert.Equal(1, result);
    }
}
