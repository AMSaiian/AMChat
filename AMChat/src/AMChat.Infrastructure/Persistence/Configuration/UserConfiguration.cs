using AMChat.Core;
using AMChat.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AMChat.Infrastructure.Persistence.Configuration;

public class UserConfiguration : BaseEntityConfiguration<User>
{
    public override void Configure(EntityTypeBuilder<User> builder)
    {
        base.Configure(builder);

        builder.HasIndex(user => user.Name)
            .IsUnique();
        builder.Property(user => user.Name)
            .HasMaxLength(DomainConstraints.UsernameLength);

        builder.HasOne(user => user.Profile)
            .WithOne(profile => profile.User)
            .HasForeignKey<Profile>(profile => profile.UserId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasMany(user => user.OwnedChats)
            .WithOne(chat => chat.Owner)
            .HasForeignKey(chat => chat.OwnerId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasMany(user => user.Messages)
            .WithOne(message => message.Author)
            .HasForeignKey(message => message.AuthorId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);
    }
}
