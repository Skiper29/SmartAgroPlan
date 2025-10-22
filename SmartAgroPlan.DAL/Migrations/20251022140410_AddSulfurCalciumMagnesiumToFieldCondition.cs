using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartAgroPlan.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddSulfurCalciumMagnesiumToFieldCondition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Calcium",
                table: "FieldConditions",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Magnesium",
                table: "FieldConditions",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Sulfur",
                table: "FieldConditions",
                type: "double precision",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Calcium",
                table: "FieldConditions");

            migrationBuilder.DropColumn(
                name: "Magnesium",
                table: "FieldConditions");

            migrationBuilder.DropColumn(
                name: "Sulfur",
                table: "FieldConditions");
        }
    }
}
