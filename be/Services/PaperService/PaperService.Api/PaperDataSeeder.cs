using PaperService.Application.DTOs;
using PaperService.Application.Interfaces;
using PaperService.Domain.Entities;
using PaperService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using PRN232ASM.Shared;

namespace PaperService.Api;

internal static class PaperDataSeeder
{
    public static async Task SeedAsync(PaperDbContext db, IPaperService paperService)
    {
        if (await db.ResearchPapers.AnyAsync()) return;

        var samples = new[]
        {
            new CreatePaperRequest
            {
                Title = "Attention Is All You Need",
                Abstract = "The dominant sequence transduction models are based on complex recurrent or convolutional neural networks. We propose the Transformer architecture.",
                Doi = "10.48550/arXiv.1706.03762",
                JournalName = "NeurIPS",
                PublishedYear = 2017,
                AuthorNames = ["Ashish Vaswani", "Noam Shazeer", "Niki Parmar"],
                Keywords = ["transformer", "deep learning", "nlp"]
            },
            new CreatePaperRequest
            {
                Title = "BERT: Pre-training of Deep Bidirectional Transformers for Language Understanding",
                Abstract = "We introduce BERT, which stands for Bidirectional Encoder Representations from Transformers.",
                Doi = "10.18653/v1/N19-1423",
                JournalName = "NAACL",
                PublishedYear = 2019,
                AuthorNames = ["Jacob Devlin", "Ming-Wei Chang", "Kenton Lee"],
                Keywords = ["bert", "language model", "nlp"]
            },
            new CreatePaperRequest
            {
                Title = "Deep Residual Learning for Image Recognition",
                Abstract = "We present a residual learning framework to ease the training of networks that are substantially deeper than those used previously.",
                Doi = "10.1109/CVPR.2016.90",
                JournalName = "CVPR",
                PublishedYear = 2016,
                AuthorNames = ["Kaiming He", "Xiangyu Zhang", "Shaoqing Ren"],
                Keywords = ["resnet", "computer vision", "deep learning"]
            },
            new CreatePaperRequest
            {
                Title = "Climate Change 2023: Synthesis Report",
                Abstract = "This Synthesis Report addresses the global challenge of climate change and mitigation pathways.",
                Doi = "10.59327/IPCC/AR6-001",
                JournalName = "IPCC",
                PublishedYear = 2023,
                AuthorNames = ["IPCC Working Group"],
                Keywords = ["climate science", "sustainability"]
            }
        };

        foreach (var sample in samples)
            await paperService.CreateAsync(sample);
    }

    public static async Task EnsureSqliteSchemaAsync(PaperDbContext db)
    {
        if (SupabaseConnectionHelper.UseSupabaseDatabase()) return;

        try
        {
            await db.Database.ExecuteSqlRawAsync("SELECT 1 FROM journals LIMIT 1");
            try
            {
                await db.Database.ExecuteSqlRawAsync("SELECT IsOpenAccess FROM research_papers LIMIT 1");
            }
            catch
            {
                await db.Database.ExecuteSqlRawAsync(
                    "ALTER TABLE research_papers ADD COLUMN IsOpenAccess INTEGER NOT NULL DEFAULT 0");
            }
        }
        catch
        {
            await db.Database.EnsureDeletedAsync();
            await db.Database.EnsureCreatedAsync();
        }
    }
}
