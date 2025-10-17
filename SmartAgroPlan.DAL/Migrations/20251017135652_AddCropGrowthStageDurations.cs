using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartAgroPlan.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddCropGrowthStageDurations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LDev",
                table: "Crops",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LIni",
                table: "Crops",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LLate",
                table: "Crops",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LMid",
                table: "Crops",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LDev",
                table: "Crops");

            migrationBuilder.DropColumn(
                name: "LIni",
                table: "Crops");

            migrationBuilder.DropColumn(
                name: "LLate",
                table: "Crops");

            migrationBuilder.DropColumn(
                name: "LMid",
                table: "Crops");
        }
    }
}
