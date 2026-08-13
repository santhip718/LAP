using FluentValidation;
using LAP.Application.Constant;
using LAP.Application.Interface;
using LAP.Application.Interface.IService;
using LAP.Shared.Exceptions;
using MediatR;

namespace LAP.Application.Feature.Assessment.Query;

/// <summary>
/// Query to export the assessment question template.
/// </summary>
public record ExportTemplateQuery()
    : IRequest<(byte[] FileContents, string ContentType, string FileName)>;

/// <summary>
/// Handler for <see cref="ExportTemplateQuery"/>.
/// </summary>
public class ExportTemplateHandler
    : IRequestHandler<
        ExportTemplateQuery,
        (byte[] FileContents, string ContentType, string FileName)
    >
{
    private readonly ICustomLogger<ExportTemplateHandler> _logger;
    private readonly IFileStorageService _fileStorageService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExportTemplateHandler"/> class.
    /// </summary>
    public ExportTemplateHandler(
        ICustomLogger<ExportTemplateHandler> logger,
        IFileStorageService fileStorageService
    )
    {
        _logger = logger;
        _fileStorageService = fileStorageService;
    }

    /// <summary>
    /// Handles the request to export the question template.
    /// </summary>
    public async Task<(byte[] FileContents, string ContentType, string FileName)> Handle(
        ExportTemplateQuery request,
        CancellationToken cancellationToken
    )
    {
        _logger.LogInfo("Export template request processing started");

        string? filePath = await _fileStorageService.GetQuestionTemplateFilePathAsync(
            cancellationToken
        );

        if (filePath is null)
        {
            _logger.LogError("Template file not found at any expected location");
            throw new NotFoundException(
                "Template file not found",
                "The question import template file could not be located."
            );
        }

        byte[] fileBytes = await File.ReadAllBytesAsync(filePath, cancellationToken);

        _logger.LogInfo("ExportTemplate completed successfully");

        return (
            fileBytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            CommonConstants.QuestionTemplateFileName
        );
    }
}
