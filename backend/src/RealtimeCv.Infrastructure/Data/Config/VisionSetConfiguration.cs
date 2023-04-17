using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealtimeCv.Core.Entities;

namespace RealtimeCv.Infrastructure.Data.Config;

public class VisionSetConfiguration : IEntityTypeConfiguration<VisionSet>
{
    public void Configure(EntityTypeBuilder<VisionSet> builder)
    {
        builder.ToTable(nameof(VisionSet));

        builder.Property(vs => vs.Id)
          .IsRequired();
        builder.Property(vs => vs.Name)
          .IsRequired()
          .HasMaxLength(Constants.DefaultMaxStringLength);
        builder.Property(vs => vs.Sources)
          .IsRequired();
    }
}
