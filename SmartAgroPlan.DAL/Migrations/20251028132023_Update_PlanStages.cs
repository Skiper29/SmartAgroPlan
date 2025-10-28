using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartAgroPlan.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Update_PlanStages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "BoronPercent",
                table: "PlanStages",
                type: "double precision",
                precision: 5,
                scale: 4,
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "CopperPercent",
                table: "PlanStages",
                type: "double precision",
                precision: 5,
                scale: 4,
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "IronPercent",
                table: "PlanStages",
                type: "double precision",
                precision: 5,
                scale: 4,
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ManganesePercent",
                table: "PlanStages",
                type: "double precision",
                precision: 5,
                scale: 4,
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "MolybdenumPercent",
                table: "PlanStages",
                type: "double precision",
                precision: 5,
                scale: 4,
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ZincPercent",
                table: "PlanStages",
                type: "double precision",
                precision: 5,
                scale: 4,
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BoronPercent",
                table: "PlanStages");

            migrationBuilder.DropColumn(
                name: "CopperPercent",
                table: "PlanStages");

            migrationBuilder.DropColumn(
                name: "IronPercent",
                table: "PlanStages");

            migrationBuilder.DropColumn(
                name: "ManganesePercent",
                table: "PlanStages");

            migrationBuilder.DropColumn(
                name: "MolybdenumPercent",
                table: "PlanStages");

            migrationBuilder.DropColumn(
                name: "ZincPercent",
                table: "PlanStages");
        }
    }
}
