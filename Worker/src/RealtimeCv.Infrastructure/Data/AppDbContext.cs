using System;
using System.Collections.Generic;
using System.Linq;
using RealtimeCv.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace RealtimeCv.Infrastructure.Data;

public class AppDbContext : DbContext
{
  public DbSet<VisionSet>? VisionSet { get; set; }
  // public DbSet<UrlStatusHistory>? UrlStatusHistories { get; set; }

  public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
  {
  }


  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    // Auto-generate Id on creation
    modelBuilder.Entity<VisionSet>().Property(vs => vs.Id).ValueGeneratedOnAdd();

    // Convert non-supported formats
    var valueComparer = new ValueComparer<ICollection<string>>(
      (c1, c2) => c2 != null && c1 != null && c1.SequenceEqual(c2),
      c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
      c => c.ToList());
    
    modelBuilder.Entity<VisionSet>().Property(vs => vs.Sources).HasConversion(
      v => JsonConvert.SerializeObject(v),
      v => JsonConvert.DeserializeObject<List<string>>(v)!,
      valueComparer
    );
    modelBuilder.Entity<VisionSet>().Property(vs => vs.Models).HasConversion(
      v => JsonConvert.SerializeObject(v),
      v => JsonConvert.DeserializeObject<List<string>>(v)!,
      valueComparer
    );
    
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
  }
}
