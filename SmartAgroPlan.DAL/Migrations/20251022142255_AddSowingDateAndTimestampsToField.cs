using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartAgroPlan.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddSowingDateAndTimestampsToField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "CurrentCropId",
                table: "Fields",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Fields",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Fields",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Fields");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Fields");

            migrationBuilder.AlterColumn<int>(
                name: "CurrentCropId",
                table: "Fields",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }
    }
}
