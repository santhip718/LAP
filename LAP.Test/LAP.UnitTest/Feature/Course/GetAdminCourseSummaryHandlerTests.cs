using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LAP.Application.DTO.Course;
using LAP.Application.Feature.Course.Query;
using LAP.Application.Interface;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using Moq;
using Xunit;

namespace LAP.UnitTest.Feature.Course;

public class GetAdminCourseSummaryHandlerTests
{
    private readonly Mock<ICustomLogger<GetAdminCourseSummaryHandler>> _loggerMock;
    private readonly Mock<ICourseService> _courseServiceMock;
    private readonly Mock<IEnrollmentService> _enrollmentServiceMock;
    private readonly GetAdminCourseSummaryHandler _handler;

    public GetAdminCourseSummaryHandlerTests()
    {
        _loggerMock = new Mock<ICustomLogger<GetAdminCourseSummaryHandler>>();
        _courseServiceMock = new Mock<ICourseService>();
        _enrollmentServiceMock = new Mock<IEnrollmentService>();
        _handler = new GetAdminCourseSummaryHandler(
            _loggerMock.Object,
            _courseServiceMock.Object,
            _enrollmentServiceMock.Object
        );
    }

    [Fact]
    public async Task Handle_ReturnsSummary()
    {
        // Arrange
        var courses = new List<LAP.Domain.Entity.Course>
        {
            new LAP.Domain.Entity.Course { IsDrafted = false },
            new LAP.Domain.Entity.Course { IsDrafted = true },
        };
        var enrollments = new List<LAP.Domain.Entity.Enrollment>
        {
            new LAP.Domain.Entity.Enrollment { UserId = Guid.NewGuid() },
        };

        _courseServiceMock
            .Setup(s => s.GetAllCourseAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(courses);
        _enrollmentServiceMock
            .Setup(s => s.GetAllEnrollmentAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(enrollments);

        // Act
        var result = await _handler.Handle(
            new GetAdminCourseSummaryQuery(),
            CancellationToken.None
        );

        // Assert
        Assert.Equal(2, result.TotalCourses);
        Assert.Equal(1, result.PublishedCourses);
        Assert.Equal(1, result.DraftCourses);
        Assert.Equal(1, result.TotalEnrollments);
        Assert.Equal(1, result.ActiveStudents);
    }
}
