using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartAgroPlan.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Add_ProductForm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ManganeseCont",
                table: "FertilizerProduct",
                newName: "ManganeseContent");

            migrationBuilder.AddColumn<int>(
                name: "Form",
                table: "FertilizerProduct",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Form",
                table: "FertilizerProduct");

            migrationBuilder.RenameColumn(
                name: "ManganeseContent",
                table: "FertilizerProduct",
                newName: "ManganeseCont");
        }
    }
}
