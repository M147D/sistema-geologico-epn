using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Servidor_Sistema_Geologia.Migrations
{
    /// <inheritdoc />
    public partial class TokenConcurrenciaXmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "tb_FotosElementos",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "tb_ElementosGeologicos",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "xmin",
                table: "tb_FotosElementos");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "tb_ElementosGeologicos");
        }
    }
}
