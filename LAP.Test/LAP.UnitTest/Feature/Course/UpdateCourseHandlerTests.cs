using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using LAP.Application.DTO.Common;
using LAP.Application.DTO.Course;
using LAP.Application.Feature.Course.Command;
using LAP.Application.Interface;
using LAP.Application.Interface.IContext;
using LAP.Application.Interface.IService;
using LAP.Domain.Entity;
using LAP.Shared.Exceptions;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace LAP.UnitTest.Feature.Course;

public class UpdateCourseHandlerTests
{
    private readonly Mock<ICourseService> _courseServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ITransactionService> _transactionServiceMock;
    private readonly Mock<ICustomLogger<UpdateCourseHandler>> _loggerMock;
    private readonly Mock<IRequestContext> _requestContextMock;
    private readonly Mock<IFileService> _fileServiceMock;
    private readonly UpdateCourseHandler _handler;

    public UpdateCourseHandlerTests()
    {
        _courseServiceMock = new Mock<ICourseService>();
        _mapperMock = new Mock<IMapper>();
        _transactionServiceMock = new Mock<ITransactionService>();
        _loggerMock = new Mock<ICustomLogger<UpdateCourseHandler>>();
        _requestContextMock = new Mock<IRequestContext>();
        _fileServiceMock = new Mock<IFileService>();

        _handler = new UpdateCourseHandler(
            _courseServiceMock.Object,
            _mapperMock.Object,
            _transactionServiceMock.Object,
            _loggerMock.Object,
            _requestContextMock.Object,
            _fileServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingCourse_UpdatesAndReturnsSuccess()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var dto = new UpdateCourseRequestDto
        {
            Title = "Updated Title",
            Description = "Updated Desc",
            CategoryId = Guid.NewGuid(),
            DifficultyLevelId = Guid.NewGuid(),
            DurationMinute = 60
        };
        var course = new LAP.Domain.Entity.Course { Id = courseId, Title = "Old Title" };
        _courseServiceMock.Setup(s => s.GetCourseByIdAsync(courseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);
        _requestContextMock.Setup(r => r.UserId).Returns(Guid.NewGuid());
        _mapperMock.Setup(m => m.Map(dto, course)).Callback(() =>
        {
            if (dto.Title is not null) course.Title = dto.Title;
            if (dto.Description is not null) course.Description = dto.Description;
            if (dto.CategoryId.HasValue) course.CategoryId = dto.CategoryId.Value;
            if (dto.SubCategoryId.HasValue) course.SubCategoryId = dto.SubCategoryId.Value;
            if (dto.DifficultyLevelId.HasValue) course.DifficultyLevelId = dto.DifficultyLevelId.Value;
            if (dto.DurationMinute.HasValue) course.DurationMinute = dto.DurationMinute.Value;
            if (dto.IsDrafted.HasValue) course.IsDrafted = dto.IsDrafted.Value;
        });

        // Act
        var result = await _handler.Handle(new UpdateCourseCommand(courseId, dto), CancellationToken.None);

        // Assert
        Assert.Equal(courseId, result.Id);
        Assert.Equal(dto.Title, course.Title);
        _courseServiceMock.Verify(s => s.UpdateCourse(course), Times.Once);
        _transactionServiceMock.Verify(s => s.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistingCourse_ThrowsNotFoundException()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var dto = new UpdateCourseRequestDto { Title = "Title" };
        _courseServiceMock.Setup(s => s.GetCourseByIdAsync(courseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((LAP.Domain.Entity.Course?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(new UpdateCourseCommand(courseId, dto), CancellationToken.None));
    }
}
