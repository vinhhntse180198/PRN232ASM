using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PRN232ASM.AuthService.Application.Interfaces;
using PRN232ASM.AuthService.Domain.Entities;

namespace PRN232ASM.AuthService.Infrastructure.Persistence;

public static class DataSeeder
{
    private static readonly Guid AdminRoleId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid ResearcherRoleId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private static readonly Guid StudentRoleId = Guid.Parse("33333333-3333-3333-3333-333333333333");

    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<AuthDbContext>>();

        await context.Database.EnsureCreatedAsync();

        if (!await context.Roles.AnyAsync())
        {
            var roles = new[]
            {
                new Role { Id = AdminRoleId, Name = Role.Admin },
                new Role { Id = ResearcherRoleId, Name = Role.Researcher },
                new Role { Id = StudentRoleId, Name = Role.Student }
            };

            await context.Roles.AddRangeAsync(roles);
            await context.SaveChangesAsync();
            logger.LogInformation("Seeded default roles.");
        }

        if (!await context.Users.AnyAsync())
        {
            var users = new[]
            {
                new User
                {
                    Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    Email = "admin@prn232.asm",
                    FullName = "System Administrator",
                    PasswordHash = passwordHasher.Hash("Admin@123"),
                    RoleId = AdminRoleId,
                    CreatedAt = DateTime.UtcNow
                },
                new User
                {
                    Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                    Email = "user@prn232.asm",
                    FullName = "Default Student",
                    PasswordHash = passwordHasher.Hash("User@123"),
                    RoleId = StudentRoleId,
                    CreatedAt = DateTime.UtcNow
                }
            };

            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();
            logger.LogInformation("Seeded default users.");
        }
    }
}
