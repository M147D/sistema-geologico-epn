using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Servidor_Sistema_Geologia.Migrations
{
    /// <inheritdoc />
    public partial class UbicacionesCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EstadoActivo",
                table: "Ubicaciones",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaActualizacion",
                table: "Ubicaciones",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "Ubicaciones",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "EstadoActivo",
                table: "Provincias",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaActualizacion",
                table: "Provincias",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "Provincias",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "EstadoActivo",
                table: "Paises",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaActualizacion",
                table: "Paises",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "Paises",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstadoActivo",
                table: "Ubicaciones");

            migrationBuilder.DropColumn(
                name: "FechaActualizacion",
                table: "Ubicaciones");

            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "Ubicaciones");

            migrationBuilder.DropColumn(
                name: "EstadoActivo",
                table: "Provincias");

            migrationBuilder.DropColumn(
                name: "FechaActualizacion",
                table: "Provincias");

            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "Provincias");

            migrationBuilder.DropColumn(
                name: "EstadoActivo",
                table: "Paises");

            migrationBuilder.DropColumn(
                name: "FechaActualizacion",
                table: "Paises");

            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "Paises");
        }
    }
}
