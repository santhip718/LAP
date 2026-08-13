using System.Globalization;
using System.Text.Json;
using CsvHelper;
using CsvHelper.Configuration;
using LAP.Domain.Entity;
using LAP.Infrastructure.Persistence;

namespace LAP.Test.IntegrationTest;

public class CsvTestDataSeeder
{
    private readonly LearningAssessmentDbContext _db;
    private readonly string _dataDir;

    public CsvTestDataSeeder(LearningAssessmentDbContext db, string dataDir)
    {
        _db = db;
        _dataDir = dataDir;
    }

    public void Seed()
    {
        SeedRefSets();
        SeedRefTerms();
        SeedFeatures();
        SeedRoleFeatureMappings();
        SeedUsers();
        SeedCourses();
        SeedMetaTopics();
        SeedAssessments();
        SeedQuestions();
        SeedEnrollments();
        SeedAssessmentHistory();
    }

    private string GetPersistenceSeedDataDir()
    {
        string dir = Path.Combine(AppContext.BaseDirectory, "Persistence", "SeedData");
        if (Directory.Exists(dir))
            return dir;

        string? solutionDir = Path.GetDirectoryName(
            Path.GetDirectoryName(
                Path.GetDirectoryName(
                    Path.GetDirectoryName(AppContext.BaseDirectory))));
        if (solutionDir != null)
        {
            dir = Path.Combine(
                solutionDir, "LAP.Infrastructure", "Persistence", "SeedData");
            if (Directory.Exists(dir))
                return dir;
        }

        return Path.Combine(AppContext.BaseDirectory, "Persistence", "SeedData");
    }

    private void SeedFeatures()
    {
        string dir = GetPersistenceSeedDataDir();
        string filePath = Path.Combine(dir, "Features.csv");
        if (!File.Exists(filePath))
            return;

        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, GetConfig());
        var records = csv.GetRecords<FeatureCsvRow>().ToList();

