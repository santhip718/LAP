using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using LAP.Domain.Entity;
using LAP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LAP.Test.IntegrationTest;

/// <summary>
/// Seeds the InMemory database with reference and test data from CSV files.
/// Uses CsvHelper to parse CSV files located in the TestData directory.
/// </summary>
public static class TestDataSeederExtensions
{
    public static void SeedDatabase(this CustomWebApplicationFactory factory)
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<LearningAssessmentDbContext>();
        TestDataSeeder.Seed(db);
    }
}

public static class TestDataSeeder
{
    private static readonly CsvConfiguration CsvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
    {
        HeaderValidated = null,
        MissingFieldFound = null,
    };

    private static readonly string TestDataPath = Path.Combine(AppContext.BaseDirectory, "TestData");

    public static void Seed(LearningAssessmentDbContext db)
    {
        if (db.RefSet.Any())
            return;

        // 1. RefSets
        var refSets = ReadCsv<RefSetRecord>("ref_sets_test.csv")
            .Select(r => new RefSet
            {
                Id = Guid.Parse(r.Id),
                Name = r.Name,
                Description = r.Description,
                DateCreated = DateTime.Parse(r.DateCreated, null, DateTimeStyles.RoundtripKind),
            })
            .ToList();
        db.RefSet.AddRange(refSets);

        // 2. RefTerms
        var refTerms = ReadCsv<RefTermRecord>("ref_terms_test.csv")
            .Select(r => new RefTerm
            {
                Id = Guid.Parse(r.Id),
                RefSetId = Guid.Parse(r.RefSetId),
                Name = r.Name,
                Description = r.Description,
                DateCreated = DateTime.Parse(r.DateCreated, null, DateTimeStyles.RoundtripKind),
            })
            .ToList();
        db.RefTerm.AddRange(refTerms);

        // 3. Features
        var features = ReadCsv<FeatureRecord>("features_test.csv")
            .Select(r => new Feature
            {
                Id = Guid.Parse(r.Id),
                Name = r.Name,
                Method = r.Method,
                Description = r.Description,
                DateCreated = DateTime.Parse(r.DateCreated, null, DateTimeStyles.RoundtripKind),
            })
            .ToList();
        db.Feature.AddRange(features);

        // 4. RoleFeatureMappings
        var roleFeatureMappings = ReadCsv<RoleFeatureMappingRecord>("role_feature_mappings_test.csv")
            .Select(r => new RoleFeatureMapping
            {
                Id = Guid.NewGuid(),
                RoleId = Guid.Parse(r.RoleId),
                FeatureId = Guid.Parse(r.FeatureId),
                DateCreated = DateTime.UtcNow,
            })
            .ToList();
        db.RoleFeatureMapping.AddRange(roleFeatureMappings);

        // 5. Persons
        var persons = ReadCsv<PersonRecord>("persons_test.csv")
            .Select(r => new Person
            {
                Id = Guid.Parse(r.Id),
                FullName = r.FullName,
                Email = r.Email,
                MobileNumber = r.MobileNumber,
                DesignationId = Guid.Parse(r.DesignationId),
                GenderId = Guid.Parse(r.GenderId),
                DateCreated = DateTime.Parse(r.DateCreated, null, DateTimeStyles.RoundtripKind),
            })
            .ToList();
        db.Person.AddRange(persons);

        // 6. Users
        var users = ReadCsv<UserRecord>("users_test.csv")
            .Select(r => new User
            {
                Id = Guid.Parse(r.Id),
                PersonId = Guid.Parse(r.PersonId),
                CurrentTierId = string.IsNullOrWhiteSpace(r.CurrentTierId) ? null : Guid.Parse(r.CurrentTierId),
                DateCreated = DateTime.Parse(r.DateCreated, null, DateTimeStyles.RoundtripKind),
            })
            .ToList();
        db.User.AddRange(users);

        // 7. UserSecrets
        var userSecrets = ReadCsv<UserSecretRecord>("user_secrets_test.csv")
            .Select(r => new UserSecret
            {
                Id = Guid.NewGuid(),
                UserId = Guid.Parse(r.UserId),
                PasswordHash = r.PasswordHash,
                PasswordSalt = r.PasswordSalt,
                DateCreated = DateTime.Parse(r.DateCreated, null, DateTimeStyles.RoundtripKind),
            })
            .ToList();
        db.UserSecret.AddRange(userSecrets);

        // 8. UserRoleMappings
        var userRoleMappings = ReadCsv<UserRoleMappingRecord>("user_role_mappings_test.csv")
            .Select(r => new UserRoleMapping
            {
                Id = Guid.NewGuid(),
                UserId = Guid.Parse(r.UserId),
                RoleId = Guid.Parse(r.RoleId),
                DateCreated = DateTime.Parse(r.DateCreated, null, DateTimeStyles.RoundtripKind),
            })
            .ToList();
        db.UserRoleMapping.AddRange(userRoleMappings);

        // 9. Courses
        var courses = ReadCsv<CourseRecord>("courses_test.csv")
            .Select(r => new Course
            {
                Id = Guid.Parse(r.Id),
                Title = r.Title,
                Description = r.Description,
                CategoryId = Guid.Parse(r.CategoryId),
                SubCategoryId = Guid.Parse(r.SubCategoryId),
                DifficultyLevelId = Guid.Parse(r.DifficultyLevelId),
                CreatedByUserId = Guid.Parse(r.CreatedByUserId),
                DurationMinute = int.Parse(r.DurationMinute, CultureInfo.InvariantCulture),
                IsDrafted = bool.Parse(r.IsDrafted),
                OverallRating = decimal.Parse(r.OverallRating, CultureInfo.InvariantCulture),
                DateCreated = DateTime.Parse(r.DateCreated, null, DateTimeStyles.RoundtripKind),
                IsActive = true
            })
            .ToList();
        db.Course.AddRange(courses);

        // 10. CourseMetaTopics
        var courseMetaTopics = ReadCsv<CourseMetaTopicRecord>("course_meta_topics_test.csv")
            .Select(r => new CourseMetaTopic
            {
                Id = Guid.Parse(r.Id),
                CourseId = Guid.Parse(r.CourseId),
                Name = r.Name,
                SequenceOrder = int.Parse(r.SequenceOrder, CultureInfo.InvariantCulture),
                DurationMinute = int.Parse(r.DurationMinute, CultureInfo.InvariantCulture),
                DateCreated = DateTime.Parse(r.DateCreated, null, DateTimeStyles.RoundtripKind),
            })
            .ToList();
        db.CourseMetaTopic.AddRange(courseMetaTopics);

        // 11. CourseContents
        var courseContents = ReadCsv<CourseContentRecord>("course_contents_test.csv")
            .Select(r => new CourseContent
            {
                Id = Guid.Parse(r.Id),
                MetaTopicId = Guid.Parse(r.MetaTopicId),
                Title = r.Title,
                ContentTypeId = Guid.Parse(r.ContentTypeId),
                VideoUrl = string.IsNullOrWhiteSpace(r.VideoUrl) ? null : r.VideoUrl,
                SequenceOrder = int.Parse(r.SequenceOrder, CultureInfo.InvariantCulture),
                DateCreated = DateTime.Parse(r.DateCreated, null, DateTimeStyles.RoundtripKind),
            })
            .ToList();
        db.CourseContent.AddRange(courseContents);

        // 12. Enrollments
        var enrollments = ReadCsv<EnrollmentRecord>("enrollments_test.csv")
            .Select(r => new Enrollment
            {
                Id = Guid.Parse(r.Id),
                UserId = Guid.Parse(r.UserId),
                CourseId = Guid.Parse(r.CourseId),
                EnrolledOn = DateTime.Parse(r.EnrolledOn, null, DateTimeStyles.RoundtripKind),
                EnrollmentStatus = bool.Parse(r.EnrollmentStatus),
                ProgressPercentage = decimal.Parse(r.ProgressPercentage, CultureInfo.InvariantCulture),
                DateCreated = DateTime.Parse(r.DateCreated, null, DateTimeStyles.RoundtripKind),
            })
            .ToList();
        db.Enrollment.AddRange(enrollments);

        // Set IsActive = true for all newly added entities
        // foreach (var entry in db.ChangeTracker.Entries<BaseEntity>())
        // {
        //     if (entry.State == EntityState.Added)
        //         entry.Entity.IsActive = true;
        // }

        db.SaveChanges();
    }

