using AutoMapper;
using LAP.Application.DTO.Assessment;
using LAP.Application.Feature.Assessment.Query;
using LAP.Application.Interface;
using LAP.Application.Interface.IService;
using Moq;

namespace LAP.UnitTest.Handler.Assessment;

public class GetAllAssessmentHandlerTest
{
    private readonly Mock<IAssessmentService> _assessmentServiceMock;
    private readonly Mock<ICustomLogger<GetAllAssessmentHandler>> _loggerMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetAllAssessmentHandler _handler;

    public GetAllAssessmentHandlerTest()
    {
        _assessmentServiceMock = new Mock<IAssessmentService>();
        _loggerMock = new Mock<ICustomLogger<GetAllAssessmentHandler>>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetAllAssessmentHandler(
            _assessmentServiceMock.Object,
            _loggerMock.Object,
            _mapperMock.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldReturnPaginatedAssessments()
    {
        var assessments = new List<LAP.Domain.Entity.Assessment>
        {
            new() { Id = Guid.NewGuid(), Title = "Assessment 1" },
            new() { Id = Guid.NewGuid(), Title = "Assessment 2" },
        };

        var dtos = new List<AssessmentOverviewDto>
        {
            new() { Id = assessments[0].Id, Title = "Assessment 1" },
            new() { Id = assessments[1].Id, Title = "Assessment 2" },
        };

        _assessmentServiceMock
            .Setup(x => x.GetAllAssessmentPaginatedAsync(1, 25, It.IsAny<CancellationToken>()))
            .ReturnsAsync((assessments, 2));
        _mapperMock.Setup(x => x.Map<List<AssessmentOverviewDto>>(assessments)).Returns(dtos);

        var result = await _handler.Handle(
            new GetAllAssessmentQuery(1, 25),
            CancellationToken.None
        );

        Assert.Equal(2, result.Data.Count());
        Assert.Equal(2, result.Total);
        Assert.Equal(1, result.Page);
        Assert.Equal(25, result.PageSize);
        Assert.Equal("Assessment 1", result.Data.First().Title);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyData_WhenNoAssessments()
    {
        _assessmentServiceMock
            .Setup(x => x.GetAllAssessmentPaginatedAsync(1, 25, It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<LAP.Domain.Entity.Assessment>(), 0));
        _mapperMock
            .Setup(x =>
                x.Map<List<AssessmentOverviewDto>>(It.IsAny<List<LAP.Domain.Entity.Assessment>>())
            )
            .Returns(new List<AssessmentOverviewDto>());

        var result = await _handler.Handle(
            new GetAllAssessmentQuery(1, 25),
            CancellationToken.None
        );

        Assert.Empty(result.Data);
        Assert.Equal(0, result.Total);
    }
}