        foreach (var r in records)
        {
            if (_db.Feature.Any(x => x.Name == r.Name && x.Method == r.Method))
                continue;

            _db.Feature.Add(new Feature
            {
                Name = r.Name,
                Method = r.Method,
                Description = r.Description,
                IsActive = true,
            });
        }
        _db.SaveChanges();
    }

    private void SeedRoleFeatureMappings()
    {
        string dir = GetPersistenceSeedDataDir();
        string filePath = Path.Combine(dir, "RoleFeatureMappings.csv");
        if (!File.Exists(filePath))
            return;

        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, GetConfig());
        var records = csv.GetRecords<RoleFeatureMappingCsvRow>().ToList();

        var roleRefSet = _db.RefSet.FirstOrDefault(x => x.Name == "Role");
        if (roleRefSet == null)
            return;

        foreach (var r in records)
        {
            var role = _db.RefTerm.FirstOrDefault(x =>
                x.RefSetId == roleRefSet.Id && x.Name == r.Role);
            if (role == null)
                continue;

            var features = _db.Feature.Where(f => f.Name == r.FeatureName).ToList();
            foreach (var feature in features)
            {
                if (_db.RoleFeatureMapping.Any(x =>
                        x.RoleId == role.Id && x.FeatureId == feature.Id))
                    continue;

                _db.RoleFeatureMapping.Add(new RoleFeatureMapping
                {
                    RoleId = role.Id,
                    FeatureId = feature.Id,
                    IsActive = true,
                });
            }
        }
        _db.SaveChanges();
    }

    private CsvConfiguration GetConfig() =>
        new(CultureInfo.InvariantCulture)
        {
            HeaderValidated = null,
            MissingFieldFound = null,
            TrimOptions = TrimOptions.Trim,
        };

    private List<T> ReadCsv<T>(string fileName)
    {
        string filePath = Path.Combine(_dataDir, fileName);
        if (!File.Exists(filePath))
            return new List<T>();

        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, GetConfig());
        return csv.GetRecords<T>().ToList();
    }

    private void SeedRefSets()
    {
        var records = ReadCsv<RefSetCsvRow>("Seed_RefSets.csv");
        foreach (var r in records)
        {
            if (_db.RefSet.Any(x => x.Id == r.Id))
                continue;
            _db.RefSet.Add(
                new RefSet
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    DateCreated = DateTime.UtcNow,
                    IsActive = true,
                }
            );
        }
        _db.SaveChanges();
    }

    private void SeedRefTerms()
    {
        var records = ReadCsv<RefTermCsvRow>("Seed_RefTerms.csv");
        foreach (var r in records)
        {
            if (_db.RefTerm.Any(x => x.Id == r.Id))
                continue;
            _db.RefTerm.Add(
                new RefTerm
                {
                    Id = r.Id,
                    RefSetId = r.RefSetId,
                    Name = r.Name,
                    Description = r.Description,
                    DateCreated = DateTime.UtcNow,
                    IsActive = true,
                }
            );
        }
        _db.SaveChanges();
    }

    private void SeedUsers()
    {
        var records = ReadCsv<UserCsvRow>("Seed_Users.csv");
        foreach (var r in records)
        {
            if (_db.User.Any(x => x.Id == r.UserId))
                continue;

            _db.Person.Add(
                new Person
                {
                    Id = r.PersonId,
                    FullName = r.FullName,
                    Email = r.Email,
                    MobileNumber = r.MobileNumber,
                    DesignationId = r.DesignationId,
                    GenderId = r.GenderId,
                    DateCreated = DateTime.UtcNow,
                    IsActive = true,
                }
            );
            _db.SaveChanges();

            _db.User.Add(
                new User
                {
                    Id = r.UserId,
                    PersonId = r.PersonId,
                    CurrentTierId = r.CurrentTierId,
                    OverallScore = r.OverallScore,
                    OverallWeightedScore = r.OverallWeightedScore,
                    DateCreated = DateTime.UtcNow,
                    IsActive = true,
                }
            );
        }
        _db.SaveChanges();
    }

    private void SeedCourses()
    {
        var records = ReadCsv<CourseCsvRow>("Seed_Courses.csv");
        foreach (var r in records)
        {
            if (_db.Course.Any(x => x.Id == r.Id))
                continue;
            _db.Course.Add(
                new Course
                {
                    Id = r.Id,
                    Title = r.Title,
                    Description = r.Description,
                    CategoryId = r.CategoryId,
                    SubCategoryId = r.SubCategoryId,
                    DifficultyLevelId = r.DifficultyLevelId,
                    CreatedByUserId = r.CreatedByUserId,
                    OverallRating = r.OverallRating,
                    DurationMinute = r.DurationMinute,
                    IsDrafted = r.IsDrafted,
                    DateCreated = DateTime.UtcNow,
                    IsActive = true,
                }
            );
        }
        _db.SaveChanges();
    }

    private void SeedMetaTopics()
    {
        var records = ReadCsv<MetaTopicCsvRow>("Seed_MetaTopics.csv");
        foreach (var r in records)
        {
            if (_db.CourseMetaTopic.Any(x => x.Id == r.Id))
                continue;
            _db.CourseMetaTopic.Add(
                new CourseMetaTopic
                {
                    Id = r.Id,
                    CourseId = r.CourseId,
                    Name = r.Name,
                    SequenceOrder = r.SequenceOrder,
                    DurationMinute = r.DurationMinute,
                    DateCreated = DateTime.UtcNow,
                    IsActive = true,
                }
            );
        }
        _db.SaveChanges();
    }

    private void SeedAssessments()
    {
        var records = ReadCsv<AssessmentCsvRow>("Seed_Assessments.csv");
        foreach (var r in records)
        {
            if (_db.Assessment.Any(x => x.Id == r.Id))
                continue;
            _db.Assessment.Add(
                new Assessment
                {
                    Id = r.Id,
                    CourseId = r.CourseId,
                    Title = r.Title,
                    Description = r.Description,
                    TotalMark = r.TotalMark,
                    PassingMark = r.PassingMark,
                    DurationMinute = r.DurationMinute,
                    DateCreated = DateTime.UtcNow,
                    IsActive = true,
                }
            );
        }
        _db.SaveChanges();
    }

    private void SeedQuestions()
    {
        var records = ReadCsv<QuestionCsvRow>("Seed_Questions.csv");
        foreach (var r in records)
        {
            if (_db.Question.Any(x => x.Id == r.Id))
                continue;

            List<string> optionList = string.IsNullOrWhiteSpace(r.OptionList)
                ? new List<string>()
                : JsonSerializer.Deserialize<List<string>>(r.OptionList) ?? new List<string>();

            _db.Question.Add(
                new Question
                {
                    Id = r.Id,
                    AssessmentId = r.AssessmentId,
                    MetaTopicId = r.MetaTopicId,
                    QuestionTypeId = r.QuestionTypeId,
                    QuestionText = r.QuestionText,
                    OptionList = optionList,
                    Answer = r.Answer,
                    Weight = r.Weight,
                    DateCreated = DateTime.UtcNow,
                    IsActive = true,
                }
            );
        }
        _db.SaveChanges();
    }

    private void SeedEnrollments()
    {
        var records = ReadCsv<EnrollmentCsvRow>("Seed_Enrollments.csv");
        foreach (var r in records)
        {
            if (_db.Enrollment.Any(x => x.Id == r.Id))
                continue;
            _db.Enrollment.Add(
                new Enrollment
                {
                    Id = r.Id,
                    UserId = r.UserId,
                    CourseId = r.CourseId,
                    EnrolledOn = r.EnrolledOn,
                    ProgressPercentage = r.ProgressPercentage,
                    EnrollmentStatus = r.EnrollmentStatus,
                    DateCreated = DateTime.UtcNow,
                    IsActive = true,
                }
            );
        }
        _db.SaveChanges();
    }

    private void SeedAssessmentHistory()
    {
        var records = ReadCsv<AssessmentHistoryCsvRow>("Seed_AssessmentHistories.csv");
        foreach (var r in records)
        {
            if (_db.AssessmentHistory.Any(x => x.Id == r.Id))
                continue;
            _db.AssessmentHistory.Add(
                new AssessmentHistory
                {
                    Id = r.Id,
                    UserId = r.UserId,
                    AssessmentId = r.AssessmentId,
                    StartedOn = r.StartedOn,
                    CompletedOn = r.CompletedOn,
                    Score = r.Score,
                    WeightedScore = r.WeightedScore,
                    TierAwardedId = r.TierAwardedId,
                    DateCreated = DateTime.UtcNow,
                    IsActive = true,
                }
            );
        }
        _db.SaveChanges();
    }

    private record RefSetCsvRow(Guid Id, string Name, string? Description);

    private record RefTermCsvRow(Guid Id, Guid RefSetId, string Name, string? Description);

    private record UserCsvRow(
        Guid PersonId,
        Guid UserId,
        string FullName,
        string Email,
        string MobileNumber,
        Guid DesignationId,
        Guid GenderId,
        decimal OverallScore,
        decimal OverallWeightedScore,
        Guid CurrentTierId
    );

    private record CourseCsvRow(
        Guid Id,
        string Title,
        string? Description,
        Guid CategoryId,
        Guid SubCategoryId,
        Guid DifficultyLevelId,
        Guid CreatedByUserId,
        decimal OverallRating,
        int DurationMinute,
        bool IsDrafted
    );

    private record MetaTopicCsvRow(
        Guid Id,
        Guid CourseId,
        string Name,
        int SequenceOrder,
        int DurationMinute
    );

    private record AssessmentCsvRow(
        Guid Id,
        Guid CourseId,
        string Title,
        string? Description,
        int TotalMark,
        int PassingMark,
        int DurationMinute
    );

    private record QuestionCsvRow(
        Guid Id,
        Guid AssessmentId,
        Guid MetaTopicId,
        Guid QuestionTypeId,
        string QuestionText,
        string? OptionList,
        string Answer,
        int Weight
    );

    private record EnrollmentCsvRow(
        Guid Id,
        Guid UserId,
        Guid CourseId,
        DateTime EnrolledOn,
        decimal ProgressPercentage,
        bool EnrollmentStatus
    );

    private record AssessmentHistoryCsvRow(
        Guid Id,
        Guid UserId,
        Guid AssessmentId,
        DateTime StartedOn,
        DateTime? CompletedOn,
        decimal Score,
        decimal WeightedScore,
        Guid? TierAwardedId
    );

    private record FeatureCsvRow(string Name, string Method, string? Description);

    private record RoleFeatureMappingCsvRow(string Role, string FeatureName);
}
