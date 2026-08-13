namespace LAP.Application.Constant;

/// <summary>
/// Defines shared constant values used across the application.
/// </summary>
public static class CommonConstants
{
    /// <summary>The cache key for storing reference sets in memory cache.</summary>
    public const string REF_SETS_CACHE_KEY = "REF_SETS";

    /// <summary>The cache key for storing reference terms in memory cache.</summary>
    public const string REF_TERMS_CACHE_KEY = "REF_TERMS";

    /// <summary>The prefix used to construct policy names for feature-based authorization.</summary>
    public const string PolicyPrefix = "Feature_";

    /// <summary>Allowed image MIME types.</summary>
    public static readonly string[] ALLOWED_IMAGE_TYPES =
    [
        "image/jpeg",
        "image/png",
        "image/gif",
        "image/webp",
    ];

    /// <summary>Allowed document MIME types.</summary>
    public static readonly string[] ALLOWED_DOCUMENT_TYPES = ["application/pdf"];

    /// <summary>The maximum number of attempts allowed per assessment.</summary>
    public const int MaxAssessmentAttempt = 3;

    /// <summary>The name of the reference set for tiers.</summary>
    public const string TIER_REF_SET_NAME = "Tier";

    /// <summary>The name of the question type reference set.</summary>
    public const string QUESTION_TYPE_REF_SET_NAME = "QuestionType";

    /// <summary>The PDF file extension.</summary>
    public const string PDF_EXTENSION = ".pdf";

    /// <summary>Format path for course content PDF files.</summary>
    public const string COURSE_CONTENT_PATH_FORMAT = "coursecontent/{0}/content/{0}{1}";

    /// <summary>Format path for course thumbnail image files.</summary>
    public const string COURSE_THUMBNAIL_PATH_FORMAT = "course/{0}/thumbnail/image{1}";

    /// <summary>The name of the MCQ question type.</summary>
    public const string QUESTION_TYPE_MCQ = "MCQ";

    /// <summary>The name of the True/False question type.</summary>
    public const string QUESTION_TYPE_TRUE_FALSE = "TrueFalse";

    /// <summary>The name of the Fill In Blank question type.</summary>
    public const string QUESTION_TYPE_FILL_IN_BLANK = "FillInBlank";

    /// <summary>Substring for identifying True/False questions.</summary>
    public const string TRUE_SUBSTRING = "True";

    /// <summary>Substring for identifying True/False questions.</summary>
    public const string FALSE_SUBSTRING = "False";

    /// <summary>Substring for identifying Multiple Choice questions.</summary>
    public const string MULTIPLE_SUBSTRING = "Multiple";

    /// <summary>Substring for identifying Multiple Choice questions.</summary>
    public const string CHOICE_SUBSTRING = "Choice";

    /// <summary>Substring for identifying Fill In Blank questions.</summary>
    public const string FILL_SUBSTRING = "Fill";

    /// <summary>Substring for identifying Fill In Blank questions.</summary>
    public const string BLANK_SUBSTRING = "Blank";

    /// <summary>The file name of the question import template.</summary>
    public const string QuestionTemplateFileName = "Question Template File.xlsx";

    /// <summary>The default page size for leaderboard queries.</summary>
    public const int DEFAULT_PAGE_SIZE = 25;

    /// <summary>Allowed image file extensions for upload.</summary>
    public static readonly string[] ALLOWED_IMAGE_EXTENSIONS = [".jpg", ".jpeg", ".png", ".webp"];

    /// <summary>The maximum profile image file size allowed (5 MB).</summary>
    public const long MAX_PROFILE_IMAGE_SIZE = 5L * 1024L * 1024L;

    /// <summary>Maps file extensions to their corresponding MIME types.</summary>
    public static readonly Dictionary<string, string> MIME_TYPE_MAP = new()
    {
        [".png"] = "image/png",
        [".webp"] = "image/webp",
        [".gif"] = "image/gif",
        [".pdf"] = "application/pdf",
    };

    /// <summary>The maximum number of top recommendations to retrieve.</summary>
    public const int TOP_10_RECOMMENDATIONS = 10;

    /// <summary>The tier name for scores 0-20%.</summary>
    public const string TIER_CODE_CADET = "Code Cadet";

    /// <summary>The tier name for scores 21-40%.</summary>
    public const string TIER_SYNTAX_VOYAGER = "Syntax Voyager";

    /// <summary>The tier name for scores 41-60%.</summary>
    public const string TIER_LOGIC_ARCHITECT = "Logic Architect";

    /// <summary>The tier name for scores 61-80%.</summary>
    public const string TIER_RUNTIME_TITAN = "Runtime Titan";

    /// <summary>The tier name for scores 81-100%.</summary>
    public const string TIER_SYSTEM_SOVEREIGN = "System Sovereign";

    /// <summary>Key for storing user ID in HTTP context items.</summary>
    public const string CONTEXT_USER_ID = "UserId";

    /// <summary>Key for storing email in HTTP context items.</summary>
    public const string CONTEXT_EMAIL = "Email";

    /// <summary>Key for storing role in HTTP context items.</summary>
    public const string CONTEXT_ROLE = "Role";

    /// <summary>Key for storing authentication status in HTTP context items.</summary>
    public const string CONTEXT_IS_AUTHENTICATED = "IsAuthenticated";
}
