using AMChat.Core;
using AMChat.Core.Entities;
using AMChat.Infrastructure.Persistence.Constraints;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AMChat.Infrastructure.Persistence.Configuration;

public class ProfileConfiguration : BaseEntityConfiguration<Profile>
{
    public override void Configure(EntityTypeBuilder<Profile> builder)
    {
        base.Configure(builder);

        builder.Property(profile => profile.Fullname)
            .HasMaxLength(DomainConstraints.FullnameLength);

        builder.Property(profile => profile.Description)
            .HasMaxLength(DomainConstraints.ProfileDescription)
            .IsRequired(false);

        builder.ToTable(table =>
        {
            table.HasCheckConstraint(CheckConstraints.ProfileBirthdateMoreThanMinimumAge.Name,
                                     CheckConstraints.ProfileBirthdateMoreThanMinimumAge.SqlCode);
        });
    }
}
