using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SmartAgroPlan.DAL.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.CreateTable(
                name: "Soils",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    WaterRetention = table.Column<double>(type: "double precision", nullable: false),
                    Acidity = table.Column<double>(type: "double precision", nullable: false),
                    NutrientContent = table.Column<double>(type: "double precision", nullable: false),
                    OrganicMatter = table.Column<double>(type: "double precision", nullable: false),
                    SoilDensity = table.Column<double>(type: "double precision", nullable: false),
                    ErosionRisk = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Soils", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Crops",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CropType = table.Column<int>(type: "integer", nullable: false),
                    WaterRequirement = table.Column<double>(type: "double precision", nullable: false),
                    FertilizerRequirement = table.Column<double>(type: "double precision", nullable: false),
                    GrowingDuration = table.Column<int>(type: "integer", nullable: false),
                    SowingStart = table.Column<string>(type: "text", nullable: false),
                    SowingEnd = table.Column<string>(type: "text", nullable: false),
                    MinTemperature = table.Column<double>(type: "double precision", nullable: false),
                    MaxTemperature = table.Column<double>(type: "double precision", nullable: false),
                    HarvestYield = table.Column<double>(type: "double precision", nullable: false),
                    AdditionalNotes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    OptimalSoilId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Crops", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Crops_Soils_OptimalSoilId",
                        column: x => x.OptimalSoilId,
                        principalTable: "Soils",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Fields",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Location = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Boundary = table.Column<Polygon>(type: "geometry", nullable: false),
                    FieldType = table.Column<int>(type: "integer", nullable: false),
                    CurrentCropId = table.Column<int>(type: "integer", nullable: false),
                    SoilId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Fields_Crops_CurrentCropId",
                        column: x => x.CurrentCropId,
                        principalTable: "Crops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Fields_Soils_SoilId",
                        column: x => x.SoilId,
                        principalTable: "Soils",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FieldCropHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FieldId = table.Column<int>(type: "integer", nullable: false),
                    CropId = table.Column<int>(type: "integer", nullable: false),
                    PlantedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    HarvestedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Yield = table.Column<double>(type: "double precision", nullable: true),
                    Notes = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldCropHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FieldCropHistories_Crops_CropId",
                        column: x => x.CropId,
                        principalTable: "Crops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FieldCropHistories_Fields_FieldId",
                        column: x => x.FieldId,
                        principalTable: "Fields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Crops_OptimalSoilId",
                table: "Crops",
                column: "OptimalSoilId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldCropHistories_CropId",
                table: "FieldCropHistories",
                column: "CropId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldCropHistories_FieldId",
                table: "FieldCropHistories",
                column: "FieldId");

            migrationBuilder.CreateIndex(
                name: "IX_Fields_CurrentCropId",
                table: "Fields",
                column: "CurrentCropId");

            migrationBuilder.CreateIndex(
                name: "IX_Fields_SoilId",
                table: "Fields",
                column: "SoilId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FieldCropHistories");

            migrationBuilder.DropTable(
                name: "Fields");

            migrationBuilder.DropTable(
                name: "Crops");

            migrationBuilder.DropTable(
                name: "Soils");
        }
    }
}
