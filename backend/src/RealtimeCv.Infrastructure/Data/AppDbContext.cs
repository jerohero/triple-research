using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using RealtimeCv.Core.Entities;
using RealtimeCv.Infrastructure.Data.Config;

namespace RealtimeCv.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public DbSet<VisionSet>? VisionSet { get; set; }
    public DbSet<Project>? Project { get; set; }
    public DbSet<Session>? Session { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Auto-generate Id on creation
        builder.Entity<VisionSet>().Property(vs => vs.Id).ValueGeneratedOnAdd();
        builder.Entity<Project>().Property(p => p.Id).ValueGeneratedOnAdd();
        builder.Entity<Session>().Property(p => p.Id).ValueGeneratedOnAdd();
        
        // Set entity relations
        builder.Entity<Project>()
            .HasMany(p => p.VisionSets)
            .WithOne(vs => vs.Project)
            .HasForeignKey(vs => vs.ProjectId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
        
        builder.Entity<VisionSet>()
            .HasMany(vs => vs.Sessions)
            .WithOne(s => s.VisionSet)
            .HasForeignKey(vs => vs.VisionSetId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        // Convert non-supported formats
        var valueComparer = new ValueComparer<ICollection<string>>(
          (c1, c2) => c2 != null && c1 != null && c1.SequenceEqual(c2),
          c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
          c => c.ToList());

        builder.Entity<VisionSet>().Property(vs => vs.Sources).HasConversion(
          v => JsonConvert.SerializeObject(v),
          v => JsonConvert.DeserializeObject<List<string>>(v)!,
          valueComparer
        );

        // Apply configurations
        builder.ApplyConfiguration(new VisionSetConfiguration());
        builder.ApplyConfiguration(new ProjectConfiguration());
        builder.ApplyConfiguration(new SessionConfiguration());

        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
