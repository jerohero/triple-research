using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealtimeCv.Core.Entities;

namespace RealtimeCv.Infrastructure.Data.Config;

public class SessionConfiguration : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> builder)
    {
        builder.ToTable(nameof(Session));

        builder.Property(s => s.Id)
          .IsRequired();
        builder.Property(s => s.IsActive)
            .IsRequired();
        builder.Property(s => s.StartedAt)
            .IsRequired();
        builder.Property(s => s.StoppedAt)
            .IsRequired();
        builder.Property(s => s.VisionSetId)
            .IsRequired();
    }
}
