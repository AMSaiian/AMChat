using AMChat.Core.Entities;
using AMChat.Infrastructure.Persistence.Constraints;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AMChat.Infrastructure.Persistence.Configuration;

public class ChatConfiguration : BaseEntityConfiguration<Chat>
{
    public override void Configure(EntityTypeBuilder<Chat> builder)
    {
        base.Configure(builder);

        builder.Property(chat => chat.Name)
            .HasMaxLength(DataSchemeConstraints.ChatNameLength);

        builder.Property(chat => chat.Description)
            .HasMaxLength(DataSchemeConstraints.ChatDescriptionLength)
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
