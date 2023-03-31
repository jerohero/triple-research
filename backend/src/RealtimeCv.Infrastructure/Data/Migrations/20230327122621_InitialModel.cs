﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealtimeCv.Infrastructure.Data.Migrations;

/// <inheritdoc />
public partial class InitialModel : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Project",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Project", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "VisionSet",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Sources = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Models = table.Column<string>(type: "nvarchar(max)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_VisionSet", x => x.Id);
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Project");

        migrationBuilder.DropTable(
            name: "VisionSet");
    }
}