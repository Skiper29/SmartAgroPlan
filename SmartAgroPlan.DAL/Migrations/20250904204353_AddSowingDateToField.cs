using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartAgroPlan.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddSowingDateToField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "SowingDate",
                table: "Fields",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SowingDate",
                table: "Fields");
        }
    }
}
