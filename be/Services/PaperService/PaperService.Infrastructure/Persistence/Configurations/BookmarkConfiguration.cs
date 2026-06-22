using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaperService.Domain.Entities;

namespace PaperService.Infrastructure.Persistence.Configurations;

public class BookmarkConfiguration : IEntityTypeConfiguration<Bookmark>
{
    public void Configure(EntityTypeBuilder<Bookmark> builder)
    {
        builder.ToTable("bookmarks");
        builder.HasKey(b => b.Id);
        builder.HasIndex(b => new { b.UserId, b.PaperId }).IsUnique();
        builder.HasOne(b => b.Paper).WithMany(p => p.Bookmarks).HasForeignKey(b => b.PaperId);
    }
}
