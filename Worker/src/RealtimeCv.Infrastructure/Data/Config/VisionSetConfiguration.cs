using RealtimeCv.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RealtimeCv.Infrastructure.Data.Config;

public class VisionSetConfiguration : IEntityTypeConfiguration<VisionSet>
{
  public void Configure(EntityTypeBuilder<VisionSet> builder)
  {
    builder.Property(vs => vs.Id)
      .IsRequired();
    builder.Property(vs => vs.Name)
      .IsRequired()
      .HasMaxLength(Constants.DEFAULT_MAX_STRING_LENGTH);
    builder.Property(vs => vs.Models)
      .IsRequired();
    builder.Property(vs => vs.Sources)
      .IsRequired();
  }
}
