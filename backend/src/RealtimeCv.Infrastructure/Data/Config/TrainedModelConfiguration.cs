using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealtimeCv.Core.Entities;

namespace RealtimeCv.Infrastructure.Data.Config;

public class TrainedModelConfiguration : IEntityTypeConfiguration<TrainedModel>
{
    public void Configure(EntityTypeBuilder<TrainedModel> builder)
    {
        builder.ToTable(nameof(TrainedModel));

        builder.Property(tm => tm.Id)
            .IsRequired();
        builder.Property(tm => tm.Name)
            .IsRequired()
            .HasMaxLength(Constants.DefaultMaxStringLength);
        builder.Property(tm => tm.IsUploadFinished)
            .IsRequired();
        builder.Property(tm => tm.ProjectId)
            .IsRequired();
    }
}
