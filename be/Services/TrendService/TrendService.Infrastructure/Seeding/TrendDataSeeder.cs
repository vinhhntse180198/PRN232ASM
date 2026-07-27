using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PRN232ASM.BuildingBlocks.EventBus.Outbox;
using PRN232ASM.TrendService.Domain.Entities;
using TrendServiceDbContext = PRN232ASM.TrendService.Infrastructure.Persistence.TrendServiceDbContext;

namespace PRN232ASM.TrendService.Infrastructure.Seeding;

public static class TrendDataSeeder
{
    private const string PaperDbConnectionString =
        "Server=sqlserver,1433;Database=PaperDb;User Id=sa;Password=Prn232_Sql_Strong!2026;TrustServerCertificate=True;Encrypt=False;";

    public static async Task SeedAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TrendServiceDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<TrendServiceDbContext>>();

        await context.Database.EnsureCreatedAsync(cancellationToken);
        await OutboxSchema.EnsureCreatedAsync(context, cancellationToken);

        if (await context.PublicationTrends.AnyAsync(cancellationToken))
        {
            logger.LogInformation("TrendService data already seeded — skipping.");
            return;
        }

        logger.LogInformation("TrendService: seeding PublicationTrends from PaperDb...");

        var trends = new List<PublicationTrend>();

        try
        {
            await using var paperConn = new SqlConnection(PaperDbConnectionString);
            await paperConn.OpenAsync(cancellationToken);

            await using var cmd = paperConn.CreateCommand();
            cmd.CommandText = @"
                SELECT
                    k.Name,
                    rp.PublicationYear,
                    COUNT(*) AS PaperCount
                FROM ResearchPapers rp
                INNER JOIN PaperKeywords pk ON pk.PaperId = rp.Id
                INNER JOIN Keywords k ON k.Id = pk.KeywordId
                GROUP BY k.Name, rp.PublicationYear
                ORDER BY PaperCount DESC";

            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                var keyword = reader.GetString(0);
                var year = reader.GetInt32(1);
                var count = reader.GetInt32(2);

                trends.Add(new PublicationTrend
                {
                    Id = Guid.NewGuid(),
                    Keyword = keyword,
                    Year = year,
                    PaperCount = count,
                    TopicId = null
                });
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Could not read PaperDb directly — falling back to static seed data.");
            trends = BuildStaticTrends();
        }

        if (trends.Count == 0)
        {
            logger.LogWarning("No trends from PaperDb — using static fallback data.");
            trends = BuildStaticTrends();
        }

        await context.PublicationTrends.AddRangeAsync(trends, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Seeded {Count} PublicationTrend rows.", trends.Count);

        if (!await context.DashboardReports.AnyAsync(cancellationToken))
        {
            var topKeyword = trends
                .GroupBy(t => t.Keyword)
                .Select(g => new { Keyword = g.Key, Count = g.Sum(x => x.PaperCount) })
                .OrderByDescending(x => x.Count)
                .FirstOrDefault();

            var report = new DashboardReport
            {
                Id = Guid.NewGuid(),
                ReportDate = DateOnly.FromDateTime(DateTime.UtcNow),
                TotalPapers = trends.Sum(t => t.PaperCount),
                TopKeyword = topKeyword?.Keyword ?? "N/A",
                GeneratedAt = DateTime.UtcNow
            };

            context.DashboardReports.Add(report);
            await context.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Seeded initial DashboardReport.");
        }
    }

    private static List<PublicationTrend> BuildStaticTrends()
    {
        var keywordCounts = new Dictionary<string, int[]>
        {
            ["AI"] = [22, 28, 35, 30, 25],
            ["Machine Learning"] = [20, 25, 32, 28, 22],
            ["NLP"] = [15, 20, 26, 24, 18],
            ["Deep Learning"] = [18, 23, 30, 27, 20],
            ["Computer Vision"] = [14, 18, 24, 21, 16],
            ["Data Mining"] = [12, 16, 20, 18, 14],
            ["Big Data"] = [10, 14, 18, 16, 12],
            ["Neural Networks"] = [16, 21, 27, 24, 19],
            ["Reinforcement Learning"] = [8, 12, 16, 14, 11],
            ["Robotics"] = [9, 13, 17, 15, 12]
        };

        var trends = new List<PublicationTrend>();
        var years = new[] { 2020, 2021, 2022, 2023, 2024 };

        foreach (var (keyword, counts) in keywordCounts)
        {
            for (var i = 0; i < years.Length; i++)
            {
                if (counts[i] > 0)
                {
                    trends.Add(new PublicationTrend
                    {
                        Id = Guid.NewGuid(),
                        Keyword = keyword,
                        Year = years[i],
                        PaperCount = counts[i],
                        TopicId = null
                    });
                }
            }
        }

        return trends;
    }
}
