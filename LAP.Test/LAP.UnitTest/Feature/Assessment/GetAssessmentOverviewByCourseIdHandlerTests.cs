using AutoMapper;
using LAP.Application.DTO.Assessment;
using LAP.Application.Feature.Assessment.Query;
using LAP.Application.Interface;
using LAP.Application.Interface.IService;
using Moq;

namespace LAP.UnitTest.Handler.Assessment;

public class GetAssessmentOverviewByCourseIdHandlerTest
{
    private readonly Mock<IAssessmentService> _assessmentServiceMock;
    private readonly Mock<ICustomLogger<GetAssessmentOverviewByCourseIdHandler>> _loggerMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetAssessmentOverviewByCourseIdHandler _handler;

    public GetAssessmentOverviewByCourseIdHandlerTest()
    {
        _assessmentServiceMock = new Mock<IAssessmentService>();
        _loggerMock = new Mock<ICustomLogger<GetAssessmentOverviewByCourseIdHandler>>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetAssessmentOverviewByCourseIdHandler(
            _assessmentServiceMock.Object,
            _loggerMock.Object,
            _mapperMock.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldReturnAssessmentsForCourse()
    {
        var courseId = Guid.NewGuid();
        var assessments = new List<LAP.Domain.Entity.Assessment>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Exam 1",
                CourseId = courseId,
            },
        };
        var dtos = new List<AssessmentOverviewDto>
        {
            new() { Id = assessments[0].Id, Title = "Exam 1" },
        };

        _assessmentServiceMock
            .Setup(x => x.GetByCourseIdAsync(courseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(assessments);
        _mapperMock.Setup(x => x.Map<List<AssessmentOverviewDto>>(assessments)).Returns(dtos);

        var result = await _handler.Handle(
            new GetAssessmentOverviewByCourseIdQuery(courseId),
            CancellationToken.None
        );

        Assert.Single(result);
        Assert.Equal("Exam 1", result[0].Title);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmpty_WhenNoAssessmentsForCourse()
    {
        var courseId = Guid.NewGuid();

        _assessmentServiceMock
            .Setup(x => x.GetByCourseIdAsync(courseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<LAP.Domain.Entity.Assessment>());
        _mapperMock
            .Setup(x =>
                x.Map<List<AssessmentOverviewDto>>(It.IsAny<List<LAP.Domain.Entity.Assessment>>())
            )
            .Returns(new List<AssessmentOverviewDto>());

        var result = await _handler.Handle(
            new GetAssessmentOverviewByCourseIdQuery(courseId),
            CancellationToken.None
        );

        Assert.Empty(result);
    }
}
