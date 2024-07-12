using AMChat.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace AMChat.Application.Common.Interfaces;

public interface IAppDbContext
{
    public DbSet<Chat> Chats { get; set; }

    public DbSet<Profile> Profiles { get; set; }

    public DbSet<User> Users { get; set; }

    public DbSet<Message> Messages { get; set; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
