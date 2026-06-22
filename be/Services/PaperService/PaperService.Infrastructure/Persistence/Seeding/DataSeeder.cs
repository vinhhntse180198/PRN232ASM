using Microsoft.EntityFrameworkCore;
using PaperService.Domain.Entities;

namespace PaperService.Infrastructure.Persistence.Seeding;

public class DataSeeder
{
    private readonly PaperServiceDbContext _context;

    public DataSeeder(PaperServiceDbContext context) => _context = context;

    public async Task SeedAsync()
    {
        if (await _context.ResearchPapers.AnyAsync())
            return;

        var journalNature = new Journal
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111101"),
            Name = "Nature Machine Intelligence",
            Issn = "2522-5839",
            Publisher = "Nature Publishing Group"
        };

        var journalIeee = new Journal
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111102"),
            Name = "IEEE Transactions on Neural Networks",
            Issn = "2162-237X",
            Publisher = "IEEE"
        };

        var topicAi = new ResearchTopic
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222201"),
            Name = "Artificial Intelligence",
            Description = "AI research and applications"
        };

        var topicNlp = new ResearchTopic
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222202"),
            Name = "Natural Language Processing",
            Description = "Language models and text understanding"
        };

        var kwMl = new Keyword { Id = Guid.Parse("33333333-3333-3333-3333-333333333301"), Name = "machine learning" };
        var kwDl = new Keyword { Id = Guid.Parse("33333333-3333-3333-3333-333333333302"), Name = "deep learning" };
        var kwLlm = new Keyword { Id = Guid.Parse("33333333-3333-3333-3333-333333333303"), Name = "large language models" };
        var kwTrend = new Keyword { Id = Guid.Parse("33333333-3333-3333-3333-333333333304"), Name = "publication trends" };

        var author1 = new Author
        {
            Id = Guid.Parse("44444444-4444-4444-4444-444444444401"),
            Name = "Yann LeCun",
            Affiliation = "Meta AI / NYU"
        };

        var author2 = new Author
        {
            Id = Guid.Parse("44444444-4444-4444-4444-444444444402"),
            Name = "Fei-Fei Li",
            Affiliation = "Stanford University"
        };

        var author3 = new Author
        {
            Id = Guid.Parse("44444444-4444-4444-4444-444444444403"),
            Name = "Andrew Ng",
            Affiliation = "Stanford University / DeepLearning.AI"
        };

        var paper1 = new ResearchPaper
        {
            Id = Guid.Parse("55555555-5555-5555-5555-555555555501"),
            JournalId = journalNature.Id,
            Title = "Self-supervised learning for scientific publication trend analysis",
            Abstract = "We propose a framework for tracking emerging research topics using self-supervised representations over paper metadata and citation graphs.",
            Doi = "10.1038/s42256-024-00001-1",
            PublishedYear = 2024,
            PublishedDate = new DateOnly(2024, 3, 15),
            CitationCount = 128,
            Url = "https://doi.org/10.1038/s42256-024-00001-1"
        };

        var paper2 = new ResearchPaper
        {
            Id = Guid.Parse("55555555-5555-5555-5555-555555555502"),
            JournalId = journalIeee.Id,
            Title = "Transformer architectures for academic keyword extraction",
            Abstract = "This paper evaluates transformer models for extracting keywords and topics from scientific abstracts across computer science domains.",
            Doi = "10.1109/TNNLS.2023.00002-2",
            PublishedYear = 2023,
            PublishedDate = new DateOnly(2023, 11, 8),
            CitationCount = 89,
            Url = "https://doi.org/10.1109/TNNLS.2023.00002-2"
        };

        var paper3 = new ResearchPaper
        {
            Id = Guid.Parse("55555555-5555-5555-5555-555555555503"),
            JournalId = journalNature.Id,
            Title = "Visualizing research momentum with temporal embedding models",
            Abstract = "We introduce temporal embedding methods to visualize how research fields evolve using open bibliographic metadata from OpenAlex and Semantic Scholar.",
            Doi = "10.1038/s42256-2022-00003-3",
            PublishedYear = 2022,
            PublishedDate = new DateOnly(2022, 6, 21),
            CitationCount = 210,
            Url = "https://doi.org/10.1038/s42256-2022-00003-3"
        };

        _context.Journals.AddRange(journalNature, journalIeee);
        _context.ResearchTopics.AddRange(topicAi, topicNlp);
        _context.Keywords.AddRange(kwMl, kwDl, kwLlm, kwTrend);
        _context.Authors.AddRange(author1, author2, author3);
        _context.ResearchPapers.AddRange(paper1, paper2, paper3);

        _context.PaperAuthors.AddRange(
            new PaperAuthor { PaperId = paper1.Id, AuthorId = author1.Id, AuthorOrder = 1 },
            new PaperAuthor { PaperId = paper1.Id, AuthorId = author3.Id, AuthorOrder = 2 },
            new PaperAuthor { PaperId = paper2.Id, AuthorId = author2.Id, AuthorOrder = 1 },
            new PaperAuthor { PaperId = paper2.Id, AuthorId = author1.Id, AuthorOrder = 2 },
            new PaperAuthor { PaperId = paper3.Id, AuthorId = author3.Id, AuthorOrder = 1 },
            new PaperAuthor { PaperId = paper3.Id, AuthorId = author2.Id, AuthorOrder = 2 }
        );

        _context.PaperKeywords.AddRange(
            new PaperKeyword { PaperId = paper1.Id, KeywordId = kwMl.Id },
            new PaperKeyword { PaperId = paper1.Id, KeywordId = kwTrend.Id },
            new PaperKeyword { PaperId = paper2.Id, KeywordId = kwDl.Id },
            new PaperKeyword { PaperId = paper2.Id, KeywordId = kwLlm.Id },
            new PaperKeyword { PaperId = paper3.Id, KeywordId = kwMl.Id },
            new PaperKeyword { PaperId = paper3.Id, KeywordId = kwTrend.Id }
        );

        _context.PaperTopics.AddRange(
            new PaperTopic { PaperId = paper1.Id, TopicId = topicAi.Id },
            new PaperTopic { PaperId = paper2.Id, TopicId = topicNlp.Id },
            new PaperTopic { PaperId = paper3.Id, TopicId = topicAi.Id }
        );

        await _context.SaveChangesAsync();
    }
}
