using AMChat.Core.Entities;
using AMChat.Infrastructure.Persistence.Constraints;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AMChat.Infrastructure.Persistence.Configuration;

public abstract class BaseEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : BaseEntity
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.Property(ent => ent.Id)
            .HasColumnType(DataSchemeConstraints.KeyType)
            .HasDefaultValueSql(DataSchemeConstraints.KeyTypeDefaultValue);

        builder.HasKey(ent => ent.Id);
    }
}
