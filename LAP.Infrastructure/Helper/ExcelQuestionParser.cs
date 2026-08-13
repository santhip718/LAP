using LAP.Application.DTO.Assessment;
using LAP.Application.Interface;
using LAP.Application.Interface.IHelper;
using LAP.Shared.Exceptions;
using Microsoft.AspNetCore.Http;
using MiniExcelLibs;

namespace LAP.Infrastructure.Helper;

/// <summary>
/// Implementation of <see cref="IQuestionParser"/> for Excel files using MiniExcel.
/// </summary>
public class ExcelQuestionParser : IQuestionParser
{
    private readonly ICustomLogger<ExcelQuestionParser> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExcelQuestionParser"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    public ExcelQuestionParser(ICustomLogger<ExcelQuestionParser> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Parses question data from the uploaded Excel file.
    /// </summary>
    /// <param name="file">The Excel file containing question data.</param>
    /// <returns>A list of parsed question import DTOs.</returns>
    /// <exception cref="BadRequestException">
    /// Thrown when the file is empty, has an invalid format, contains no data rows,
    /// or is missing required columns.
    /// </exception>
    public async Task<List<QuestionImportDto>> ParseQuestionAsync(IFormFile file)
    {
        _logger.LogInfo("Starting file parsing for {FileName}", file.FileName);

        if (file == null || file.Length == 0)
        {
            _logger.LogError("File is null or empty");
            throw new BadRequestException("Invalid file", "The uploaded file is empty.");
        }

        string extension = Path.GetExtension(file.FileName).ToLower();
        if (extension != ".xlsx" && extension != ".xls" && extension != ".xlsb")
        {
            _logger.LogError("Invalid file format: {Extension}", extension);
            throw new BadRequestException(
                "Invalid file format",
                "Only Excel files (.xlsx, .xls, .xlsb) are supported."
            );
        }

        using (MemoryStream stream = new MemoryStream())
        {
            await file.CopyToAsync(stream);
            stream.Position = 0;

            // MiniExcel Query<T> will map headers to property names.
            // We should check if the first row contains our expected headers.
            // Alternatively, we can just check the results.

            List<QuestionImportDto> rows = stream.Query<QuestionImportDto>().ToList();

            if (rows == null || !rows.Any())
            {
                _logger.LogError("No data rows found in the Excel file");
                throw new BadRequestException(
                    "Invalid file",
                    "The uploaded Excel file contains no data rows."
                );
            }

            // Basic header presence check via first row data
            QuestionImportDto firstRow = rows[0];
            if (
                string.IsNullOrWhiteSpace(firstRow.QuestionText)
                && string.IsNullOrWhiteSpace(firstRow.QuestionTypeName)
                && string.IsNullOrWhiteSpace(firstRow.MetaTopicName)
            )
            {
                _logger.LogError("Missing required columns or empty first row");
                throw new BadRequestException(
                    "Invalid format",
                    "The Excel file is missing required columns or has an invalid header format."
                );
            }

            _logger.LogInfo("Successfully parsed {Count} rows", rows.Count);
            return rows;
        }
    }
}
