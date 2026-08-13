using LAP.Application.Interface;
using LAP.Application.Interface.IRepository;
using LAP.Application.Interface.IService;
using LAP.Application.Service;
using LAP.Domain.Entity;
using Moq;
using System.Linq.Expressions;
using MockQueryable.Moq;
using MockQueryable;
using Microsoft.EntityFrameworkCore;

namespace LAP.UnitTest.Services;

public class AssessmentServiceTest
{
    private readonly Mock<IRepositoryWrapper> _repoWrapperMock;
    private readonly Mock<IAssessmentRepository> _assessmentRepoMock;
    private readonly Mock<ICustomLogger<AssessmentService>> _loggerMock;
    private readonly Mock<IBaseRepository<Course>> _courseRepoMock;
    private readonly Mock<IBaseRepository<Question>> _questionRepoMock;
    private readonly Mock<IBaseRepository<CourseMetaTopic>> _metaTopicRepoMock;
    private readonly Mock<IBaseRepository<RefTerm>> _refTermRepoMock;
    private readonly Mock<IAssessmentHistoryRepository> _assessmentHistoryRepoMock;
    private readonly Mock<IBaseRepository<RefSet>> _refSetRepoMock;
    private readonly IAssessmentService _assessmentService;

    public AssessmentServiceTest()
    {
        _repoWrapperMock = new Mock<IRepositoryWrapper>();
        _assessmentRepoMock = new Mock<IAssessmentRepository>();
        _loggerMock = new Mock<ICustomLogger<AssessmentService>>();
        _courseRepoMock = new Mock<IBaseRepository<Course>>();
        _questionRepoMock = new Mock<IBaseRepository<Question>>();
        _metaTopicRepoMock = new Mock<IBaseRepository<CourseMetaTopic>>();
        _refTermRepoMock = new Mock<IBaseRepository<RefTerm>>();
        _assessmentHistoryRepoMock = new Mock<IAssessmentHistoryRepository>();
        _refSetRepoMock = new Mock<IBaseRepository<RefSet>>();

        _repoWrapperMock.Setup(x => x.Assessment).Returns(_assessmentRepoMock.Object);
        _repoWrapperMock.Setup(x => x.Repository<Course>()).Returns(_courseRepoMock.Object);
        _repoWrapperMock.Setup(x => x.Repository<Question>()).Returns(_questionRepoMock.Object);
        _repoWrapperMock.Setup(x => x.Repository<CourseMetaTopic>()).Returns(_metaTopicRepoMock.Object);
        _repoWrapperMock.Setup(x => x.Repository<RefTerm>()).Returns(_refTermRepoMock.Object);
        _repoWrapperMock.Setup(x => x.AssessmentHistory).Returns(_assessmentHistoryRepoMock.Object);
        _repoWrapperMock.Setup(x => x.Repository<RefSet>()).Returns(_refSetRepoMock.Object);

        _assessmentService = new AssessmentService(_repoWrapperMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetMetaTopicByCourseIdAsync_ShouldReturnTopics()
    {
        var courseId = Guid.NewGuid();
        var topics = new List<CourseMetaTopic>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Topic 1",
                CourseId = courseId,
                IsActive = true
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Topic 2",
                CourseId = courseId,
                IsActive = true
            },
        };

        var mock = topics.BuildMock();

        _metaTopicRepoMock
            .Setup(x => x.FindByCondition(It.IsAny<Expression<Func<CourseMetaTopic, bool>>>()))
            .Returns(mock);

        var result = await _assessmentService.GetMetaTopicByCourseIdAsync(courseId);

        Assert.Equal(2, result.Count);
        Assert.Equal("Topic 1", result[0].Name);
    }

