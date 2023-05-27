using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealtimeCv.Core.Entities;

namespace RealtimeCv.Infrastructure.Data.Config;

public class TrainedModelConfiguration : IEntityTypeConfiguration<TrainedModel>
{
    public void Configure(EntityTypeBuilder<TrainedModel> builder)
    {
        builder.ToTable(nameof(TrainedModel));

        builder.Property(vs => vs.Id)
            .IsRequired();
        builder.Property(vs => vs.Name)
            .IsRequired()
            .HasMaxLength(Constants.DefaultMaxStringLength);
        builder.Property(vs => vs.ProjectId)
            .IsRequired();
    }
}
