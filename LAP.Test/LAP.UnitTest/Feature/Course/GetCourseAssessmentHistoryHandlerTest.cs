using AutoMapper;
using LAP.Application.DTO.Assessment;
using LAP.Application.Feature.Course.Query;
using LAP.Application.Interface;
using LAP.Application.Interface.IContext;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using Moq;

namespace LAP.UnitTest.Features.CourseHandlers;

public class GetCourseAssessmentHistoryHandlerTest
{
    private readonly Mock<ICourseService> _courseServiceMock;
    private readonly Mock<IRequestContext> _requestContextMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ICustomLogger<GetCourseAssessmentHistoryHandler>> _loggerMock;
    private readonly GetCourseAssessmentHistoryHandler _handler;

    public GetCourseAssessmentHistoryHandlerTest()
    {
        _courseServiceMock = new Mock<ICourseService>();
        _requestContextMock = new Mock<IRequestContext>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ICustomLogger<GetCourseAssessmentHistoryHandler>>();
        _handler = new GetCourseAssessmentHistoryHandler(
            _courseServiceMock.Object,
            _requestContextMock.Object,
            _mapperMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldReturnPaginatedHistory()
    {
        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var query = new GetCourseAssessmentHistoryQuery(courseId, 1, 10);
        var histories = new List<AssessmentHistory> { new AssessmentHistory() };
        var dtos = new List<AssessmentHistoryDto> { new AssessmentHistoryDto() };

        _requestContextMock.Setup(x => x.UserId).Returns(userId);
        _courseServiceMock
            .Setup(x =>
                x.GetAssessmentHistoryAsync(courseId, userId, 1, 10, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync((histories, 1));
        _mapperMock.Setup(x => x.Map<ICollection<AssessmentHistoryDto>>(histories)).Returns(dtos);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.Single(result.Data!);
        Assert.Equal(1, result.Total);
    }

    [Fact]
    public async Task Handle_ShouldUseEmptyGuid_WhenUserNotAuthenticated()
    {
        var query = new GetCourseAssessmentHistoryQuery(Guid.NewGuid(), 1, 10);

        _requestContextMock.Setup(x => x.UserId).Returns((Guid?)null);
        _courseServiceMock
            .Setup(x =>
                x.GetAssessmentHistoryAsync(
                    It.IsAny<Guid>(),
                    Guid.Empty,
                    1,
                    10,
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync((new List<AssessmentHistory>(), 0));
        _mapperMock
            .Setup(x =>
                x.Map<ICollection<AssessmentHistoryDto>>(It.IsAny<IEnumerable<AssessmentHistory>>())
            )
            .Returns(new List<AssessmentHistoryDto>());

        await _handler.Handle(query, CancellationToken.None);

        _courseServiceMock.Verify(
            x =>
                x.GetAssessmentHistoryAsync(
                    It.IsAny<Guid>(),
                    Guid.Empty,
                    1,
                    10,
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }
}