    [Fact]
    public async Task GetQuestionTypeAsync_ShouldReturnTypes()
    {
        var types = new List<RefTerm>
        {
            new() { Id = Guid.NewGuid(), Name = "MCQ", IsActive = true, RefSet = new RefSet { Name = "QuestionType" } },
            new() { Id = Guid.NewGuid(), Name = "TrueFalse", IsActive = true, RefSet = new RefSet { Name = "QuestionType" } },
        };
        
        var mock = types.BuildMock();
        _refTermRepoMock.Setup(x => x.FindByCondition(It.IsAny<Expression<Func<RefTerm, bool>>>()))
            .Returns(mock);

        var result = await _assessmentService.GetQuestionTypeAsync();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetAssessmentByIdAsync_ShouldReturnAssessment_WhenFound()
    {
        var id = Guid.NewGuid();
        var assessment = new LAP.Domain.Entity.Assessment { Id = id, Title = "Test Assessment", IsActive = true };
        _assessmentRepoMock
            .Setup(x => x.FindFirstByConditionAsync(It.IsAny<Expression<Func<LAP.Domain.Entity.Assessment, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(assessment);

        var result = await _assessmentService.GetAssessmentByIdAsync(id);

        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
    }

    [Fact]
    public async Task GetAssessmentByIdAsync_ShouldReturnNull_WhenNotFound()
    {
        _assessmentRepoMock
            .Setup(x => x.FindFirstByConditionAsync(It.IsAny<Expression<Func<LAP.Domain.Entity.Assessment, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((LAP.Domain.Entity.Assessment?)null);

        var result = await _assessmentService.GetAssessmentByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task GetQuestionByAssessmentIdAsync_ShouldReturnQuestions()
    {
        var assessmentId = Guid.NewGuid();
        var questions = new List<Question>
        {
            new()
            {
                Id = Guid.NewGuid(),
                QuestionText = "Q1",
                AssessmentId = assessmentId,
                IsActive = true
            },
        };
        
        var mock = questions.BuildMock();
        _questionRepoMock.Setup(x => x.FindByCondition(It.IsAny<Expression<Func<Question, bool>>>()))
            .Returns(mock);

        var result = await _assessmentService.GetQuestionByAssessmentIdAsync(assessmentId);

        Assert.Single(result);
    }

    [Fact]
    public async Task AddAssessmentAsync_ShouldDelegateToRepository()
    {
        var assessment = new LAP.Domain.Entity.Assessment { Title = "New Assessment" };
        _assessmentRepoMock
            .Setup(x => x.CreateAsync(assessment, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await _assessmentService.AddAssessmentAsync(assessment);

        _assessmentRepoMock.Verify(
            x => x.CreateAsync(assessment, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task UpdateAssessmentAsync_ShouldUpdateAndSave()
    {
        var assessment = new LAP.Domain.Entity.Assessment
        {
            Id = Guid.NewGuid(),
            Title = "Updated",
        };
        _assessmentRepoMock.Setup(x => x.Update(assessment));
        _repoWrapperMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _assessmentService.UpdateAssessmentAsync(assessment);

        _assessmentRepoMock.Verify(x => x.Update(assessment), Times.Once);
        _repoWrapperMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddQuestionAsync_ShouldDelegateToRepository()
    {
        var question = new Question { QuestionText = "New Q" };
        _questionRepoMock
            .Setup(x => x.CreateAsync(question, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await _assessmentService.AddQuestionAsync(question);

        _questionRepoMock.Verify(
            x => x.CreateAsync(question, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldDelegateToWrapper()
    {
        _repoWrapperMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _assessmentService.SaveChangesAsync();

        _repoWrapperMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllAssessmentAsync_ShouldReturnAll()
    {
        var assessments = new List<LAP.Domain.Entity.Assessment>
        {
            new() { Id = Guid.NewGuid(), Title = "A1", IsActive = true },
            new() { Id = Guid.NewGuid(), Title = "A2", IsActive = true },
        };
        
        var mock = assessments.BuildMock();
        _assessmentRepoMock.Setup(x => x.FindByCondition(It.IsAny<Expression<Func<LAP.Domain.Entity.Assessment, bool>>>()))
            .Returns(mock);

        var result = await _assessmentService.GetAllAssessmentAsync();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task DeleteAssessmentAsync_ShouldSoftDelete()
    {
        var id = Guid.NewGuid();
        _assessmentRepoMock
            .Setup(x => x.SoftDeleteAsync(It.IsAny<Expression<Func<LAP.Domain.Entity.Assessment, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _assessmentService.DeleteAssessmentAsync(id);

        _assessmentRepoMock.Verify(
            x => x.SoftDeleteAsync(It.IsAny<Expression<Func<LAP.Domain.Entity.Assessment, bool>>>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
        Assert.Equal(1, result);
    }
    
    [Fact]
    public async Task GetQuestionByIdAsync_ShouldReturnQuestion_WhenFound()
    {
        var id = Guid.NewGuid();
        var question = new Question { Id = id, QuestionText = "Test Q", IsActive = true };
        _questionRepoMock
            .Setup(x => x.FindFirstByConditionAsync(It.IsAny<Expression<Func<Question, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(question);

        var result = await _assessmentService.GetQuestionByIdAsync(id);

        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
    }

    [Fact]
    public async Task UpdateQuestionAsync_ShouldUpdateAndSave()
    {
        var question = new Question { Id = Guid.NewGuid(), QuestionText = "Updated Q" };
        _questionRepoMock.Setup(x => x.Update(question));
        _repoWrapperMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _assessmentService.UpdateQuestionAsync(question);

        _questionRepoMock.Verify(x => x.Update(question), Times.Once);
        _repoWrapperMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteQuestionAsync_ShouldSoftDelete()
    {
        var id = Guid.NewGuid();
        _questionRepoMock
            .Setup(x => x.SoftDeleteAsync(It.IsAny<Expression<Func<Question, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _assessmentService.DeleteQuestionAsync(id);

        _questionRepoMock.Verify(
            x => x.SoftDeleteAsync(It.IsAny<Expression<Func<Question, bool>>>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
        Assert.Equal(1, result);
    }

    [Fact]
    public async Task GetByCourseIdAsync_ShouldReturnAssessments()
    {
        var courseId = Guid.NewGuid();
        var assessments = new List<LAP.Domain.Entity.Assessment>
        {
            new() { Id = Guid.NewGuid(), CourseId = courseId, IsActive = true },
        };

        var mock = assessments.BuildMock();

        _assessmentRepoMock
            .Setup(x => x.FindByCondition(It.IsAny<Expression<Func<LAP.Domain.Entity.Assessment, bool>>>()))
            .Returns(mock);

        var result = await _assessmentService.GetByCourseIdAsync(courseId);

        Assert.Single(result);
    }

    [Fact]
    public async Task CourseExistsAsync_ShouldReturnTrue_WhenCourseExists()
    {
        var courseId = Guid.NewGuid();
        _courseRepoMock
            .Setup(x => x.AnyByConditionAsync(It.IsAny<Expression<Func<Course, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _assessmentService.CourseExistsAsync(courseId);

        Assert.True(result);
    }

    [Fact]
    public async Task CourseExistsAsync_ShouldReturnFalse_WhenCourseDoesNotExist()
    {
        _courseRepoMock
            .Setup(x => x.AnyByConditionAsync(It.IsAny<Expression<Func<Course, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _assessmentService.CourseExistsAsync(Guid.NewGuid());

        Assert.False(result);
    }

    [Fact]
    public async Task GetUserAllCompletedAssessmentHistoryAsync_ShouldReturnCompletedHistories()
    {
        Guid userId = Guid.NewGuid();
        List<AssessmentHistory> histories = new List<AssessmentHistory>
        {
            new AssessmentHistory { Id = Guid.NewGuid(), UserId = userId, CompletedOn = DateTime.UtcNow, IsActive = true }
        };

        var mock = histories.BuildMock();

        _assessmentHistoryRepoMock
            .Setup(x => x.FindByCondition(It.IsAny<Expression<Func<AssessmentHistory, bool>>>()))
            .Returns(mock);

        IEnumerable<AssessmentHistory> result = await _assessmentService.GetUserAllCompletedAssessmentHistoryAsync(userId);

        Assert.Single(result);
        Assert.Equal(userId, result.First().UserId);
    }

    [Fact]
    public async Task GetTierAsync_ShouldReturnActiveTiers()
    {
        RefSet refSet = new RefSet { Id = Guid.NewGuid(), Name = "Tier", IsActive = true };
        List<RefTerm> refTerms = new List<RefTerm>
        {
            new RefTerm { Id = Guid.NewGuid(), RefSetId = refSet.Id, IsActive = true }
        };

        var mock = refTerms.BuildMock();

        _refSetRepoMock
            .Setup(x => x.FindFirstByConditionAsync(It.IsAny<Expression<Func<RefSet, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(refSet);

        _refTermRepoMock
            .Setup(x => x.FindByCondition(It.IsAny<Expression<Func<RefTerm, bool>>>()))
            .Returns(mock);

        IEnumerable<RefTerm> result = await _assessmentService.GetTierAsync();

        Assert.Single(result);
        Assert.Equal(refSet.Id, result.First().RefSetId);
    }
}
