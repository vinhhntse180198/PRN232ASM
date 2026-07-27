using Microsoft.EntityFrameworkCore;
using NotificationService.Domain.Entities;
using PRN232ASM.BuildingBlocks.EventBus.Outbox;

namespace NotificationService.Infrastructure.Persistence;

public class NotificationDbContext : DbContext
{
    public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options) { }

    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<FollowTopic> FollowTopics => Set<FollowTopic>();
    public DbSet<FollowKeyword> FollowKeywords => Set<FollowKeyword>();
    public DbSet<FollowJournal> FollowJournals => Set<FollowJournal>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.ToTable("Notifications");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Title).HasMaxLength(500).IsRequired();
            entity.Property(x => x.Message).HasMaxLength(2000).IsRequired();
            entity.Property(x => x.Type).HasMaxLength(100).IsRequired();
            entity.HasIndex(x => x.UserId);
        });

        modelBuilder.Entity<FollowTopic>(entity =>
        {
            entity.ToTable("FollowTopics");
            entity.HasKey(x => new { x.UserId, x.TopicId });
        });

        modelBuilder.Entity<FollowKeyword>(entity =>
        {
            entity.ToTable("FollowKeywords");
            entity.HasKey(x => new { x.UserId, x.KeywordId });
        });

        modelBuilder.Entity<FollowJournal>(entity =>
        {
            entity.ToTable("FollowJournals");
            entity.HasKey(x => new { x.UserId, x.JournalId });
        });

        modelBuilder.ConfigureOutboxMessages();
    }
}
