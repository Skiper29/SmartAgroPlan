using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace SmartAgroPlan.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UseGeographyForFieldBoundary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Polygon>(
                name: "Boundary",
                table: "Fields",
                type: "geography(Polygon, 4326)",
                nullable: false,
                oldClrType: typeof(Polygon),
                oldType: "geometry");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Polygon>(
                name: "Boundary",
                table: "Fields",
                type: "geometry",
                nullable: false,
                oldClrType: typeof(Polygon),
                oldType: "geography(Polygon, 4326)");
        }
    }
}
