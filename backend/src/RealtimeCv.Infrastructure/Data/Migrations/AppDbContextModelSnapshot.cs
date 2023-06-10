﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RealtimeCv.Infrastructure.Data;

#nullable disable

namespace RealtimeCv.Infrastructure.Data.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("RealtimeCv.Core.Entities.Project", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.ToTable("Project", (string)null);
                });

            modelBuilder.Entity("RealtimeCv.Core.Entities.Session", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("Pod")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Source")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("StartedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("StoppedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("VisionSetId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("VisionSetId");

                    b.ToTable("Session", (string)null);
                });

            modelBuilder.Entity("RealtimeCv.Core.Entities.TrainedModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("IsUploadFinished")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("ProjectId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.ToTable("TrainedModel", (string)null);
                });

            modelBuilder.Entity("RealtimeCv.Core.Entities.VisionSet", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ContainerImage")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("ProjectId")
                        .HasColumnType("int");

                    b.Property<string>("Sources")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("TrainedModelId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.HasIndex("TrainedModelId");

                    b.ToTable("VisionSet", (string)null);
                });

            modelBuilder.Entity("RealtimeCv.Core.Entities.Session", b =>
                {
                    b.HasOne("RealtimeCv.Core.Entities.VisionSet", "VisionSet")
                        .WithMany("Sessions")
                        .HasForeignKey("VisionSetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("VisionSet");
                });

            modelBuilder.Entity("RealtimeCv.Core.Entities.TrainedModel", b =>
                {
                    b.HasOne("RealtimeCv.Core.Entities.Project", "Project")
                        .WithMany("TrainedModels")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Project");
                });

            modelBuilder.Entity("RealtimeCv.Core.Entities.VisionSet", b =>
                {
                    b.HasOne("RealtimeCv.Core.Entities.Project", "Project")
                        .WithMany("VisionSets")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RealtimeCv.Core.Entities.TrainedModel", "TrainedModel")
                        .WithMany("VisionSets")
                        .HasForeignKey("TrainedModelId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.Navigation("Project");

                    b.Navigation("TrainedModel");
                });

            modelBuilder.Entity("RealtimeCv.Core.Entities.Project", b =>
                {
                    b.Navigation("TrainedModels");

                    b.Navigation("VisionSets");
                });

            modelBuilder.Entity("RealtimeCv.Core.Entities.TrainedModel", b =>
                {
                    b.Navigation("VisionSets");
                });

            modelBuilder.Entity("RealtimeCv.Core.Entities.VisionSet", b =>
                {
                    b.Navigation("Sessions");
                });
#pragma warning restore 612, 618
        }
    }
}
