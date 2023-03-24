using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealtimeCv.Core.Entities;

namespace RealtimeCv.Infrastructure.Data.Configurations;

public class VisionSetConfiguration : IEntityTypeConfiguration<VisionSet>
{
  public void Configure(EntityTypeBuilder<VisionSet> builder)
  {
    builder.ToTable(nameof(VisionSet));

    builder.HasKey(x => x.Id);
  }
}
