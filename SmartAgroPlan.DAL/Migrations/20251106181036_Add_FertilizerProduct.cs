using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartAgroPlan.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Add_FertilizerProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FertilizerApplicationProduct_FertilizerProduct_FertilizerPr~",
                table: "FertilizerApplicationProduct");

            migrationBuilder.DropForeignKey(
                name: "FK_FertilizerApplicationRecordProduct_FertilizerProduct_Fertil~",
                table: "FertilizerApplicationRecordProduct");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FertilizerProduct",
                table: "FertilizerProduct");

            migrationBuilder.RenameTable(
                name: "FertilizerProduct",
                newName: "FertilizerProducts");

            migrationBuilder.RenameColumn(
                name: "ManganeseCont",
                table: "FertilizerProducts",
                newName: "ManganeseContent");

            migrationBuilder.RenameIndex(
                name: "IX_FertilizerProduct_Type",
                table: "FertilizerProducts",
                newName: "IX_FertilizerProducts_Type");

            migrationBuilder.RenameIndex(
                name: "IX_FertilizerProduct_Name",
                table: "FertilizerProducts",
                newName: "IX_FertilizerProducts_Name");

            migrationBuilder.AddColumn<int>(
                name: "Form",
                table: "FertilizerProducts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_FertilizerProducts",
                table: "FertilizerProducts",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FertilizerApplicationProduct_FertilizerProducts_FertilizerP~",
                table: "FertilizerApplicationProduct",
                column: "FertilizerProductId",
                principalTable: "FertilizerProducts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FertilizerApplicationRecordProduct_FertilizerProducts_Ferti~",
                table: "FertilizerApplicationRecordProduct",
                column: "FertilizerProductId",
                principalTable: "FertilizerProducts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FertilizerApplicationProduct_FertilizerProducts_FertilizerP~",
                table: "FertilizerApplicationProduct");

            migrationBuilder.DropForeignKey(
                name: "FK_FertilizerApplicationRecordProduct_FertilizerProducts_Ferti~",
                table: "FertilizerApplicationRecordProduct");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FertilizerProducts",
                table: "FertilizerProducts");

            migrationBuilder.DropColumn(
                name: "Form",
                table: "FertilizerProducts");

            migrationBuilder.RenameTable(
                name: "FertilizerProducts",
                newName: "FertilizerProduct");

            migrationBuilder.RenameColumn(
                name: "ManganeseContent",
                table: "FertilizerProduct",
                newName: "ManganeseCont");

            migrationBuilder.RenameIndex(
                name: "IX_FertilizerProducts_Type",
                table: "FertilizerProduct",
                newName: "IX_FertilizerProduct_Type");

            migrationBuilder.RenameIndex(
                name: "IX_FertilizerProducts_Name",
                table: "FertilizerProduct",
                newName: "IX_FertilizerProduct_Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FertilizerProduct",
                table: "FertilizerProduct",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FertilizerApplicationProduct_FertilizerProduct_FertilizerPr~",
                table: "FertilizerApplicationProduct",
                column: "FertilizerProductId",
                principalTable: "FertilizerProduct",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FertilizerApplicationRecordProduct_FertilizerProduct_Fertil~",
                table: "FertilizerApplicationRecordProduct",
                column: "FertilizerProductId",
                principalTable: "FertilizerProduct",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
