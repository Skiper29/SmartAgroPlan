using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SmartAgroPlan.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Add_CropCoefficientDefinition_Entity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CropCoefficientDefinitions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CropType = table.Column<int>(type: "integer", nullable: false),
                    KcIni = table.Column<double>(type: "double precision", nullable: false),
                    KcMid = table.Column<double>(type: "double precision", nullable: false),
                    KcEnd = table.Column<double>(type: "double precision", nullable: false),
                    LIni = table.Column<int>(type: "integer", nullable: false),
                    LDev = table.Column<int>(type: "integer", nullable: false),
                    LMid = table.Column<int>(type: "integer", nullable: false),
                    LLate = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CropCoefficientDefinitions", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CropCoefficientDefinitions");
        }
    }
}
