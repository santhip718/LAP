using AutoMapper;
using LAP.Application.DTO.Assessment;
using LAP.Application.Feature.Assessment.Command;
using LAP.Application.Interface;
using LAP.Application.Interface.IHelper;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using LAP.Shared.Exceptions;
using Microsoft.AspNetCore.Http;
using Moq;

namespace LAP.UnitTest.Handler.Assessment;

public class CreateAssessmentHandlerTest
{
    private readonly Mock<IAssessmentService> _assessmentServiceMock;
    private readonly Mock<ITransactionService> _transactionServiceMock;
    private readonly Mock<ICustomLogger<CreateAssessmentHandler>> _loggerMock;
    private readonly Mock<IQuestionParser> _questionParserMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CreateAssessmentHandler _handler;

    public CreateAssessmentHandlerTest()
    {
        _assessmentServiceMock = new Mock<IAssessmentService>();
        _transactionServiceMock = new Mock<ITransactionService>();
        _loggerMock = new Mock<ICustomLogger<CreateAssessmentHandler>>();
        _questionParserMock = new Mock<IQuestionParser>();
        _mapperMock = new Mock<IMapper>();

        _handler = new CreateAssessmentHandler(
            _assessmentServiceMock.Object,
            _transactionServiceMock.Object,
            _loggerMock.Object,
            _questionParserMock.Object,
            _mapperMock.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldCreateAssessment_WhenDataIsValid()
    {
        var courseId = Guid.NewGuid();
        var metaTopicId = Guid.NewGuid();
        var questionTypeId = Guid.NewGuid();
        var fileMock = new Mock<IFormFile>();
        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("dummy"));
        fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
        fileMock.Setup(f => f.FileName).Returns("questions.xlsx");
        fileMock.Setup(f => f.Length).Returns(5);

        var command = new CreateAssessmentCommand(
            courseId,
            "Test Assessment",
            "Description",
            1,
            60,
            fileMock.Object
        );

        var importedQuestions = new List<QuestionImportDto>
        {
            new()
            {
                QuestionText = "Q1",
                QuestionTypeName = "MCQ",
                MetaTopicName = "Topic 1",
                Option1 = "A",
                Option2 = "B",
                Option3 = "C",
                Option4 = "D",
                Answer = "A",
                Weight = 1,
            },
        };

        _assessmentServiceMock
            .Setup(x => x.ActiveAssessmentExistsForCourseAsync(courseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _questionParserMock
            .Setup(x => x.ParseQuestionAsync(fileMock.Object))
            .ReturnsAsync(importedQuestions);

        _assessmentServiceMock
            .Setup(x => x.GetMetaTopicByCourseIdAsync(courseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new List<CourseMetaTopic>
                {
                    new()
                    {
                        Id = metaTopicId,
                        Name = "Topic 1",
                        CourseId = courseId,
                    },
                }
            );

        _assessmentServiceMock
            .Setup(x => x.GetQuestionTypeAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new List<RefTerm>
                {
                    new() { Id = questionTypeId, Name = "MCQ" },
                }
            );

        _transactionServiceMock
            .Setup(x =>
                x.ExecuteInTransactionAsync(
                    It.IsAny<Func<Task<Guid>>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Callback<Func<Task<Guid>>, CancellationToken>((func, ct) => func())
            .ReturnsAsync(Guid.NewGuid);

        _assessmentServiceMock
            .Setup(x =>
                x.AddAssessmentAsync(
                    It.IsAny<LAP.Domain.Entity.Assessment>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns(Task.CompletedTask);
        _assessmentServiceMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mapperMock
            .Setup(x => x.Map<Question>(It.IsAny<QuestionImportDto>()))
            .Returns(new Question());

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
    }

    [Fact]
    public async Task Handle_ShouldThrowBadRequest_WhenActiveAssessmentExistsForCourse()
    {
        var courseId = Guid.NewGuid();
        var fileMock = new Mock<IFormFile>();
        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("dummy"));
        fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
        fileMock.Setup(f => f.FileName).Returns("questions.xlsx");
        fileMock.Setup(f => f.Length).Returns(5);

        var command = new CreateAssessmentCommand(
            courseId,
            "Test Assessment",
            "Description",
            1,
            60,
            fileMock.Object
        );

        _assessmentServiceMock
            .Setup(x => x.ActiveAssessmentExistsForCourseAsync(courseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await Assert.ThrowsAsync<BadRequestException>(() =>
            _handler.Handle(command, CancellationToken.None)
        );
    }

    [Fact]
    public async Task Handle_ShouldThrowBadRequest_WhenPassingMarkExceedsTotalMark()
    {
        var courseId = Guid.NewGuid();
        var fileMock = new Mock<IFormFile>();
        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("dummy"));
        fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
        fileMock.Setup(f => f.FileName).Returns("questions.xlsx");
        fileMock.Setup(f => f.Length).Returns(5);

        var command = new CreateAssessmentCommand(
            courseId,
            "Test",
            "Desc",
            10,
            60,
            fileMock.Object
        );

        var importedQuestions = new List<QuestionImportDto>
        {
            new()
            {
                QuestionText = "Q1",
                QuestionTypeName = "MCQ",
                MetaTopicName = "T1",
                Option1 = "A",
                Option2 = "B",
                Option3 = "C",
                Option4 = "D",
                Answer = "A",
                Weight = 1,
            },
        };

        _assessmentServiceMock
            .Setup(x => x.ActiveAssessmentExistsForCourseAsync(courseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _questionParserMock
            .Setup(x => x.ParseQuestionAsync(fileMock.Object))
            .ReturnsAsync(importedQuestions);

        await Assert.ThrowsAsync<BadRequestException>(() =>
            _handler.Handle(command, CancellationToken.None)
        );
    }

    [Fact]
    public async Task Handle_ShouldThrowBadRequest_WhenNoQuestionsParsed()
    {
        var fileMock = new Mock<IFormFile>();
        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(""));
        fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
        fileMock.Setup(f => f.FileName).Returns("questions.xlsx");
        fileMock.Setup(f => f.Length).Returns(0);

        var command = new CreateAssessmentCommand(
            Guid.NewGuid(),
            "Test",
            "Desc",
            1,
            60,
            fileMock.Object
        );

        _assessmentServiceMock
            .Setup(x => x.ActiveAssessmentExistsForCourseAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _questionParserMock
            .Setup(x => x.ParseQuestionAsync(fileMock.Object))
            .ReturnsAsync(new List<QuestionImportDto>());

        await Assert.ThrowsAsync<BadRequestException>(() =>
            _handler.Handle(command, CancellationToken.None)
        );
    }

    [Fact]
    public async Task Handle_ShouldThrowBadRequest_WhenMetaTopicNotFound()
    {
        var courseId = Guid.NewGuid();
        var fileMock = new Mock<IFormFile>();
        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("dummy"));
        fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
        fileMock.Setup(f => f.FileName).Returns("questions.xlsx");
        fileMock.Setup(f => f.Length).Returns(5);

        var command = new CreateAssessmentCommand(courseId, "Test", "Desc", 1, 60, fileMock.Object);

        var importedQuestions = new List<QuestionImportDto>
        {
            new()
            {
                QuestionText = "Q1",
                QuestionTypeName = "MCQ",
                MetaTopicName = "NonExistentTopic",
                Option1 = "A",
                Option2 = "B",
                Option3 = "C",
                Option4 = "D",
                Answer = "A",
                Weight = 1,
            },
        };

        _assessmentServiceMock
            .Setup(x => x.ActiveAssessmentExistsForCourseAsync(courseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _questionParserMock
            .Setup(x => x.ParseQuestionAsync(fileMock.Object))
            .ReturnsAsync(importedQuestions);
        _assessmentServiceMock
            .Setup(x => x.GetMetaTopicByCourseIdAsync(courseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CourseMetaTopic>());
        _assessmentServiceMock
            .Setup(x => x.GetQuestionTypeAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new List<RefTerm>
                {
                    new() { Id = Guid.NewGuid(), Name = "MCQ" },
                }
            );
        _assessmentServiceMock
            .Setup(x =>
                x.AddAssessmentAsync(
                    It.IsAny<LAP.Domain.Entity.Assessment>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns(Task.CompletedTask);

        _transactionServiceMock
            .Setup(x =>
                x.ExecuteInTransactionAsync(
                    It.IsAny<Func<Task<Guid>>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns<Func<Task<Guid>>, CancellationToken>((func, ct) => func());

        await Assert.ThrowsAsync<BadRequestException>(() =>
            _handler.Handle(command, CancellationToken.None)
        );
    }
}
