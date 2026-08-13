using LAP.Application.DTO.Common;
using LAP.Application.Feature.Course.Query;
using LAP.Application.Interface;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using Moq;

namespace LAP.UnitTest.Feature.Course;

public class GetActiveCategoryHandlerTest
{
    private readonly Mock<ICourseService> _courseServiceMock;
    private readonly Mock<ICustomLogger<GetActiveCategoryHandler>> _loggerMock;
    private readonly GetActiveCategoryHandler _handler;

    public GetActiveCategoryHandlerTest()
    {
        _courseServiceMock = new Mock<ICourseService>();
        _loggerMock = new Mock<ICustomLogger<GetActiveCategoryHandler>>();
        _handler = new GetActiveCategoryHandler(
            _courseServiceMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldReturnCategories_WhenCoursesExist()
    {
        var categories = new List<RefTerm>
        {
            new() { Id = Guid.NewGuid(), Name = "Web Development" },
            new() { Id = Guid.NewGuid(), Name = "Data Science" },
        };

        _courseServiceMock
            .Setup(x => x.GetActiveCategoryAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(categories);

        var result = await _handler.Handle(new GetActiveCategoryQuery(), CancellationToken.None);

        Assert.Equal(2, result.Count);
        Assert.Equal(categories[0].Id, result[0].Id);
        Assert.Equal(categories[0].Name, result[0].Name);
        Assert.Equal(categories[1].Id, result[1].Id);
        Assert.Equal(categories[1].Name, result[1].Name);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoCoursesExist()
    {
        _courseServiceMock
            .Setup(x => x.GetActiveCategoryAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<RefTerm>());

        var result = await _handler.Handle(new GetActiveCategoryQuery(), CancellationToken.None);

        Assert.Empty(result);
    }
}
