using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SmartAgroPlan.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Add_FertilierApplication_Entities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FertilizerApplicationPlan",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FieldId = table.Column<int>(type: "integer", nullable: false),
                    FertilizationPlanId = table.Column<int>(type: "integer", nullable: false),
                    PlanStageId = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PlannedApplicationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DaysAfterPlanting = table.Column<int>(type: "integer", nullable: false),
                    PlannedNitrogen = table.Column<double>(type: "double precision", nullable: false),
                    PlannedPhosphorus = table.Column<double>(type: "double precision", nullable: false),
                    PlannedPotassium = table.Column<double>(type: "double precision", nullable: false),
                    PlannedSulfur = table.Column<double>(type: "double precision", nullable: false),
                    PlannedCalcium = table.Column<double>(type: "double precision", nullable: false),
                    PlannedMagnesium = table.Column<double>(type: "double precision", nullable: false),
                    PlannedBoron = table.Column<double>(type: "double precision", nullable: false),
                    PlannedZinc = table.Column<double>(type: "double precision", nullable: false),
                    PlannedManganese = table.Column<double>(type: "double precision", nullable: false),
                    PlannedCopper = table.Column<double>(type: "double precision", nullable: false),
                    PlannedIron = table.Column<double>(type: "double precision", nullable: false),
                    PlannedMolybdenum = table.Column<double>(type: "double precision", nullable: false),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    ActualApplicationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FertilizerApplicationPlan", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FertilizerApplicationPlan_FertilizationPlans_FertilizationP~",
                        column: x => x.FertilizationPlanId,
                        principalTable: "FertilizationPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FertilizerApplicationPlan_Fields_FieldId",
                        column: x => x.FieldId,
                        principalTable: "Fields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FertilizerApplicationPlan_PlanStages_PlanStageId",
                        column: x => x.PlanStageId,
                        principalTable: "PlanStages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FertilizerProduct",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    NitrogenContent = table.Column<double>(type: "double precision", nullable: false),
                    PhosphorusContent = table.Column<double>(type: "double precision", nullable: false),
                    PotassiumContent = table.Column<double>(type: "double precision", nullable: false),
                    SulfurContent = table.Column<double>(type: "double precision", nullable: true),
                    CalciumContent = table.Column<double>(type: "double precision", nullable: true),
                    MagnesiumContent = table.Column<double>(type: "double precision", nullable: true),
                    IronContent = table.Column<double>(type: "double precision", nullable: true),
                    ZincContent = table.Column<double>(type: "double precision", nullable: true),
                    BoronContent = table.Column<double>(type: "double precision", nullable: true),
                    ManganeseCont = table.Column<double>(type: "double precision", nullable: true),
                    CopperContent = table.Column<double>(type: "double precision", nullable: true),
                    MolybdenumContent = table.Column<double>(type: "double precision", nullable: true),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Manufacturer = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FertilizerProduct", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FertilizerApplicationRecord",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FieldId = table.Column<int>(type: "integer", nullable: false),
                    ApplicationPlanId = table.Column<int>(type: "integer", nullable: true),
                    ApplicationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RecordedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AppliedNitrogen = table.Column<double>(type: "double precision", nullable: false),
                    AppliedPhosphorus = table.Column<double>(type: "double precision", nullable: false),
                    AppliedPotassium = table.Column<double>(type: "double precision", nullable: false),
                    AppliedSulfur = table.Column<double>(type: "double precision", nullable: false),
                    AppliedCalcium = table.Column<double>(type: "double precision", nullable: false),
                    AppliedMagnesium = table.Column<double>(type: "double precision", nullable: false),
                    AppliedBoron = table.Column<double>(type: "double precision", nullable: false),
                    AppliedZinc = table.Column<double>(type: "double precision", nullable: false),
                    AppliedManganese = table.Column<double>(type: "double precision", nullable: false),
                    AppliedCopper = table.Column<double>(type: "double precision", nullable: false),
                    AppliedIron = table.Column<double>(type: "double precision", nullable: false),
                    AppliedMolybdenum = table.Column<double>(type: "double precision", nullable: false),
                    ApplicationMethodId = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    TemperatureC = table.Column<double>(type: "double precision", nullable: true),
                    WindSpeedKmh = table.Column<double>(type: "double precision", nullable: true),
                    Humidity = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FertilizerApplicationRecord", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FertilizerApplicationRecord_ApplicationMethods_ApplicationM~",
                        column: x => x.ApplicationMethodId,
                        principalTable: "ApplicationMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FertilizerApplicationRecord_FertilizerApplicationPlan_Appli~",
                        column: x => x.ApplicationPlanId,
                        principalTable: "FertilizerApplicationPlan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_FertilizerApplicationRecord_Fields_FieldId",
                        column: x => x.FieldId,
                        principalTable: "Fields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FertilizerApplicationProduct",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ApplicationPlanId = table.Column<int>(type: "integer", nullable: false),
                    FertilizerProductId = table.Column<int>(type: "integer", nullable: false),
                    QuantityKgPerHa = table.Column<double>(type: "double precision", nullable: false),
                    TotalQuantityKg = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FertilizerApplicationProduct", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FertilizerApplicationProduct_FertilizerApplicationPlan_Appl~",
                        column: x => x.ApplicationPlanId,
                        principalTable: "FertilizerApplicationPlan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FertilizerApplicationProduct_FertilizerProduct_FertilizerPr~",
                        column: x => x.FertilizerProductId,
                        principalTable: "FertilizerProduct",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FertilizerApplicationRecordProduct",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ApplicationRecordId = table.Column<int>(type: "integer", nullable: false),
                    FertilizerProductId = table.Column<int>(type: "integer", nullable: false),
                    QuantityUsedKg = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FertilizerApplicationRecordProduct", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FertilizerApplicationRecordProduct_FertilizerApplicationRec~",
                        column: x => x.ApplicationRecordId,
                        principalTable: "FertilizerApplicationRecord",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FertilizerApplicationRecordProduct_FertilizerProduct_Fertil~",
                        column: x => x.FertilizerProductId,
                        principalTable: "FertilizerProduct",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FertilizerApplicationPlan_FertilizationPlanId",
                table: "FertilizerApplicationPlan",
                column: "FertilizationPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_FertilizerApplicationPlan_FieldId_PlannedApplicationDate",
                table: "FertilizerApplicationPlan",
                columns: new[] { "FieldId", "PlannedApplicationDate" });

            migrationBuilder.CreateIndex(
                name: "IX_FertilizerApplicationPlan_IsCompleted",
                table: "FertilizerApplicationPlan",
                column: "IsCompleted");

            migrationBuilder.CreateIndex(
                name: "IX_FertilizerApplicationPlan_PlanStageId",
                table: "FertilizerApplicationPlan",
                column: "PlanStageId");

            migrationBuilder.CreateIndex(
                name: "IX_FertilizerApplicationProduct_ApplicationPlanId",
                table: "FertilizerApplicationProduct",
                column: "ApplicationPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_FertilizerApplicationProduct_FertilizerProductId",
                table: "FertilizerApplicationProduct",
                column: "FertilizerProductId");

            migrationBuilder.CreateIndex(
                name: "IX_FertilizerApplicationRecord_ApplicationMethodId",
                table: "FertilizerApplicationRecord",
                column: "ApplicationMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_FertilizerApplicationRecord_ApplicationPlanId",
                table: "FertilizerApplicationRecord",
                column: "ApplicationPlanId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FertilizerApplicationRecord_FieldId_ApplicationDate",
                table: "FertilizerApplicationRecord",
                columns: new[] { "FieldId", "ApplicationDate" });

            migrationBuilder.CreateIndex(
                name: "IX_FertilizerApplicationRecord_RecordedDate",
                table: "FertilizerApplicationRecord",
                column: "RecordedDate");

            migrationBuilder.CreateIndex(
                name: "IX_FertilizerApplicationRecordProduct_ApplicationRecordId",
                table: "FertilizerApplicationRecordProduct",
                column: "ApplicationRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_FertilizerApplicationRecordProduct_FertilizerProductId",
                table: "FertilizerApplicationRecordProduct",
                column: "FertilizerProductId");

            migrationBuilder.CreateIndex(
                name: "IX_FertilizerProduct_Name",
                table: "FertilizerProduct",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_FertilizerProduct_Type",
                table: "FertilizerProduct",
                column: "Type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FertilizerApplicationProduct");

            migrationBuilder.DropTable(
                name: "FertilizerApplicationRecordProduct");

            migrationBuilder.DropTable(
                name: "FertilizerApplicationRecord");

            migrationBuilder.DropTable(
                name: "FertilizerProduct");

            migrationBuilder.DropTable(
                name: "FertilizerApplicationPlan");
        }
    }
}
