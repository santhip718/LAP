namespace LAP.Test.IntegrationTest;

/// <summary>
/// Contains all predefined GUID constants for seed data entities used across integration tests.
/// </summary>
public static class TestSeedIds
{
    // ── RefSets ──────────────────────────────────────────────────────────
    public static readonly Guid RoleRefSetId = Guid.Parse("10000000-0000-0000-0000-000000000001");
    public static readonly Guid GenderRefSetId = Guid.Parse("10000000-0000-0000-0000-000000000002");
    public static readonly Guid QuestionTypeRefSetId = Guid.Parse("10000000-0000-0000-0000-000000000003");
    public static readonly Guid DifficultyLevelRefSetId = Guid.Parse("10000000-0000-0000-0000-000000000004");
    public static readonly Guid TierRefSetId = Guid.Parse("10000000-0000-0000-0000-000000000005");
    public static readonly Guid ContentTypeRefSetId = Guid.Parse("10000000-0000-0000-0000-000000000006");
    public static readonly Guid ImportStatusRefSetId = Guid.Parse("10000000-0000-0000-0000-000000000007");
    public static readonly Guid DesignationRefSetId = Guid.Parse("10000000-0000-0000-0000-000000000008");
    public static readonly Guid CategoryRefSetId = Guid.Parse("10000000-0000-0000-0000-000000000009");

    // ── RefTerms ─────────────────────────────────────────────────────────
    // Admin and Student roles from RoleConstants (matching seeds)
    public static readonly Guid AdminRoleId = Guid.Parse("20000000-0000-0000-0000-000000000001");
    public static readonly Guid StudentRoleId = Guid.Parse("20000000-0000-0000-0000-000000000002");

    public static readonly Guid MaleGenderId = Guid.Parse("20000000-0000-0000-0000-000000000010");
    public static readonly Guid FemaleGenderId = Guid.Parse("20000000-0000-0000-0000-000000000011");
    public static readonly Guid OtherGenderId = Guid.Parse("20000000-0000-0000-0000-000000000012");

    public static readonly Guid EasyDifficultyId = Guid.Parse("20000000-0000-0000-0000-000000000020");
    public static readonly Guid MediumDifficultyId = Guid.Parse("20000000-0000-0000-0000-000000000021");
    public static readonly Guid HardDifficultyId = Guid.Parse("20000000-0000-0000-0000-000000000022");

    public static readonly Guid CodeCadetTierId = Guid.Parse("20000000-0000-0000-0000-000000000030");
    public static readonly Guid SyntaxVoyagerTierId = Guid.Parse("20000000-0000-0000-0000-000000000031");

    public static readonly Guid JuniorDeveloperDesignationId = Guid.Parse("20000000-0000-0000-0000-000000000040");
    public static readonly Guid SeniorDeveloperDesignationId = Guid.Parse("20000000-0000-0000-0000-000000000041");

    public static readonly Guid TechnologyCategoryId = Guid.Parse("20000000-0000-0000-0000-000000000050");
    public static readonly Guid BusinessCategoryId = Guid.Parse("20000000-0000-0000-0000-000000000051");

    public static readonly Guid ProgrammingSubCategoryId = Guid.Parse("20000000-0000-0000-0000-000000000060");
    public static readonly Guid ManagementSubCategoryId = Guid.Parse("20000000-0000-0000-0000-000000000061");

    public static readonly Guid VideoContentTypeId = Guid.Parse("20000000-0000-0000-0000-000000000070");
    public static readonly Guid PdfContentTypeId = Guid.Parse("20000000-0000-0000-0000-000000000071");

    // ── Features ─────────────────────────────────────────────────────────
    public static readonly Guid ViewUsersFeatureId = Guid.Parse("30000000-0000-0000-0000-000000000001");
    public static readonly Guid ManageUsersFeatureId = Guid.Parse("30000000-0000-0000-0000-000000000002");
    public static readonly Guid ManageCoursesFeatureId = Guid.Parse("30000000-0000-0000-0000-000000000003");
    public static readonly Guid ViewCourseAdministrationFeatureId = Guid.Parse("30000000-0000-0000-0000-000000000004");
    public static readonly Guid ViewForumFeatureId = Guid.Parse("30000000-0000-0000-0000-000000000005");
    public static readonly Guid ParticipateForumFeatureId = Guid.Parse("30000000-0000-0000-0000-000000000006");
    public static readonly Guid RequestEnrollmentsFeatureId = Guid.Parse("30000000-0000-0000-0000-000000000007");
    public static readonly Guid ManageCourseContentFeatureId = Guid.Parse("30000000-0000-0000-0000-000000000008");
    public static readonly Guid ManageEnrollmentsFeatureId = Guid.Parse("30000000-0000-0000-0000-000000000009");

    // ── Persons ──────────────────────────────────────────────────────────
    public static readonly Guid SeedUserPersonId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
    public static readonly Guid MutateUserPersonId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb002");
    public static readonly Guid DeleteUserPersonId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb003");

    // ── Users ────────────────────────────────────────────────────────────
    public static readonly Guid SeedUserId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    public static readonly Guid MutateUserId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaa002");
    public static readonly Guid DeleteUserId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaa003");

    // ── Courses ──────────────────────────────────────────────────────────
    public static readonly Guid CourseId_1 = Guid.Parse("c0000000-0000-0000-0000-000000000001");
    public static readonly Guid CourseId_2 = Guid.Parse("c0000000-0000-0000-0000-000000000002");
    public static readonly Guid DeleteCourseId = Guid.Parse("c0000000-0000-0000-0000-000000000003");

    // ── CourseMetaTopics ─────────────────────────────────────────────────
    public static readonly Guid MetaTopicId_1 = Guid.Parse("d0000000-0000-0000-0000-000000000001");
    public static readonly Guid DeleteMetaTopicId = Guid.Parse("d0000000-0000-0000-0000-000000000002");

    // ── CourseContents ───────────────────────────────────────────────────
    public static readonly Guid CourseContentId_1 = Guid.Parse("e0000000-0000-0000-0000-000000000001");
    public static readonly Guid DeleteCourseContentId = Guid.Parse("e0000000-0000-0000-0000-000000000002");

    // ── Enrollments ──────────────────────────────────────────────────────
    public static readonly Guid EnrollmentId_1 = Guid.Parse("f0000000-0000-0000-0000-000000000001");

    // ── RefTerm Aliases (used by Controllers/ tests) ─────────────────────
    public static readonly Guid RefTerm_Technology = TechnologyCategoryId;
    public static readonly Guid RefTerm_Programming = ProgrammingSubCategoryId;
    public static readonly Guid RefTerm_Easy = EasyDifficultyId;
    public static readonly Guid RefTerm_Video = VideoContentTypeId;
    public static readonly Guid RefTerm_Pdf = PdfContentTypeId;
    public static readonly Guid RefTerm_SeniorDeveloper = SeniorDeveloperDesignationId;
    public static readonly Guid RefTerm_JuniorDeveloper = JuniorDeveloperDesignationId;
    public static readonly Guid RefTerm_Male = MaleGenderId;

    // Non-existent ID for 404 tests
    public static readonly Guid NonExistentId = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");
}
