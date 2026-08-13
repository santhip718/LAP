using LAP.Application.DTO.Assessment;
using Microsoft.AspNetCore.Http;

namespace LAP.Application.Interface.IHelper;

/// <summary>
/// Interface for parsing questions from an uploaded file.
/// </summary>
public interface IQuestionParser
{
    /// <summary>
    /// Parses questions from the provided file.
    /// </summary>
    /// <param name="file">The uploaded question file.</param>
    /// <returns>A list of imported question DTOs.</returns>
    Task<List<QuestionImportDto>> ParseQuestionAsync(IFormFile file);
}
