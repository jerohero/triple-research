using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealtimeCv.Core.Entities;

namespace RealtimeCv.Infrastructure.Data.Config;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable(nameof(Project));

        builder.Property(vs => vs.Id)
          .IsRequired();
        builder.Property(vs => vs.Name)
          .IsRequired()
          .HasMaxLength(Constants.DEFAULT_MAX_STRING_LENGTH);
    }
}
