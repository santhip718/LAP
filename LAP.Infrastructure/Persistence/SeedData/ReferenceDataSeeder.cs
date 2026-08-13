using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using LAP.Application.Interface;
using LAP.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace LAP.Infrastructure.Persistence.SeedData;

public class ReferenceDataSeeder
{
    private readonly LearningAssessmentDbContext _context;
    private readonly ICustomLogger<ReferenceDataSeeder> _logger;

    public ReferenceDataSeeder(
        LearningAssessmentDbContext context,
        ICustomLogger<ReferenceDataSeeder> logger
    )
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        _logger.LogInfo("Starting reference data seeding...");

        CsvConfiguration config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HeaderValidated = null,
            MissingFieldFound = null,
        };

        await SeedRefSetsAsync(config);
        await _context.SaveChangesAsync();

        await SeedRefTermsAsync(config);
        await _context.SaveChangesAsync();

        _logger.LogInfo("Reference data seeding completed.");
    }

    private async Task SeedRefSetsAsync(CsvConfiguration config)
    {
        string filePath = Path.Combine(
            AppContext.BaseDirectory,
            "Persistence",
            "SeedData",
            "RefSet.csv"
        );

        if (!File.Exists(filePath))
        {
            _logger.LogWarning(
                "RefSet.csv not found at {FilePath}. Skipping RefSet seeding.",
                filePath
            );
            return;
        }

        using StreamReader reader = new StreamReader(filePath);
        using CsvReader csv = new CsvReader(reader, config);

        List<RefSetSeedModel> recordList = csv.GetRecords<RefSetSeedModel>().ToList();
        _logger.LogInfo("Found {Count} RefSet records in CSV.", recordList.Count);

        int addedCount = 0;
        foreach (RefSetSeedModel record in recordList)
        {
            bool exists = await _context.RefSet.AnyAsync(x => x.Name == record.Name);

            if (exists)
            {
                _logger.LogDebug("RefSet already exists. Skipping. Name: {Name}.", record.Name);
                continue;
            }

            await _context.RefSet.AddAsync(
                new RefSet { Name = record.Name, Description = record.Description }
            );
            addedCount++;
        }

        _logger.LogInfo(
            "RefSet seeding complete. Added: {AddedCount}, Skipped: {SkippedCount}.",
            addedCount,
            recordList.Count - addedCount
        );
    }

    private async Task SeedRefTermsAsync(CsvConfiguration config)
    {
        string filePath = Path.Combine(
            AppContext.BaseDirectory,
            "Persistence",
            "SeedData",
            "RefTerm.csv"
        );

        if (!File.Exists(filePath))
        {
            _logger.LogWarning(
                "RefTerm.csv not found at {FilePath}. Skipping RefTerm seeding.",
                filePath
            );
            return;
        }

        using StreamReader reader = new StreamReader(filePath);
        using CsvReader csv = new CsvReader(reader, config);

        List<RefTermSeedModel> recordList = csv.GetRecords<RefTermSeedModel>().ToList();
        _logger.LogInfo("Found {Count} RefTerm records in CSV.", recordList.Count);

        int addedCount = 0;
        int skippedCount = 0;
        foreach (RefTermSeedModel record in recordList)
        {
            RefSet? refSet = await _context.RefSet.FirstOrDefaultAsync(x =>
                x.Name == record.RefSetName
            );

            if (refSet == null)
            {
                _logger.LogWarning(
                    "RefSet '{RefSetName}' not found. Skipping RefTerm: {Name}.",
                    record.RefSetName,
                    record.Name
                );
                skippedCount++;
                continue;
            }

            bool exists = await _context.RefTerm.AnyAsync(x =>
                x.RefSetId == refSet.Id && x.Name == record.Name
            );

            if (exists)
            {
                _logger.LogDebug(
                    "RefTerm already exists. Skipping. Name: {Name}, RefSet: {RefSetName}.",
                    record.Name,
                    record.RefSetName
                );
                skippedCount++;
                continue;
            }

            await _context.RefTerm.AddAsync(
                new RefTerm
                {
                    RefSetId = refSet.Id,
                    Name = record.Name,
                    Description = record.Description,
                }
            );
            addedCount++;
        }

        _logger.LogInfo(
            "RefTerm seeding complete. Added: {AddedCount}, Skipped: {SkippedCount}.",
            addedCount,
            skippedCount
        );
    }
}
