namespace LAP.Application.Options;

public class FileStorageOptions
{
    public const string SectionName = "FileStorageOptions";

    public string StorageRoot { get; set; } = string.Empty;

    public string QuestionTemplatePath { get; set; } = string.Empty;
}
