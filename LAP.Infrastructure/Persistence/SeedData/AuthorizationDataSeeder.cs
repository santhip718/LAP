using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using LAP.Application.Interface;
using LAP.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace LAP.Infrastructure.Persistence.SeedData;

public class AuthorizationDataSeeder
{
    private readonly LearningAssessmentDbContext _context;
    private readonly ICustomLogger<AuthorizationDataSeeder> _logger;

    public AuthorizationDataSeeder(
        LearningAssessmentDbContext context,
        ICustomLogger<AuthorizationDataSeeder> logger
    )
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        _logger.LogInfo("Starting authorization data seeding...");

        CsvConfiguration config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HeaderValidated = null,
            MissingFieldFound = null,
        };

        await SeedFeaturesAsync(config);
        await _context.SaveChangesAsync();

        await SeedRoleFeatureMappingsAsync(config);
        await _context.SaveChangesAsync();

        _logger.LogInfo("Authorization data seeding completed.");
    }

    private async Task SeedFeaturesAsync(CsvConfiguration config)
    {
        string filePath = Path.Combine(
            AppContext.BaseDirectory,
            "Persistence",
            "SeedData",
            "Features.csv"
        );

        if (!File.Exists(filePath))
        {
            _logger.LogWarning(
                "Features.csv not found at {FilePath}. Skipping feature seeding.",
                filePath
            );
            return;
        }

        using StreamReader reader = new StreamReader(filePath);
        using CsvReader csv = new CsvReader(reader, config);

        List<FeatureSeedModel> recordList = csv.GetRecords<FeatureSeedModel>().ToList();
        _logger.LogInfo("Found {Count} feature records in CSV.", recordList.Count);

        int addedCount = 0;
        foreach (FeatureSeedModel record in recordList)
        {
            bool exists = await _context.Feature.AnyAsync(x =>
                x.Name == record.Name && x.Method == record.Method
            );

            if (exists)
            {
                _logger.LogDebug(
                    "Feature already exists. Skipping. Name: {Name}, Method: {Method}",
                    record.Name,
                    record.Method
                );
                continue;
            }

            await _context.Feature.AddAsync(
                new Feature
                {
                    Name = record.Name,
                    Method = record.Method,
                    Description = record.Description,
                    IsActive = true,
                }
            );
            addedCount++;
        }

        _logger.LogInfo(
            "Feature seeding complete. Added: {AddedCount}, Skipped: {SkippedCount}.",
            addedCount,
            recordList.Count - addedCount
        );
    }

    private async Task SeedRoleFeatureMappingsAsync(CsvConfiguration config)
    {
        string filePath = Path.Combine(
            AppContext.BaseDirectory,
            "Persistence",
            "SeedData",
            "RoleFeatureMappings.csv"
        );

        if (!File.Exists(filePath))
        {
            _logger.LogWarning(
                "RoleFeatureMappings.csv not found at {FilePath}. Skipping role-feature mapping seeding.",
                filePath
            );
            return;
        }

        using StreamReader reader = new StreamReader(filePath);
        using CsvReader csv = new CsvReader(reader, config);

        List<RoleFeatureMappingSeedModel> recordList = csv.GetRecords<RoleFeatureMappingSeedModel>()
            .ToList();
        _logger.LogInfo("Found {Count} role-feature mapping records in CSV.", recordList.Count);

        RefSet? roleRefSet = await _context.RefSet.FirstOrDefaultAsync(x => x.Name == "Role");

        if (roleRefSet == null)
        {
            _logger.LogError("RefSet 'Role' not found. Please seed RefSet and RefTerm data first.");
            throw new Exception(
                "RefSet 'Role' not found. Please seed RefSet and RefTerm data first."
            );
        }

        int addedCount = 0;
        int skippedCount = 0;
        foreach (RoleFeatureMappingSeedModel record in recordList)
        {
            RefTerm? role = await _context.RefTerm.FirstOrDefaultAsync(x =>
                x.RefSetId == roleRefSet.Id && x.Name == record.Role
            );

            if (role == null)
            {
                _logger.LogWarning(
                    "Role '{Role}' not found in RefTerms. Skipping mapping for FeatureName: {FeatureName}.",
                    record.Role,
                    record.FeatureName
                );
                skippedCount++;
                continue;
            }

            List<Feature> featureList = await _context
                .Feature.Where(f => f.Name == record.FeatureName)
                .ToListAsync();

            if (!featureList.Any())
            {
                _logger.LogWarning(
                    "Feature '{FeatureName}' not found. Skipping mapping for Role: {Role}.",
                    record.FeatureName,
                    record.Role
                );
                skippedCount++;
                continue;
            }

            foreach (Feature feature in featureList)
            {
                bool exists = await _context.RoleFeatureMapping.AnyAsync(x =>
                    x.RoleId == role.Id && x.FeatureId == feature.Id
                );

                if (exists)
                {
                    _logger.LogDebug(
                        "Role-feature mapping already exists. Skipping. Role: {Role}, Feature: {FeatureName}.",
                        record.Role,
                        record.FeatureName
                    );
                    skippedCount++;
                    continue;
                }

                await _context.RoleFeatureMapping.AddAsync(
                    new RoleFeatureMapping
                    {
                        RoleId = role.Id,
                        FeatureId = feature.Id,
                        IsActive = true,
                    }
                );
                addedCount++;
            }
        }

        _logger.LogInfo(
            "Role-feature mapping seeding complete. Added: {AddedCount}, Skipped: {SkippedCount}.",
            addedCount,
            skippedCount
        );
    }
}
