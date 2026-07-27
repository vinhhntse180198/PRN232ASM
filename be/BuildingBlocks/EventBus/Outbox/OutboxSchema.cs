using Microsoft.EntityFrameworkCore;

namespace PRN232ASM.BuildingBlocks.EventBus.Outbox;

public static class OutboxSchema
{
    /// <summary>
    /// Ensures OutboxMessages exists on SQL Server even when the database was created before Outbox was added.
    /// </summary>
    public static async Task EnsureCreatedAsync(DbContext db, CancellationToken cancellationToken = default)
    {
        await db.Database.ExecuteSqlRawAsync(
            """
            IF OBJECT_ID(N'dbo.OutboxMessages', N'U') IS NULL
            BEGIN
                CREATE TABLE [dbo].[OutboxMessages] (
                    [Id] uniqueidentifier NOT NULL,
                    [TypeName] nvarchar(512) NOT NULL,
                    [Payload] nvarchar(max) NOT NULL,
                    [CreatedAt] datetime2 NOT NULL,
                    [ProcessedAt] datetime2 NULL,
                    [LastError] nvarchar(2000) NULL,
                    [RetryCount] int NOT NULL,
                    CONSTRAINT [PK_OutboxMessages] PRIMARY KEY ([Id])
                );
                CREATE INDEX [IX_OutboxMessages_ProcessedAt] ON [dbo].[OutboxMessages] ([ProcessedAt]);
                CREATE INDEX [IX_OutboxMessages_CreatedAt] ON [dbo].[OutboxMessages] ([CreatedAt]);
            END
            """,
            cancellationToken);
    }
}
