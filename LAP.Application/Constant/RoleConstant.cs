namespace LAP.Application.Constant;

/// <summary>
/// Defines constant GUID identifiers for system roles used during registration and authorization.
/// </summary>
public static class RoleConstants
{
    /// <summary>The unique identifier for the Administrator role.</summary>
    public static readonly Guid ADMIN_ID = Guid.Parse("c3f5b394-4ef8-4275-a0c6-c3a126393006");

    /// <summary>The role name for administrators used in authorization checks.</summary>
    public const string ADMIN_ROLE_NAME = "Admin";

    /// <summary>The unique identifier for the Student role (default role for new registrations).</summary>
    public static readonly Guid STUDENT_ID = Guid.Parse("366744c0-0ef9-42ed-a0ba-b7e2075a5ad1");
}
