namespace LAP.Domain.Entity;

/// <summary>
/// Stores personal information such as name, email, mobile number, designation, and gender.
/// </summary>
public class Person : BaseEntity
{
    /// <summary>Gets or sets the full name of the person.</summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>Gets or sets the email address of the person.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Gets or sets the mobile number of the person.</summary>
    public string MobileNumber { get; set; } = string.Empty;

    /// <summary>Gets or sets the foreign key to the designation reference term.</summary>
    public Guid DesignationId { get; set; }

    /// <summary>Gets or sets the foreign key to the gender reference term.</summary>
    public Guid GenderId { get; set; }

    /// <summary>Gets or sets the designation reference term.</summary>
    public RefTerm Designation { get; set; } = null!;

    /// <summary>Gets or sets the gender reference term.</summary>
    public RefTerm Gender { get; set; } = null!;

    /// <summary>Gets or sets the associated user account.</summary>
    public User User { get; set; } = null!;
}
