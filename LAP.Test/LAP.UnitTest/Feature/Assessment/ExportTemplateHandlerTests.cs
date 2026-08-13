using LAP.Application.Constant;
using LAP.Application.Feature.Assessment.Query;
using LAP.Application.Interface;
using LAP.Application.Interface.IService;
using Moq;

namespace LAP.UnitTest.Handler.Assessment;

public class ExportTemplateHandlerTest
{
    private readonly Mock<ICustomLogger<ExportTemplateHandler>> _loggerMock;
    private readonly Mock<IFileStorageService> _fileStorageServiceMock;
    private readonly ExportTemplateHandler _handler;

    public ExportTemplateHandlerTest()
    {
        _loggerMock = new Mock<ICustomLogger<ExportTemplateHandler>>();
        _fileStorageServiceMock = new Mock<IFileStorageService>();
        _handler = new ExportTemplateHandler(_loggerMock.Object, _fileStorageServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFileContents_WhenTemplateFileExists()
    {
        string testDir = Path.Combine(
            Path.GetTempPath(),
            "LAP.UnitTest",
            Guid.NewGuid().ToString("N")
        );
        Directory.CreateDirectory(testDir);
        string testFilePath = Path.Combine(testDir, CommonConstants.QuestionTemplateFileName);
        await File.WriteAllBytesAsync(testFilePath, [0x00]);

        _fileStorageServiceMock
            .Setup(x => x.GetQuestionTemplateFilePathAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(testFilePath);

        var (fileContents, contentType, fileName) = await _handler.Handle(
            new ExportTemplateQuery(),
            CancellationToken.None
        );

        Assert.NotNull(fileContents);
        Assert.NotEmpty(fileContents);
        Assert.Equal(
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            contentType
        );
        Assert.Equal(CommonConstants.QuestionTemplateFileName, fileName);

        // Cleanup
        Directory.Delete(testDir, true);
    }
}
