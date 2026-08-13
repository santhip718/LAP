using AutoMapper;
using LAP.Application.DTO.Assessment;
using LAP.Application.Feature.Assessment.Command;
using LAP.Application.Interface;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using LAP.Shared.Exceptions;
using Moq;

namespace LAP.UnitTest.Handler.Assessment;

public class UpdateQuestionByIdHandlerTest
{
    private readonly Mock<IAssessmentService> _assessmentServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ICustomLogger<UpdateQuestionByIdHandler>> _loggerMock;
    private readonly UpdateQuestionByIdHandler _handler;

    public UpdateQuestionByIdHandlerTest()
    {
        _assessmentServiceMock = new Mock<IAssessmentService>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ICustomLogger<UpdateQuestionByIdHandler>>();
        _handler = new UpdateQuestionByIdHandler(
            _assessmentServiceMock.Object,
            _mapperMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldUpdateQuestion_WhenQuestionExists()
    {
        var id = Guid.NewGuid();
        var metaTopicId = Guid.NewGuid();
        var questionTypeId = Guid.NewGuid();
        var dto = new UpdateQuestionRequestDto
        {
            QuestionText = "Updated Question?",
            OptionList = new List<string> { "A", "B", "C", "D" },
            Answer = "A",
            Weight = 2,
            QuestionTypeId = questionTypeId,
            MetaTopicId = metaTopicId.ToString(),
        };
        var command = new UpdateQuestionByIdCommand(id, dto);

        var question = new Question
        {
            Id = id,
            QuestionText = "Old Question?",
            Answer = "B",
            Weight = 1,
        };

        _assessmentServiceMock
            .Setup(x => x.GetQuestionByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(question);
        _assessmentServiceMock
            .Setup(x => x.UpdateQuestionAsync(question, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mapperMock.Setup(x => x.Map(It.IsAny<UpdateQuestionRequestDto>(), It.IsAny<Question>()))
            .Callback<object, object>((src, dest) =>
            {
                var s = (UpdateQuestionRequestDto)src;
                var d = (Question)dest;
                d.QuestionText = s.QuestionText ?? d.QuestionText;
                d.Answer = s.Answer ?? d.Answer;
                d.Weight = s.Weight ?? d.Weight;
                d.QuestionTypeId = s.QuestionTypeId ?? d.QuestionTypeId;
            });

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotEmpty(result.Message);
        Assert.Equal("Updated Question?", question.QuestionText);
        Assert.Equal("A", question.Answer);
        Assert.Equal(2, question.Weight);
        Assert.Equal(questionTypeId, question.QuestionTypeId);
        Assert.Equal(metaTopicId, question.MetaTopicId);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenQuestionDoesNotExist()
    {
        var id = Guid.NewGuid();
        var command = new UpdateQuestionByIdCommand(
            id,
            new UpdateQuestionRequestDto
            {
                QuestionText = "Q?",
                OptionList = new List<string>(),
                Answer = "A",
                Weight = 1,
                QuestionTypeId = Guid.NewGuid(),
            }
        );

        _assessmentServiceMock
            .Setup(x => x.GetQuestionByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Question?)null);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            _handler.Handle(command, CancellationToken.None)
        );
    }
}
