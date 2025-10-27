using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SmartAgroPlan.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddFertilizationPlanEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationMethods",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationMethods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FertilizationPlans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CropType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FertilizationPlans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlanStages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StageName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Rationale = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    FertilizationPlanId = table.Column<int>(type: "integer", nullable: false),
                    GrowthStage = table.Column<int>(type: "integer", nullable: false),
                    TimingFactor = table.Column<double>(type: "double precision", nullable: false),
                    ApplicationMethodId = table.Column<int>(type: "integer", nullable: false),
                    NitrogenPercent = table.Column<double>(type: "double precision", precision: 5, scale: 4, nullable: false),
                    PhosphorusPercent = table.Column<double>(type: "double precision", precision: 5, scale: 4, nullable: false),
                    PotassiumPercent = table.Column<double>(type: "double precision", precision: 5, scale: 4, nullable: false),
                    SulfurPercent = table.Column<double>(type: "double precision", precision: 5, scale: 4, nullable: false),
                    CalciumPercent = table.Column<double>(type: "double precision", precision: 5, scale: 4, nullable: false),
                    MagnesiumPercent = table.Column<double>(type: "double precision", precision: 5, scale: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanStages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlanStages_ApplicationMethods_ApplicationMethodId",
                        column: x => x.ApplicationMethodId,
                        principalTable: "ApplicationMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlanStages_FertilizationPlans_FertilizationPlanId",
                        column: x => x.FertilizationPlanId,
                        principalTable: "FertilizationPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ApplicationMethods",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Внесення твердих добрив перед сівбою з подальшим загортанням у ґрунт.", "Розкидання з загортанням" },
                    { 2, "Внесення добрив у ґрунт поблизу кореневої зони під час вегетації.", "Прикореневе підживлення" },
                    { 3, "Розкидання добрив по поверхні ґрунту (часто для азоту).", "Поверхневе підживлення (розкидання)" },
                    { 4, "Внесення розчинених добрив безпосередньо на листя рослини.", "Листкове обприскування" },
                    { 5, "Внесення добрив разом із поливною водою через систему зрошення.", "Фертигація" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlanStages_ApplicationMethodId",
                table: "PlanStages",
                column: "ApplicationMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanStages_FertilizationPlanId",
                table: "PlanStages",
                column: "FertilizationPlanId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlanStages");

            migrationBuilder.DropTable(
                name: "ApplicationMethods");

            migrationBuilder.DropTable(
                name: "FertilizationPlans");
        }
    }
}
