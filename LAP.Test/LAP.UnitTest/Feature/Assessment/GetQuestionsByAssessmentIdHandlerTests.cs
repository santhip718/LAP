using AutoMapper;
using LAP.Application.DTO.Assessment;
using LAP.Application.Feature.Assessment.Query;
using LAP.Application.Interface;
using LAP.Application.Interface.IContext;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using Moq;

namespace LAP.UnitTest.Handler.Assessment;

public class GetQuestionByAssessmentIdHandlerTest
{
    private readonly Mock<IAssessmentService> _assessmentServiceMock;
    private readonly Mock<ICustomLogger<GetQuestionByAssessmentIdHandler>> _loggerMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRequestContext> _requestContextMock;
    private readonly Mock<IPermissionCacheService> _permissionCacheServiceMock;
    private readonly GetQuestionByAssessmentIdHandler _handler;

    public GetQuestionByAssessmentIdHandlerTest()
    {
        _assessmentServiceMock = new Mock<IAssessmentService>();
        _loggerMock = new Mock<ICustomLogger<GetQuestionByAssessmentIdHandler>>();
        _mapperMock = new Mock<IMapper>();
        _requestContextMock = new Mock<IRequestContext>();
        _permissionCacheServiceMock = new Mock<IPermissionCacheService>();
        _handler = new GetQuestionByAssessmentIdHandler(
            _assessmentServiceMock.Object,
            _loggerMock.Object,
            _mapperMock.Object,
            _requestContextMock.Object,
            _permissionCacheServiceMock.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldReturnQuestions()
    {
        var assessmentId = Guid.NewGuid();
        var assessment = new LAP.Domain.Entity.Assessment { Id = assessmentId, Title = "Test Assessment" };
        var questions = new List<Question>
        {
            new()
            {
                Id = Guid.NewGuid(),
                QuestionText = "Q1",
                AssessmentId = assessmentId,
            },
            new()
            {
                Id = Guid.NewGuid(),
                QuestionText = "Q2",
                AssessmentId = assessmentId,
            },
        };
        var dtos = new List<QuestionDto>
        {
            new() { Id = questions[0].Id, QuestionText = "Q1" },
            new() { Id = questions[1].Id, QuestionText = "Q2" },
        };

        _requestContextMock.Setup(x => x.Role).Returns("Admin");
        _permissionCacheServiceMock
            .Setup(x => x.GetPermissionsAsync("Admin", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HashSet<string> { "MANAGE_ASSESSMENT" });

        _assessmentServiceMock
            .Setup(x => x.GetAssessmentByIdAsync(assessmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(assessment);
        _assessmentServiceMock
            .Setup(x =>
                x.GetQuestionByAssessmentIdAsync(assessmentId, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(questions);
        _mapperMock.Setup(x => x.Map<List<QuestionDto>>(questions)).Returns(dtos);

        var result = await _handler.Handle(
            new GetQuestionByAssessmentIdQuery(assessmentId),
            CancellationToken.None
        );

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmpty_WhenNoQuestions()
    {
        var assessmentId = Guid.NewGuid();
        var assessment = new LAP.Domain.Entity.Assessment { Id = assessmentId, Title = "Test Assessment" };

        _requestContextMock.Setup(x => x.Role).Returns("Admin");
        _permissionCacheServiceMock
            .Setup(x => x.GetPermissionsAsync("Admin", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HashSet<string> { "MANAGE_ASSESSMENT" });

        _assessmentServiceMock
            .Setup(x => x.GetAssessmentByIdAsync(assessmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(assessment);
        _assessmentServiceMock
            .Setup(x =>
                x.GetQuestionByAssessmentIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(new List<Question>());
        _mapperMock
            .Setup(x => x.Map<List<QuestionDto>>(It.IsAny<List<Question>>()))
            .Returns(new List<QuestionDto>());

        var result = await _handler.Handle(
            new GetQuestionByAssessmentIdQuery(assessmentId),
            CancellationToken.None
        );

        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_ShouldHideAnswers_WhenUserIsNotAdmin()
    {
        var assessmentId = Guid.NewGuid();
        var assessment = new LAP.Domain.Entity.Assessment { Id = assessmentId, Title = "Test Assessment" };
        var questions = new List<Question>
        {
            new()
            {
                Id = Guid.NewGuid(),
                QuestionText = "Q1",
                AssessmentId = assessmentId,
                Answer = "CorrectAnswer",
            },
        };
        var dtos = new List<QuestionDto>
        {
            new() { Id = questions[0].Id, QuestionText = "Q1", Answer = "CorrectAnswer" },
        };

        _requestContextMock.Setup(x => x.Role).Returns("Student");
        _permissionCacheServiceMock
            .Setup(x => x.GetPermissionsAsync("Student", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HashSet<string>());

        _assessmentServiceMock
            .Setup(x => x.GetAssessmentByIdAsync(assessmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(assessment);
        _assessmentServiceMock
            .Setup(x =>
                x.GetQuestionByAssessmentIdAsync(assessmentId, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(questions);
        _mapperMock.Setup(x => x.Map<List<QuestionDto>>(questions)).Returns(dtos);

        var result = await _handler.Handle(
            new GetQuestionByAssessmentIdQuery(assessmentId),
            CancellationToken.None
        );

        Assert.Single(result);
        Assert.Null(result[0].Answer);
    }

    [Fact]
    public async Task Handle_ShouldShowAnswers_WhenUserHasManageAssessmentAccess()
    {
        var assessmentId = Guid.NewGuid();
        var assessment = new LAP.Domain.Entity.Assessment { Id = assessmentId, Title = "Test Assessment" };
        var questions = new List<Question>
        {
            new()
            {
                Id = Guid.NewGuid(),
                QuestionText = "Q1",
                AssessmentId = assessmentId,
                Answer = "CorrectAnswer",
            },
        };
        var dtos = new List<QuestionDto>
        {
            new() { Id = questions[0].Id, QuestionText = "Q1", Answer = "CorrectAnswer" },
        };

        _requestContextMock.Setup(x => x.Role).Returns("Admin");
        _permissionCacheServiceMock
            .Setup(x => x.GetPermissionsAsync("Admin", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HashSet<string> { "MANAGE_ASSESSMENT" });

        _assessmentServiceMock
            .Setup(x => x.GetAssessmentByIdAsync(assessmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(assessment);
        _assessmentServiceMock
            .Setup(x =>
                x.GetQuestionByAssessmentIdAsync(assessmentId, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(questions);
        _mapperMock.Setup(x => x.Map<List<QuestionDto>>(questions)).Returns(dtos);

        var result = await _handler.Handle(
            new GetQuestionByAssessmentIdQuery(assessmentId),
            CancellationToken.None
        );

        Assert.Single(result);
        Assert.Equal("CorrectAnswer", result[0].Answer);
    }
}
