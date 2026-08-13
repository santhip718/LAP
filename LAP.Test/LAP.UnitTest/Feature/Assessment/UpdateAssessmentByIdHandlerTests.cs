using LAP.Application.DTO.Assessment;
using LAP.Application.Feature.Assessment.Command;
using LAP.Application.Interface;
using LAP.Application.Interface.IService;
using LAP.Application.Mapping;
using LAP.Shared.Exceptions;
using AutoMapper;
using Moq;

namespace LAP.UnitTest.Handler.Assessment;

public class UpdateAssessmentByIdHandlerTest
{
    private readonly Mock<IAssessmentService> _assessmentServiceMock;
    private readonly Mock<ICustomLogger<UpdateAssessmentByIdHandler>> _loggerMock;
    private readonly IMapper _mapper;
    private readonly UpdateAssessmentByIdHandler _handler;

    public UpdateAssessmentByIdHandlerTest()
    {
        _assessmentServiceMock = new Mock<IAssessmentService>();
        _loggerMock = new Mock<ICustomLogger<UpdateAssessmentByIdHandler>>();
        
        var config = new MapperConfiguration(cfg => cfg.AddProfile<AssessmentMappingProfile>());
        _mapper = config.CreateMapper();

        _handler = new UpdateAssessmentByIdHandler(
            _assessmentServiceMock.Object,
            _mapper,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldUpdateAssessment_WhenAssessmentExists()
    {
        var id = Guid.NewGuid();
        var dto = new UpdateAssessmentRequestDto
        {
            Title = "Updated Title",
            Description = "Updated Description",
            TotalMark = 10,
            PassingMark = 5,
            DurationMinute = 60,
        };
        var command = new UpdateAssessmentByIdCommand(id, dto);

        var assessment = new LAP.Domain.Entity.Assessment
        {
            Id = id,
            Title = "Old Title",
            Description = "Old Description",
            TotalMark = 8,
            PassingMark = 4,
            DurationMinute = 30,
        };

        _assessmentServiceMock
            .Setup(x => x.GetAssessmentByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(assessment);
        _assessmentServiceMock
            .Setup(x => x.UpdateAssessmentAsync(assessment, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotEmpty(result.Message);
        Assert.Equal("Updated Title", assessment.Title);
        Assert.Equal("Updated Description", assessment.Description);
        Assert.Equal(10, assessment.TotalMark);
        Assert.Equal(5, assessment.PassingMark);
        Assert.Equal(60, assessment.DurationMinute);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenAssessmentDoesNotExist()
    {
        var id = Guid.NewGuid();
        var command = new UpdateAssessmentByIdCommand(
            id,
            new UpdateAssessmentRequestDto
            {
                Title = "Title",
                TotalMark = 10,
                PassingMark = 5,
                DurationMinute = 60,
            }
        );

        _assessmentServiceMock
            .Setup(x => x.GetAssessmentByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((LAP.Domain.Entity.Assessment?)null);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            _handler.Handle(command, CancellationToken.None)
        );
    }
}
