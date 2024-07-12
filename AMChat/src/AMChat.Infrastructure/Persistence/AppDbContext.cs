using System.Reflection;
using AMChat.Application.Common.Interfaces;
using AMChat.Core.Entities;
using AMChat.Infrastructure.Persistence.Constraints;
using Microsoft.EntityFrameworkCore;

namespace AMChat.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options),
                                                                    IAppDbContext
{
    public DbSet<Chat> Chats { get; set; } = default!;

    public DbSet<Profile> Profiles { get; set; } = default!;

    public DbSet<User> Users { get; set; } = default!;

    public DbSet<Message> Messages { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasPostgresExtension(DataSchemeConstraints.KeyGenerationExtensionName);
        modelBuilder.HasDefaultSchema(DataSchemeConstraints.SchemeName);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
