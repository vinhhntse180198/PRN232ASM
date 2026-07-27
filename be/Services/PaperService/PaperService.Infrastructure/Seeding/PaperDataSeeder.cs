using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PRN232ASM.PaperService.Domain.Entities;
using PRN232ASM.PaperService.Infrastructure.Persistence;

namespace PRN232ASM.PaperService.Infrastructure.Seeding;

public static class PaperDataSeeder
{
    private static readonly string[] Keywords =
    [
        "AI", "Machine Learning", "NLP", "Deep Learning", "Computer Vision",
        "Data Mining", "Big Data", "Neural Networks", "Reinforcement Learning", "Robotics"
    ];

    private static readonly string[] Topics =
    [
        "Artificial Intelligence", "Software Engineering", "Cybersecurity",
        "Cloud Computing", "Bioinformatics", "Quantum Computing"
    ];

    private static readonly string[] Journals =
    [
        "Nature Machine Intelligence", "IEEE Transactions on Pattern Analysis",
        "Journal of Artificial Intelligence Research", "ACM Computing Surveys",
        "Science Robotics", "Neural Computation", "Data & Knowledge Engineering"
    ];

    private static readonly string[] FirstNames =
    [
        "Alice", "Bob", "Carol", "David", "Eve", "Frank", "Grace", "Henry", "Ivy", "Jack"
    ];

    private static readonly string[] LastNames =
    [
        "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Wilson", "Taylor"
    ];

    private static readonly string[] TitlePrefixes =
    [
        "Advances in", "A Survey of", "Deep", "Efficient", "Robust", "Scalable", "Novel",
        "Interpretable", "Self-Supervised", "Federated"
    ];

    private static readonly string[] TitleSubjects =
    [
        "Transformers", "Graph Neural Networks", "Language Models", "Vision Transformers",
        "Knowledge Graphs", "Recommendation Systems", "Anomaly Detection", "Time Series Forecasting"
    ];

    public static async Task SeedAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PaperServiceDbContext>();

        await context.Database.EnsureCreatedAsync(cancellationToken);
        await PRN232ASM.BuildingBlocks.EventBus.Outbox.OutboxSchema.EnsureCreatedAsync(context, cancellationToken);

        if (await context.ResearchPapers.CountAsync(cancellationToken) > 0)
        {
            return;
        }

        var existingCount = await context.ResearchPapers.CountAsync(cancellationToken);
        if (existingCount > 0)
        {
            context.ResearchPapers.RemoveRange(context.ResearchPapers);
            context.PaperAuthors.RemoveRange(context.PaperAuthors);
            context.PaperKeywords.RemoveRange(context.PaperKeywords);
            context.PaperTopics.RemoveRange(context.PaperTopics);
            context.Bookmarks.RemoveRange(context.Bookmarks);
            context.Authors.RemoveRange(context.Authors);
            context.Keywords.RemoveRange(context.Keywords);
            context.ResearchTopics.RemoveRange(context.ResearchTopics);
            context.Journals.RemoveRange(context.Journals);
            await context.SaveChangesAsync(cancellationToken);
        }

        var journalEntities = Journals.Select((name, index) => new Journal
        {
            Id = DeterministicGuid($"journal-{index}"),
            Name = name,
            Issn = $"ISSN-{1000 + index:0000}-{2000 + index:0000}",
            Publisher = index % 2 == 0 ? "Springer" : "Elsevier"
        }).ToList();

        var keywordEntities = Keywords.Select((name, index) => new Keyword
        {
            Id = DeterministicGuid($"keyword-{index}"),
            Name = name
        }).ToList();

        var topicEntities = Topics.Select((name, index) => new ResearchTopic
        {
            Id = DeterministicGuid($"topic-{index}"),
            Name = name,
            Description = $"Research topic covering {name}"
        }).ToList();

        var authorEntities = new List<Author>();
        for (var i = 0; i < 50; i++)
        {
            authorEntities.Add(new Author
            {
                Id = DeterministicGuid($"author-{i}"),
                Name = $"{FirstNames[i % FirstNames.Length]} {LastNames[(i * 3) % LastNames.Length]}",
                Affiliation = $"University of Science {(i % 10) + 1}",
                Email = $"author{i}@research.edu"
            });
        }

        context.Journals.AddRange(journalEntities);
        context.Keywords.AddRange(keywordEntities);
        context.ResearchTopics.AddRange(topicEntities);
        context.Authors.AddRange(authorEntities);

        for (var i = 1; i <= 303; i++)
        {
            var journal = journalEntities[i % journalEntities.Count];
            var topic = topicEntities[i % topicEntities.Count];
            var year = 2020 + (i % 5);
            var paperId = DeterministicGuid($"paper-{i}");

            var paper = new ResearchPaper
            {
                Id = paperId,
                Title = $"{TitlePrefixes[i % TitlePrefixes.Length]} {TitleSubjects[i % TitleSubjects.Length]} for Scientific Discovery #{i}",
                Abstract = $"This paper presents a comprehensive study on {TitleSubjects[i % TitleSubjects.Length].ToLower()} " +
                           $"with applications in {topic.Name.ToLower()}. We propose a novel approach evaluated on benchmark datasets " +
                           $"achieving state-of-the-art results. Publication index: {i}.",
                Doi = $"10.1000/prn232.paper.{i:0000}",
                PublicationYear = year,
                CitationCount = (i * 7) % 500,
                JournalId = journal.Id,
                Journal = journal,
                CreatedAt = new DateTime(year, (i % 12) + 1, (i % 28) + 1, 0, 0, 0, DateTimeKind.Utc)
            };

            var authorCount = 2 + (i % 3);
            for (var a = 0; a < authorCount; a++)
            {
                var author = authorEntities[(i + a) % authorEntities.Count];
                paper.PaperAuthors.Add(new PaperAuthor
                {
                    PaperId = paperId,
                    AuthorId = author.Id,
                    AuthorOrder = a + 1,
                    Author = author
                });
            }

            var keywordCount = 2 + (i % 4);
            for (var k = 0; k < keywordCount; k++)
            {
                var keyword = keywordEntities[(i + k) % keywordEntities.Count];
                paper.PaperKeywords.Add(new PaperKeyword
                {
                    PaperId = paperId,
                    KeywordId = keyword.Id,
                    Keyword = keyword
                });
            }

            paper.PaperTopics.Add(new PaperTopic
            {
                PaperId = paperId,
                TopicId = topic.Id,
                Topic = topic
            });

            context.ResearchPapers.Add(paper);
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    private static Guid DeterministicGuid(string input)
    {
        var bytes = System.Security.Cryptography.MD5.HashData(System.Text.Encoding.UTF8.GetBytes(input));
        return new Guid(bytes);
    }
}
