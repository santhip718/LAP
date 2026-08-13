using System.Linq.Expressions;
using System.Text.Json;
using LAP.Application.Interface.IContext;
using LAP.Domain.Entity;
using LAP.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace LAP.Infrastructure.Persistence;

public class LearningAssessmentDbContext : DbContext
{
    private readonly IRequestContext _requestContext;

    public LearningAssessmentDbContext(
        DbContextOptions<LearningAssessmentDbContext> options,
        IRequestContext requestContext
    )
        : base(options)
    {
        _requestContext = requestContext;
    }

    #region DbSets

    public DbSet<Person> Person => Set<Person>();

    public DbSet<User> User => Set<User>();

    public DbSet<UserSecret> UserSecret => Set<UserSecret>();

    public DbSet<UserRoleMapping> UserRoleMapping => Set<UserRoleMapping>();

    public DbSet<Feature> Feature => Set<Feature>();

    public DbSet<RoleFeatureMapping> RoleFeatureMapping => Set<RoleFeatureMapping>();

    public DbSet<RefSet> RefSet => Set<RefSet>();

    public DbSet<RefTerm> RefTerm => Set<RefTerm>();

    public DbSet<Course> Course => Set<Course>();

    public DbSet<CourseMetaTopic> CourseMetaTopic => Set<CourseMetaTopic>();

    public DbSet<CourseContent> CourseContent => Set<CourseContent>();

    public DbSet<Enrollment> Enrollment => Set<Enrollment>();

    public DbSet<Assessment> Assessment => Set<Assessment>();

    public DbSet<Question> Question => Set<Question>();

    public DbSet<AssessmentHistory> AssessmentHistory => Set<AssessmentHistory>();

    public DbSet<AssessmentAnswer> AssessmentAnswer => Set<AssessmentAnswer>();

    public DbSet<Review> Review => Set<Review>();

    public DbSet<ForumMessage> ForumMessage => Set<ForumMessage>();

    public DbSet<UserCourseProgress> UserCourseProgress => Set<UserCourseProgress>();

    public DbSet<ImportJob> ImportJob => Set<ImportJob>();
    public DbSet<RefreshToken> RefreshToken => Set<RefreshToken>();

    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("LAP");

        ConfigureRelationships(modelBuilder);
        ConfigureIndexes(modelBuilder);