    private static List<T> ReadCsv<T>(string fileName)
    {
        var path = Path.Combine(TestDataPath, fileName);
        using var reader = new StreamReader(path);
        using var csv = new CsvReader(reader, CsvConfig);
        return csv.GetRecords<T>().ToList();
    }

    // ── CSV Record Classes ───────────────────────────────────────────────

    private sealed class RefSetRecord
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string DateCreated { get; set; } = string.Empty;
    }

    private sealed class RefTermRecord
    {
        public string Id { get; set; } = string.Empty;
        public string RefSetId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string DateCreated { get; set; } = string.Empty;
    }

    private sealed class FeatureRecord
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Method { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string DateCreated { get; set; } = string.Empty;
    }

    private sealed class RoleFeatureMappingRecord
    {
        public string RoleId { get; set; } = string.Empty;
        public string FeatureId { get; set; } = string.Empty;
    }

    private sealed class PersonRecord
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        public string DesignationId { get; set; } = string.Empty;
        public string GenderId { get; set; } = string.Empty;
        public string DateCreated { get; set; } = string.Empty;
    }

    private sealed class UserRecord
    {
        public string Id { get; set; } = string.Empty;
        public string PersonId { get; set; } = string.Empty;
        public string? CurrentTierId { get; set; }
        public string DateCreated { get; set; } = string.Empty;
    }

    private sealed class UserSecretRecord
    {
        public string UserId { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string PasswordSalt { get; set; } = string.Empty;
        public string DateCreated { get; set; } = string.Empty;
    }

    private sealed class UserRoleMappingRecord
    {
        public string UserId { get; set; } = string.Empty;
        public string RoleId { get; set; } = string.Empty;
        public string DateCreated { get; set; } = string.Empty;
    }

    private sealed class CourseRecord
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string CategoryId { get; set; } = string.Empty;
        public string SubCategoryId { get; set; } = string.Empty;
        public string DifficultyLevelId { get; set; } = string.Empty;
        public string CreatedByUserId { get; set; } = string.Empty;
        public string DurationMinute { get; set; } = string.Empty;
        public string IsDrafted { get; set; } = string.Empty;
        public string OverallRating { get; set; } = string.Empty;
        public string DateCreated { get; set; } = string.Empty;
    }

    private sealed class CourseMetaTopicRecord
    {
        public string Id { get; set; } = string.Empty;
        public string CourseId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string SequenceOrder { get; set; } = string.Empty;
        public string DurationMinute { get; set; } = string.Empty;
        public string DateCreated { get; set; } = string.Empty;
    }

    private sealed class CourseContentRecord
    {
        public string Id { get; set; } = string.Empty;
        public string MetaTopicId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string ContentTypeId { get; set; } = string.Empty;
        public string? VideoUrl { get; set; }
        public string SequenceOrder { get; set; } = string.Empty;
        public string DateCreated { get; set; } = string.Empty;
    }

    private sealed class EnrollmentRecord
    {
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string CourseId { get; set; } = string.Empty;
        public string EnrolledOn { get; set; } = string.Empty;
        public string EnrollmentStatus { get; set; } = string.Empty;
        public string ProgressPercentage { get; set; } = string.Empty;
        public string DateCreated { get; set; } = string.Empty;
    }
}
