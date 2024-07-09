using AMChat.Core.Entities;
using AMChat.Infrastructure.Persistence.Constraints;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AMChat.Infrastructure.Persistence.Configuration;

public class MessageConfiguration : BaseEntityConfiguration<Message>
{
    public override void Configure(EntityTypeBuilder<Message> builder)
    {
        base.Configure(builder);

        builder.Property(message => message.Text)
            .HasMaxLength(DataSchemeConstraints.MessageTextLength)
            .IsRequired();

        builder.ToTable(table =>
        {
            table.HasCheckConstraint(CheckConstraints.MessagePostedDateNotInFuture.Name,
                                     CheckConstraints.MessagePostedDateNotInFuture.SqlCode);
        });
    }
}
