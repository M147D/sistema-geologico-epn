using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Servidor_Sistema_Geologia.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSubType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Roca_Litologia",
                table: "ElementosGeologicos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TipoFosil",
                table: "ElementosGeologicos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TipoMineral",
                table: "ElementosGeologicos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Roca_Litologia",
                table: "ElementosGeologicos");

            migrationBuilder.DropColumn(
                name: "TipoFosil",
                table: "ElementosGeologicos");

            migrationBuilder.DropColumn(
                name: "TipoMineral",
                table: "ElementosGeologicos");
        }
    }
}
