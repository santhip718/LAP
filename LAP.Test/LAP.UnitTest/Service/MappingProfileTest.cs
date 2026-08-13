using AutoMapper;
using LAP.Application.DTO.Assessment;
using LAP.Application.DTO.Course;
using LAP.Application.DTO.CourseReview;
using LAP.Application.DTO.Enrollment;
using LAP.Application.DTO.Review;
using LAP.Application.DTO.User;
using LAP.Application.Interface.IService;
using LAP.Application.Mapping;
using LAP.Domain.Entity;
using Moq;

namespace LAP.UnitTest.Service;

public class MappingProfileTest
{
    private static IMapper CreateMapperWithAllProfiles()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<UserMappingProfile>();
            cfg.AddProfile<CourseMappingProfile>();
            cfg.AddProfile<ReviewMappingProfile>();
            cfg.AddProfile<AssessmentMappingProfile>();
        });
        return config.CreateMapper();
    }

    [Fact]
    public void UserMapping_ShouldMap_UserToUserDetailDto()
    {
        var mapper = CreateMapperWithAllProfiles();

        var person = new Person
        {
            FullName = "John Doe",
            Email = "john@test.com",
            MobileNumber = "1234567890",
            Designation = new RefTerm { Name = "Developer" },
            Gender = new RefTerm { Name = "Male" },
        };
        var user = new User
        {
            Id = Guid.NewGuid(),
            Person = person,
            CurrentTier = new RefTerm { Name = "Code Cadet" },
            UserRoles = new List<UserRoleMapping>
            {
                new() { Role = new RefTerm { Name = "Student" } },
            },
            DateCreated = DateTime.UtcNow,
        };

        var dto = mapper.Map<UserDetailDto>(user);

        Assert.Equal("John Doe", dto.FullName);
        Assert.Equal("john@test.com", dto.Email);
        Assert.Equal("1234567890", dto.MobileNumber);
        Assert.Contains("Student", dto.Roles);
    }

    [Fact]
    public void UserMapping_ShouldMap_UserToUserSummaryDto()
    {
        var mapper = CreateMapperWithAllProfiles();
        var person = new Person { FullName = "Jane Doe", Email = "jane@test.com" };
        var user = new User
        {
            Person = person,
            UserRoles = new List<UserRoleMapping>
            {
                new() { Role = new RefTerm { Name = "Admin" } },
            },
        };

        var dto = mapper.Map<UserSummaryDto>(user);

        Assert.Equal("Jane Doe", dto.FullName);
        Assert.Equal("jane@test.com", dto.Email);
    }

    [Fact]
    public void CourseMapping_ShouldMap_CourseToCourseSummaryDto()
    {
        var mapper = CreateMapperWithAllProfiles();
        var course = new Course
        {
            Id = Guid.NewGuid(),
            Title = "Test Course",
            IsDrafted = false,
        };

        var dto = mapper.Map<CourseSummaryDto>(course);

        Assert.Equal("Test Course", dto.Title);
        Assert.False(dto.IsDrafted);
    }


    [Fact]
    public void CourseMapping_ShouldMap_CourseToCourseDetailDto()
    {
        var mapper = CreateMapperWithAllProfiles();
        var course = new Course
        {
            Id = Guid.NewGuid(),
            Title = "Detail Course",
            Description = "Desc",
            IsDrafted = true,
            DateCreated = DateTime.UtcNow,
        };

        var dto = mapper.Map<CourseDetailDto>(course);

        Assert.Equal("Detail Course", dto.Title);
        Assert.True(dto.IsDrafted);
    }

    [Fact]
    public void CourseMapping_ShouldMap_EnrollmentToCourseProgressResponseDto()
    {
        var mapper = CreateMapperWithAllProfiles();
        var enrollment = new Enrollment
        {
            Id = Guid.NewGuid(),
            ProgressPercentage = 50,
            CompletedOn = DateTime.UtcNow,
            EnrollmentStatus = true,
        };

        var dto = mapper.Map<CourseProgressResponseDto>(enrollment);

        Assert.Equal(50, dto.ProgressPercentage);
        Assert.True(dto.EnrollmentStatus);
        Assert.NotNull(dto.CompletedOn);
    }

    [Fact]
    public void ReviewMapping_ShouldMap_ReviewToReviewDto()
    {
        var mapper = CreateMapperWithAllProfiles();
        var person = new Person { FullName = "John Doe" };
        var user = new User { Person = person };
        var review = new Review
        {
            Id = Guid.NewGuid(),
            Rating = 4,
            ReviewText = "Great!",
            User = user,
            DateCreated = DateTime.UtcNow,
        };

        var dto = mapper.Map<ReviewDto>(review);

        Assert.Equal(4, dto.Rating);
        Assert.Equal("Great!", dto.ReviewText);
        Assert.Equal("John Doe", dto.UserFullName);
    }

    [Fact]
    public void ReviewMapping_ShouldMap_CreateReviewRequestDtoToReview()
    {
        var mapper = CreateMapperWithAllProfiles();
        var dto = new CreateReviewRequestDto { Rating = 5, ReviewText = "Excellent" };

        var review = mapper.Map<Review>(dto);

        Assert.Equal(5, review.Rating);
        Assert.Equal("Excellent", review.ReviewText);
    }

    [Fact]
    public void ReviewMapping_ShouldMap_UpdateReviewRequestDtoToReview()
    {
        var mapper = CreateMapperWithAllProfiles();
        var dto = new UpdateReviewRequestDto { Rating = 3, ReviewText = "Average" };

        var review = mapper.Map<Review>(dto);

        Assert.Equal(3, review.Rating);
        Assert.Equal("Average", review.ReviewText);
    }

    [Fact]
    public void AssessmentMapping_ShouldMap_AssessmentHistoryToAssessmentHistoryDto()
    {
        var mapper = CreateMapperWithAllProfiles();
        var history = new AssessmentHistory
        {
            Id = Guid.NewGuid(),
            Score = 85,
            WeightedScore = 80,
            StartedOn = DateTime.UtcNow.AddHours(-1),
            CompletedOn = DateTime.UtcNow,
            TierAwarded = new RefTerm { Name = "Runtime Titan" },
        };

        var dto = mapper.Map<AssessmentHistoryDto>(history);

        Assert.NotNull(dto.TierAwarded);
        Assert.Equal("Runtime Titan", dto.TierAwarded.Name);
    }

    [Fact]
    public void CourseMapping_ShouldMap_CourseContentToCourseContentDto()
    {
        var mapper = CreateMapperWithAllProfiles();
        var contentType = new RefTerm { Name = "Video" };
        var metaTopic = new CourseMetaTopic
        {
            Name = "Intro",
            SequenceOrder = 1,
            DurationMinute = 10,
        };
        var content = new CourseContent
        {
            Id = Guid.NewGuid(),
            Title = "Getting Started",
            ContentType = contentType,
            MetaTopic = metaTopic,
            SequenceOrder = 1,
            VideoUrl = "https://example.com/video.mp4",
        };

        var dto = mapper.Map<CourseContentDto>(content);

        Assert.Equal("Getting Started", dto.Title);
        Assert.NotNull(dto.VideoUrl);
        Assert.Equal(1, dto.SequenceOrder);
    }

    [Fact]
    public void CourseMapping_ShouldMap_CourseMetaTopicToCourseMetaTopicDto()
    {
        var mapper = CreateMapperWithAllProfiles();
        var topic = new CourseMetaTopic
        {
            Id = Guid.NewGuid(),
            Name = "Module 1",
            SequenceOrder = 1,
            DurationMinute = 30,
        };

        var dto = mapper.Map<CourseMetaTopicDto>(topic);

        Assert.Equal("Module 1", dto.Name);
        Assert.Equal(1, dto.SequenceOrder);
    }

    [Fact]
    public void UserMapping_ShouldMap_EnrollmentToEnrolledCourseDto()
    {
        var mapper = CreateMapperWithAllProfiles();
        var course = new Course
        {
            Id = Guid.NewGuid(),
            Title = "My Course",
            Category = new RefTerm { Name = "Tech" },
            DifficultyLevel = new RefTerm { Name = "Beginner" },
        };
        var enrollment = new Enrollment
        {
            Id = Guid.NewGuid(),
            CourseId = course.Id,
            Course = course,
            EnrolledOn = DateTime.UtcNow,
            CompletedOn = null,
            ProgressPercentage = 30,
        };

        var dto = mapper.Map<EnrolledCourseDto>(enrollment);

        Assert.Equal("My Course", dto.CourseTitle);
        Assert.Equal(30, (int)dto.ProgressPercentage);
    }

    [Fact]
    public void CourseMapping_ShouldMap_CourseContentToCourseContentProgressDto()
    {
        var mapper = CreateMapperWithAllProfiles();
        var contentType = new RefTerm { Name = "Video" };
        var metaTopic = new CourseMetaTopic
        {
            Name = "Intro",
            SequenceOrder = 1,
            DurationMinute = 10,
        };
        var content = new CourseContent
        {
            Id = Guid.NewGuid(),
            Title = "Getting Started",
            ContentType = contentType,
            MetaTopic = metaTopic,
            UserCourseProgresses = new List<UserCourseProgress>
            {
                new() { IsCompleted = true, CompletedOn = DateTime.UtcNow },
            },
        };

        var dto = mapper.Map<CourseContentProgressDto>(content);

        Assert.True(dto.IsCompleted);
        Assert.NotNull(dto.CompletedOn);
    }
}
