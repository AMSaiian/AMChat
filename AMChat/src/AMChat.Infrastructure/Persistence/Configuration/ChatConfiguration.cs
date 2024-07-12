using AMChat.Core;
using AMChat.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AMChat.Infrastructure.Persistence.Configuration;

public class ChatConfiguration : BaseEntityConfiguration<Chat>
{
    public override void Configure(EntityTypeBuilder<Chat> builder)
    {
        base.Configure(builder);

        builder.Property(chat => chat.Name)
            .HasMaxLength(DomainConstraints.ChatNameLength);

        builder.Property(chat => chat.Description)
            .HasMaxLength(DomainConstraints.ChatDescriptionLength)
            .IsRequired(false);

        builder.HasMany(chat => chat.Messages)
            .WithOne(message => message.Chat)
            .HasForeignKey(message => message.ChatId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.HasMany(chat => chat.JoinedUsers)
            .WithMany(user => user.JoinedChats);
    }
}