        modelBuilder.ApplySnakeCaseNamingConvention();
    }

    private static void ConfigureIndexes(ModelBuilder modelBuilder)
    {
        // ── Person ──────────────────────────────────────────────
        modelBuilder.Entity<Person>().HasIndex(x => x.DesignationId);
        // JOIN to RefTerm on every profile read (designation label display)

        modelBuilder.Entity<Person>().HasIndex(x => x.GenderId);
        // JOIN to RefTerm on every profile read (gender label display)

        modelBuilder.Entity<Person>().HasIndex(x => x.Email).IsUnique();
        // Login lookup: WHERE email = ? on every authentication request; unique enforces no duplicate accounts

        modelBuilder.Entity<Person>().HasIndex(x => x.IsActive);
        // Soft-delete filter applied to every Person query via global query filter

        // ── User ────────────────────────────────────────────────
        modelBuilder.Entity<User>().HasIndex(x => x.CurrentTierId);
        // JOIN to RefTerm for tier label; filtered in leaderboard/progress queries

        modelBuilder.Entity<User>().HasIndex(x => x.IsActive);
        // Soft-delete filter on every User query

        // ── UserSecret ──────────────────────────────────────────
        modelBuilder.Entity<UserSecret>().HasIndex(x => x.UserId).IsUnique();
        // Every login/password-reset fetches secret by UserId; unique = one secret row per user

        modelBuilder.Entity<UserSecret>().HasIndex(x => x.IsActive);
        // Soft-delete filter

        // ── UserRoleMapping ─────────────────────────────────────
        modelBuilder.Entity<UserRoleMapping>().HasIndex(x => x.UserId);
        // Authorization: fetch all roles for a user on every request

        modelBuilder.Entity<UserRoleMapping>().HasIndex(x => x.RoleId);
        // Reverse lookup: which users have a given role (admin screens)

        modelBuilder
            .Entity<UserRoleMapping>()
            .HasIndex(x => new { x.UserId, x.RoleId })
            .IsUnique();
        // Composite: role-check query filters both columns together; unique prevents duplicate assignments

        modelBuilder.Entity<UserRoleMapping>().HasIndex(x => x.IsActive);
        // Soft-delete filter

        // ── Feature ─────────────────────────────────────────────
        modelBuilder.Entity<Feature>().HasIndex(x => new { x.Method });
        // Authorization middleware: WHERE url = ? AND method = ? on every API request to resolve the feature

        modelBuilder.Entity<Feature>().HasIndex(x => x.IsActive);
        // Soft-delete filter

        // ── RoleFeatureMapping ──────────────────────────────────
        modelBuilder.Entity<RoleFeatureMapping>().HasIndex(x => x.RoleId);
        // Permission check: fetch all features allowed for a role

        modelBuilder.Entity<RoleFeatureMapping>().HasIndex(x => x.FeatureId);
        // Reverse lookup: which roles can access a feature

        modelBuilder.Entity<RoleFeatureMapping>().HasIndex(x => new { x.RoleId, x.FeatureId });
        // Composite: exact permission check filters both columns together in one seek

        modelBuilder.Entity<RoleFeatureMapping>().HasIndex(x => x.IsActive);
        // Soft-delete filter

        // ── RefSet ──────────────────────────────────────────────
        modelBuilder.Entity<RefSet>().HasIndex(x => x.IsActive);
        // Soft-delete filter

        // ── RefTerm ─────────────────────────────────────────────
        modelBuilder.Entity<RefTerm>().HasIndex(x => x.RefSetId);
        // Every dropdown/label query filters by RefSetId (e.g. get all Gender terms)

        modelBuilder.Entity<RefTerm>().HasIndex(x => x.IsActive);
        // Soft-delete filter

        // ── Course ──────────────────────────────────────────────
        modelBuilder.Entity<Course>().HasIndex(x => x.CategoryId);
        // Browse/filter by category on course listing page

        modelBuilder.Entity<Course>().HasIndex(x => x.SubCategoryId);
        // Browse/filter by sub-category on course listing page

        modelBuilder.Entity<Course>().HasIndex(x => x.DifficultyLevelId);
        // Filter by difficulty level on course listing page

        modelBuilder.Entity<Course>().HasIndex(x => x.CreatedByUserId);
        // Fetch courses created by a specific instructor

        modelBuilder.Entity<Course>().HasIndex(x => x.Title);
        // Search: WHERE title ILIKE '%keyword%' benefits from index on Title

        modelBuilder.Entity<Course>().HasIndex(x => x.IsActive);
        // Soft-delete filter

        // ── CourseMetaTopic ─────────────────────────────────────
        modelBuilder.Entity<CourseMetaTopic>().HasIndex(x => x.CourseId);
        // Fetch all topics for a course (course detail page)

        modelBuilder.Entity<CourseMetaTopic>().HasIndex(x => new { x.CourseId, x.SequenceOrder });
        // Composite: topics fetched per course already ordered by sequence — avoids sort step

        modelBuilder.Entity<CourseMetaTopic>().HasIndex(x => x.IsActive);
        // Soft-delete filter

        // ── CourseContent ───────────────────────────────────────
        modelBuilder.Entity<CourseContent>().HasIndex(x => x.MetaTopicId);
        // Fetch all content items under a topic

        modelBuilder.Entity<CourseContent>().HasIndex(x => x.ContentTypeId);
        // Filter content by type (video/article/quiz) on topic view

        modelBuilder
            .Entity<CourseContent>()
            .HasIndex(x => new { x.MetaTopicId, x.SequenceOrder });
        // Composite: content fetched per topic in sequence order — avoids sort step

        modelBuilder.Entity<CourseContent>().HasIndex(x => x.IsActive);
        // Soft-delete filter

        // ── Enrollment ──────────────────────────────────────────
        modelBuilder.Entity<Enrollment>().HasIndex(x => x.UserId);
        // Fetch all courses a user is enrolled in (my courses page)

        modelBuilder.Entity<Enrollment>().HasIndex(x => x.CourseId);
        // Fetch all enrolled users for a course (instructor/admin view)

        modelBuilder.Entity<Enrollment>().HasIndex(x => new { x.UserId, x.CourseId }).IsUnique();
        // Composite: check if user is enrolled in a specific course; unique prevents duplicate enrollments

        modelBuilder.Entity<Enrollment>().HasIndex(x => x.EnrolledOn);
        // Date-range filter in enrollment reports (e.g. enrolled between Jan–Mar)

        modelBuilder.Entity<Enrollment>().HasIndex(x => x.CompletedOn);
        // Date-range filter in completion reports; also used for completion rate queries

        modelBuilder.Entity<Enrollment>().HasIndex(x => x.IsActive);
        // Soft-delete filter

        // ── Assessment ──────────────────────────────────────────
        // CourseId unique index already defined in ConfigureRelationships
        modelBuilder.Entity<Assessment>().HasIndex(x => x.IsActive);
        // Soft-delete filter

        // ── Question ────────────────────────────────────────────
        modelBuilder.Entity<Question>().HasIndex(x => x.AssessmentId);
        // Fetch all questions for an assessment

        modelBuilder.Entity<Question>().HasIndex(x => x.MetaTopicId);
        // Filter questions by topic (topic-wise question breakdown)

        modelBuilder.Entity<Question>().HasIndex(x => x.QuestionTypeId);
        // Filter by question type (MCQ/True-False/etc.)

        modelBuilder.Entity<Question>().HasIndex(x => new { x.AssessmentId, x.MetaTopicId });
        // Composite: topic-wise question grouping within an assessment in one seek

        modelBuilder.Entity<Question>().HasIndex(x => x.IsActive);
        // Soft-delete filter

        // ── AssessmentHistory ───────────────────────────────────
        modelBuilder.Entity<AssessmentHistory>().HasIndex(x => x.UserId);
        // Fetch all attempts by a user (my results page)

        modelBuilder.Entity<AssessmentHistory>().HasIndex(x => x.AssessmentId);
        // Fetch all attempts for an assessment (admin/instructor view)

        modelBuilder.Entity<AssessmentHistory>().HasIndex(x => x.TierAwardedId);
        // Filter attempts by tier awarded (leaderboard / tier breakdown reports)

        modelBuilder.Entity<AssessmentHistory>().HasIndex(x => new { x.UserId, x.AssessmentId });
        // Composite: check if user has already attempted this assessment; used in attempt-limit logic

        modelBuilder.Entity<AssessmentHistory>().HasIndex(x => x.StartedOn);
        // Date-range filter in attempt reports

        modelBuilder.Entity<AssessmentHistory>().HasIndex(x => x.CompletedOn);
        // Date-range filter; null check here also drives "in-progress" detection

        modelBuilder.Entity<AssessmentHistory>().HasIndex(x => x.IsActive);
        // Soft-delete filter

        // ── AssessmentAnswer ────────────────────────────────────
        modelBuilder.Entity<AssessmentAnswer>().HasIndex(x => x.AssessmentHistoryId);
        // Fetch all answers for a single attempt (result detail page)

        modelBuilder.Entity<AssessmentAnswer>().HasIndex(x => x.QuestionId);
        // Reverse lookup: which attempts answered a specific question (question analytics)

        modelBuilder.Entity<AssessmentAnswer>().HasIndex(x => x.IsActive);
        // Soft-delete filter

        // ── Review ──────────────────────────────────────────────
        modelBuilder.Entity<Review>().HasIndex(x => x.UserId);
        // Fetch all reviews written by a user

        modelBuilder.Entity<Review>().HasIndex(x => x.CourseId);
        // Fetch all reviews for a course (course detail rating section)

        modelBuilder.Entity<Review>().HasIndex(x => new { x.UserId, x.CourseId }).IsUnique();
        // Composite: check if user already reviewed this course; unique prevents duplicate reviews

        modelBuilder.Entity<Review>().HasIndex(x => x.IsActive);
        // Soft-delete filter

        // ── ForumMessage ────────────────────────────────────────
        modelBuilder.Entity<ForumMessage>().HasIndex(x => x.CourseId);
        // Fetch all messages in a course forum (forum thread view)

        modelBuilder.Entity<ForumMessage>().HasIndex(x => x.UserId);
        // Fetch all messages posted by a user

        modelBuilder.Entity<ForumMessage>().HasIndex(x => new { x.CourseId, x.UserId });
        // Composite: filter a user's posts within a specific course forum

        modelBuilder.Entity<ForumMessage>().HasIndex(x => x.IsActive);
        // Soft-delete filter

        // ── UserCourseProgress ──────────────────────────────────
        modelBuilder.Entity<UserCourseProgress>().HasIndex(x => x.EnrollmentId);
        // Fetch all progress records for an enrollment (progress bar calculation)

        modelBuilder.Entity<UserCourseProgress>().HasIndex(x => x.CourseContentId);
        // Reverse lookup: which enrollments have accessed a content item

        modelBuilder
            .Entity<UserCourseProgress>()
            .HasIndex(x => new { x.EnrollmentId, x.CourseContentId })
            .IsUnique();
        // Composite: check/upsert progress for a specific content item in an enrollment; unique prevents duplicates

        modelBuilder.Entity<UserCourseProgress>().HasIndex(x => x.IsCompleted);
        // Filter: count completed items for progress percentage (WHERE IsCompleted = true)

        modelBuilder.Entity<UserCourseProgress>().HasIndex(x => x.IsActive);
        // Soft-delete filter

        // ── ImportJob ───────────────────────────────────────────
        modelBuilder.Entity<ImportJob>().HasIndex(x => x.AssessmentId);
        // Fetch all import jobs for an assessment (question import history)

        modelBuilder.Entity<ImportJob>().HasIndex(x => x.StatusId);
        // Poll pending jobs: WHERE StatusId = 'Pending' on background job processor

        modelBuilder.Entity<ImportJob>().HasIndex(x => x.IsActive);
        // Soft-delete filter

        // ── RefreshToken ────────────────────────────────────────
        modelBuilder.Entity<RefreshToken>().HasIndex(x => x.UserId);
        // Fetch/revoke all tokens for a user on logout or password change

        modelBuilder.Entity<RefreshToken>().HasIndex(x => x.Token).IsUnique();
        // Token refresh flow: WHERE token = ? on every refresh request; unique prevents collision

        modelBuilder.Entity<RefreshToken>().HasIndex(x => x.IsActive);
        // Soft-delete filter (also doubles as revocation flag check)
    }

    private static void ConfigureRelationships(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<Person>()
            .HasOne(x => x.User)
            .WithOne(x => x.Person)
            .HasForeignKey<User>(x => x.PersonId);

        modelBuilder
            .Entity<User>()
            .HasOne(x => x.UserSecret)
            .WithOne(x => x.User)
            .HasForeignKey<UserSecret>(x => x.UserId);
        modelBuilder
            .Entity<RefreshToken>()
            .HasOne(x => x.User)
            .WithMany(x => x.RefreshTokens)
            .HasForeignKey(x => x.UserId);

        modelBuilder
            .Entity<Course>()
            .HasOne(x => x.Assessment)
            .WithOne(x => x.Course)
            .HasForeignKey<Assessment>(x => x.CourseId);

        modelBuilder
            .Entity<Question>()
            .Property(x => x.OptionList)
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v =>
                    JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null)
                    ?? new List<string>()
            );

        modelBuilder.Entity<Assessment>().HasIndex(x => x.CourseId).IsUnique().HasFilter("is_active = true");
    }

    private void UpdateEntities()
    {
        var entries = ChangeTracker.Entries<BaseEntity>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.DateCreated = DateTime.UtcNow;
                entry.Entity.CreatedBy = _requestContext.UserId;

                entry.Entity.IsActive = true;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.DateUpdated = DateTime.UtcNow;
                entry.Entity.UpdatedBy = _requestContext.UserId;
            }
        }
    }

    public override int SaveChanges()
    {
        UpdateEntities();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateEntities();
        return await base.SaveChangesAsync(cancellationToken);
    }
}
