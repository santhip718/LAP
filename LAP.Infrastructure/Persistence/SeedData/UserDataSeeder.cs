using LAP.Application.Constant;
using LAP.Application.Interface;
using LAP.Domain.Entity;
using LAP.Shared.Helpers;
using Microsoft.EntityFrameworkCore;

namespace LAP.Infrastructure.Persistence.SeedData;

/// <summary>
/// Seeds default login users (Admin and Student) if they do not already exist.
/// </summary>
public class UserDataSeeder
{
    private readonly LearningAssessmentDbContext _context;
    private readonly ICustomLogger<UserDataSeeder> _logger;

    public UserDataSeeder(
        LearningAssessmentDbContext context,
        ICustomLogger<UserDataSeeder> logger
    )
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        _logger.LogInfo("Starting user data seeding...");

        // Retrieve Gender
        RefTerm? gender = await _context.RefTerm
            .Include(r => r.RefSet)
            .FirstOrDefaultAsync(r => r.RefSet.Name == "Gender" && r.Name == "Male")
            ?? await _context.RefTerm.FirstOrDefaultAsync(r => r.RefSet.Name == "Gender");

        // Retrieve Designation
        RefTerm? designation = await _context.RefTerm
            .Include(r => r.RefSet)
            .FirstOrDefaultAsync(r => r.RefSet.Name == "Designation" && r.Name == "Junior Developer")
            ?? await _context.RefTerm.FirstOrDefaultAsync(r => r.RefSet.Name == "Designation");

        // Retrieve Admin Role
        RefTerm? adminRole = await _context.RefTerm
            .Include(r => r.RefSet)
            .FirstOrDefaultAsync(r => r.RefSet.Name == "Role" && r.Name == RoleConstants.ADMIN_ROLE_NAME)
            ?? await _context.RefTerm.FirstOrDefaultAsync(r => r.Id == RoleConstants.ADMIN_ID);

        // Retrieve Student Role
        RefTerm? studentRole = await _context.RefTerm
            .Include(r => r.RefSet)
            .FirstOrDefaultAsync(r => r.RefSet.Name == "Role" && r.Name == "Student")
            ?? await _context.RefTerm.FirstOrDefaultAsync(r => r.Id == RoleConstants.STUDENT_ID);

        if (gender == null || designation == null || adminRole == null || studentRole == null)
        {
            _logger.LogWarning("Required reference data (Gender, Designation, Admin Role, or Student Role) not found. Skipping user seeding.");
            return;
        }

        // 1. Seed Admin User
        await CreateUserIfNotExistsAsync(
            email: "admin@lap.com",
            fullName: "Admin User",
            mobile: "9876543210",
            password: "Admin@123",
            genderId: gender.Id,
            designationId: designation.Id,
            roleIds: new[] { adminRole.Id, studentRole.Id }
        );

        // 2. Seed Student User: student1@gmail.com
        await CreateUserIfNotExistsAsync(
            email: "student1@gmail.com",
            fullName: "Student One",
            mobile: "9876543211",
            password: "Student@123",
            genderId: gender.Id,
            designationId: designation.Id,
            roleIds: new[] { studentRole.Id }
        );

        await _context.SaveChangesAsync();
        _logger.LogInfo("User data seeding completed.");
    }

    private async Task CreateUserIfNotExistsAsync(
        string email,
        string fullName,
        string mobile,
        string password,
        Guid genderId,
        Guid designationId,
        IEnumerable<Guid> roleIds
    )
    {
        bool userExists = await _context.Person.AnyAsync(p => p.Email == email);
        if (userExists)
        {
            _logger.LogInfo("Seed user '{Email}' already exists. Skipping.", email);
            return;
        }

        var person = new Person
        {
            FullName = fullName,
            Email = email,
            MobileNumber = mobile,
            GenderId = genderId,
            DesignationId = designationId
        };
        await _context.Person.AddAsync(person);

        var user = new User
        {
            PersonId = person.Id,
            OverallScore = 0,
            OverallWeightedScore = 0
        };
        await _context.User.AddAsync(user);

        string passwordHash = UserSecretHelper.HashPasswordBcrypt(password, out string passwordSalt);
        var userSecret = new UserSecret
        {
            UserId = user.Id,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt
        };
        await _context.UserSecret.AddAsync(userSecret);

        foreach (var roleId in roleIds)
        {
            var roleMapping = new UserRoleMapping
            {
                UserId = user.Id,
                RoleId = roleId
            };
            await _context.UserRoleMapping.AddAsync(roleMapping);
        }

        _logger.LogInfo("Prepared seed user '{Email}' with role(s). Password: {Password}", email, password);
    }
}
