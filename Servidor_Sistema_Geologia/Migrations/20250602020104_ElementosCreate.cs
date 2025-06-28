using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Servidor_Sistema_Geologia.Migrations
{
    /// <inheritdoc />
    public partial class ElementosCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ElementosGeologicos_GaleriaElementosGeologicoId",
                table: "ElementosGeologicos");

            migrationBuilder.AlterColumn<int>(
                name: "UbicacionId",
                table: "ElementosGeologicos",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "GaleriaElementosGeologicoId",
                table: "ElementosGeologicos",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EstadoActivo",
                table: "ElementosGeologicos",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaActualizacion",
                table: "ElementosGeologicos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "ElementosGeologicos",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            migrationBuilder.CreateIndex(
                name: "IX_ElementosGeologicos_GaleriaElementosGeologicoId",
                table: "ElementosGeologicos",
                column: "GaleriaElementosGeologicoId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ElementosGeologicos_GaleriaElementosGeologicoId",
                table: "ElementosGeologicos");

            migrationBuilder.DropColumn(
                name: "EstadoActivo",
                table: "ElementosGeologicos");

            migrationBuilder.DropColumn(
                name: "FechaActualizacion",
                table: "ElementosGeologicos");

            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "ElementosGeologicos");

            migrationBuilder.AlterColumn<int>(
                name: "UbicacionId",
                table: "ElementosGeologicos",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "GaleriaElementosGeologicoId",
                table: "ElementosGeologicos",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_ElementosGeologicos_GaleriaElementosGeologicoId",
                table: "ElementosGeologicos",
                column: "GaleriaElementosGeologicoId",
                unique: true,
                filter: "[GaleriaElementosGeologicoId] IS NOT NULL");
        }
    }
}
