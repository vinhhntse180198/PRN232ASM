using Microsoft.EntityFrameworkCore;

namespace PRN232ASM.BuildingBlocks.EventBus.Outbox;

public static class OutboxModelBuilderExtensions
{
    public static void ConfigureOutboxMessages(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OutboxMessage>(entity =>
        {
            entity.ToTable("OutboxMessages");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.TypeName).HasMaxLength(512).IsRequired();
            entity.Property(x => x.Payload).IsRequired();
            entity.Property(x => x.LastError).HasMaxLength(2000);
            entity.HasIndex(x => x.ProcessedAt);
            entity.HasIndex(x => x.CreatedAt);
        });
    }
}
