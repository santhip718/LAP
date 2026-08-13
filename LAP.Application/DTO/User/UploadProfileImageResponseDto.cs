namespace LAP.Application.DTO.User;

/// <summary>
/// Represents the response model containing the uploaded profile image path.
/// </summary>
public class UploadProfileImageResponseDto
{
    /// <summary>
    /// Gets or sets the relative path to the uploaded profile image.
    /// </summary>
    public string ProfileImage { get; set; } = null!;
}
