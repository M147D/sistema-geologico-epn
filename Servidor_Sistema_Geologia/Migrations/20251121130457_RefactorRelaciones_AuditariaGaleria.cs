using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Servidor_Sistema_Geologia.Migrations
{
    /// <inheritdoc />
    public partial class RefactorRelaciones_AuditariaGaleria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Paso 1: Eliminar las FK que serán refactorizadas
            migrationBuilder.DropForeignKey(
                name: "FK_ElementosGeologicos_GaleriaElementosGeologicos_GaleriaElementosGeologicoId",
                table: "ElementosGeologicos");

            migrationBuilder.DropForeignKey(
                name: "FK_Ubicaciones_Paises_PaisId",
                table: "Ubicaciones");

            // Paso 2: Ahora que las FK están eliminadas, limpiar galerías huérfanas
            migrationBuilder.Sql(@"
                -- Primero, eliminar las fotos de galerías huérfanas
                DELETE FROM [FotosElementos]
                WHERE [GaleriaElementosGeologicoId] IN (
                    SELECT [Id] FROM [GaleriaElementosGeologicos]
                    WHERE [ElementoGeologicoId] = 0 OR [ElementoGeologicoId] IS NULL
                );

                -- Luego, eliminar las galerías huérfanas
                DELETE FROM [GaleriaElementosGeologicos]
                WHERE [ElementoGeologicoId] = 0 OR [ElementoGeologicoId] IS NULL;
            ");

            migrationBuilder.DropTable(
                name: "HistorialAccesos");

            migrationBuilder.DropIndex(
                name: "IX_ElementosGeologicos_GaleriaElementosGeologicoId",
                table: "ElementosGeologicos");

            migrationBuilder.DropColumn(
                name: "GaleriaElementosGeologicoId",
                table: "ElementosGeologicos");

            migrationBuilder.RenameColumn(
                name: "PaisId",
                table: "Ubicaciones",
                newName: "UsuarioEliminacionId");

            migrationBuilder.RenameIndex(
                name: "IX_Ubicaciones_PaisId",
                table: "Ubicaciones",
                newName: "IX_Ubicaciones_UsuarioEliminacionId");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaEliminacion",
                table: "Ubicaciones",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioActualizacionId",
                table: "Ubicaciones",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioCreacionId",
                table: "Ubicaciones",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaEliminacion",
                table: "Provincias",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioActualizacionId",
                table: "Provincias",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioCreacionId",
                table: "Provincias",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioEliminacionId",
                table: "Provincias",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaEliminacion",
                table: "Paises",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioActualizacionId",
                table: "Paises",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioCreacionId",
                table: "Paises",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioEliminacionId",
                table: "Paises",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EstadoActivo",
                table: "GaleriaElementosGeologicos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaActualizacion",
                table: "GaleriaElementosGeologicos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "GaleriaElementosGeologicos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaEliminacion",
                table: "GaleriaElementosGeologicos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioActualizacionId",
                table: "GaleriaElementosGeologicos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioCreacionId",
                table: "GaleriaElementosGeologicos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioEliminacionId",
                table: "GaleriaElementosGeologicos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EstadoActivo",
                table: "FotosElementos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaActualizacion",
                table: "FotosElementos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "FotosElementos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaEliminacion",
                table: "FotosElementos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioActualizacionId",
                table: "FotosElementos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioCreacionId",
                table: "FotosElementos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioEliminacionId",
                table: "FotosElementos",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UbicacionId",
                table: "ElementosGeologicos",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaEliminacion",
                table: "ElementosGeologicos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioActualizacionId",
                table: "ElementosGeologicos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioCreacionId",
                table: "ElementosGeologicos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioEliminacionId",
                table: "ElementosGeologicos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ubicaciones_UsuarioActualizacionId",
                table: "Ubicaciones",
                column: "UsuarioActualizacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Ubicaciones_UsuarioCreacionId",
                table: "Ubicaciones",
                column: "UsuarioCreacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Provincias_UsuarioActualizacionId",
                table: "Provincias",
                column: "UsuarioActualizacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Provincias_UsuarioCreacionId",
                table: "Provincias",
                column: "UsuarioCreacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Provincias_UsuarioEliminacionId",
                table: "Provincias",
                column: "UsuarioEliminacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Paises_UsuarioActualizacionId",
                table: "Paises",
                column: "UsuarioActualizacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Paises_UsuarioCreacionId",
                table: "Paises",
                column: "UsuarioCreacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Paises_UsuarioEliminacionId",
                table: "Paises",
                column: "UsuarioEliminacionId");

            migrationBuilder.CreateIndex(
                name: "IX_GaleriaElementosGeologicos_ElementoGeologicoId",
                table: "GaleriaElementosGeologicos",
                column: "ElementoGeologicoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GaleriaElementosGeologicos_UsuarioActualizacionId",
                table: "GaleriaElementosGeologicos",
                column: "UsuarioActualizacionId");

            migrationBuilder.CreateIndex(
                name: "IX_GaleriaElementosGeologicos_UsuarioCreacionId",
                table: "GaleriaElementosGeologicos",
                column: "UsuarioCreacionId");

            migrationBuilder.CreateIndex(
                name: "IX_GaleriaElementosGeologicos_UsuarioEliminacionId",
                table: "GaleriaElementosGeologicos",
                column: "UsuarioEliminacionId");

            migrationBuilder.CreateIndex(
                name: "IX_FotosElementos_UsuarioActualizacionId",
                table: "FotosElementos",
                column: "UsuarioActualizacionId");

            migrationBuilder.CreateIndex(
                name: "IX_FotosElementos_UsuarioCreacionId",
                table: "FotosElementos",
                column: "UsuarioCreacionId");

            migrationBuilder.CreateIndex(
                name: "IX_FotosElementos_UsuarioEliminacionId",
                table: "FotosElementos",
                column: "UsuarioEliminacionId");

            migrationBuilder.CreateIndex(
                name: "IX_ElementosGeologicos_UsuarioActualizacionId",
                table: "ElementosGeologicos",
                column: "UsuarioActualizacionId");

            migrationBuilder.CreateIndex(
                name: "IX_ElementosGeologicos_UsuarioCreacionId",
                table: "ElementosGeologicos",
                column: "UsuarioCreacionId");

            migrationBuilder.CreateIndex(
                name: "IX_ElementosGeologicos_UsuarioEliminacionId",
                table: "ElementosGeologicos",
                column: "UsuarioEliminacionId");

            migrationBuilder.AddForeignKey(
                name: "FK_ElementosGeologicos_Usuarios_UsuarioActualizacionId",
                table: "ElementosGeologicos",
                column: "UsuarioActualizacionId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ElementosGeologicos_Usuarios_UsuarioCreacionId",
                table: "ElementosGeologicos",
                column: "UsuarioCreacionId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ElementosGeologicos_Usuarios_UsuarioEliminacionId",
                table: "ElementosGeologicos",
                column: "UsuarioEliminacionId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FotosElementos_Usuarios_UsuarioActualizacionId",
                table: "FotosElementos",
                column: "UsuarioActualizacionId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FotosElementos_Usuarios_UsuarioCreacionId",
                table: "FotosElementos",
                column: "UsuarioCreacionId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FotosElementos_Usuarios_UsuarioEliminacionId",
                table: "FotosElementos",
                column: "UsuarioEliminacionId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GaleriaElementosGeologicos_ElementosGeologicos_ElementoGeologicoId",
                table: "GaleriaElementosGeologicos",
                column: "ElementoGeologicoId",
                principalTable: "ElementosGeologicos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GaleriaElementosGeologicos_Usuarios_UsuarioActualizacionId",
                table: "GaleriaElementosGeologicos",
                column: "UsuarioActualizacionId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GaleriaElementosGeologicos_Usuarios_UsuarioCreacionId",
                table: "GaleriaElementosGeologicos",
                column: "UsuarioCreacionId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GaleriaElementosGeologicos_Usuarios_UsuarioEliminacionId",
                table: "GaleriaElementosGeologicos",
                column: "UsuarioEliminacionId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Paises_Usuarios_UsuarioActualizacionId",
                table: "Paises",
                column: "UsuarioActualizacionId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Paises_Usuarios_UsuarioCreacionId",
                table: "Paises",
                column: "UsuarioCreacionId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Paises_Usuarios_UsuarioEliminacionId",
                table: "Paises",
                column: "UsuarioEliminacionId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Provincias_Usuarios_UsuarioActualizacionId",
                table: "Provincias",
                column: "UsuarioActualizacionId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Provincias_Usuarios_UsuarioCreacionId",
                table: "Provincias",
                column: "UsuarioCreacionId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Provincias_Usuarios_UsuarioEliminacionId",
                table: "Provincias",
                column: "UsuarioEliminacionId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Ubicaciones_Usuarios_UsuarioActualizacionId",
                table: "Ubicaciones",
                column: "UsuarioActualizacionId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Ubicaciones_Usuarios_UsuarioCreacionId",
                table: "Ubicaciones",
                column: "UsuarioCreacionId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Ubicaciones_Usuarios_UsuarioEliminacionId",
                table: "Ubicaciones",
                column: "UsuarioEliminacionId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ElementosGeologicos_Usuarios_UsuarioActualizacionId",
                table: "ElementosGeologicos");

            migrationBuilder.DropForeignKey(
                name: "FK_ElementosGeologicos_Usuarios_UsuarioCreacionId",
                table: "ElementosGeologicos");

            migrationBuilder.DropForeignKey(
                name: "FK_ElementosGeologicos_Usuarios_UsuarioEliminacionId",
                table: "ElementosGeologicos");

            migrationBuilder.DropForeignKey(
                name: "FK_FotosElementos_Usuarios_UsuarioActualizacionId",
                table: "FotosElementos");

            migrationBuilder.DropForeignKey(
                name: "FK_FotosElementos_Usuarios_UsuarioCreacionId",
                table: "FotosElementos");

            migrationBuilder.DropForeignKey(
                name: "FK_FotosElementos_Usuarios_UsuarioEliminacionId",
                table: "FotosElementos");

            migrationBuilder.DropForeignKey(
                name: "FK_GaleriaElementosGeologicos_ElementosGeologicos_ElementoGeologicoId",
                table: "GaleriaElementosGeologicos");

            migrationBuilder.DropForeignKey(
                name: "FK_GaleriaElementosGeologicos_Usuarios_UsuarioActualizacionId",
                table: "GaleriaElementosGeologicos");

            migrationBuilder.DropForeignKey(
                name: "FK_GaleriaElementosGeologicos_Usuarios_UsuarioCreacionId",
                table: "GaleriaElementosGeologicos");

            migrationBuilder.DropForeignKey(
                name: "FK_GaleriaElementosGeologicos_Usuarios_UsuarioEliminacionId",
                table: "GaleriaElementosGeologicos");

            migrationBuilder.DropForeignKey(
                name: "FK_Paises_Usuarios_UsuarioActualizacionId",
                table: "Paises");

            migrationBuilder.DropForeignKey(
                name: "FK_Paises_Usuarios_UsuarioCreacionId",
                table: "Paises");

            migrationBuilder.DropForeignKey(
                name: "FK_Paises_Usuarios_UsuarioEliminacionId",
                table: "Paises");

            migrationBuilder.DropForeignKey(
                name: "FK_Provincias_Usuarios_UsuarioActualizacionId",
                table: "Provincias");

            migrationBuilder.DropForeignKey(
                name: "FK_Provincias_Usuarios_UsuarioCreacionId",
                table: "Provincias");

            migrationBuilder.DropForeignKey(
                name: "FK_Provincias_Usuarios_UsuarioEliminacionId",
                table: "Provincias");

            migrationBuilder.DropForeignKey(
                name: "FK_Ubicaciones_Usuarios_UsuarioActualizacionId",
                table: "Ubicaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_Ubicaciones_Usuarios_UsuarioCreacionId",
                table: "Ubicaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_Ubicaciones_Usuarios_UsuarioEliminacionId",
                table: "Ubicaciones");

            migrationBuilder.DropIndex(
                name: "IX_Ubicaciones_UsuarioActualizacionId",
                table: "Ubicaciones");

            migrationBuilder.DropIndex(
                name: "IX_Ubicaciones_UsuarioCreacionId",
                table: "Ubicaciones");

            migrationBuilder.DropIndex(
                name: "IX_Provincias_UsuarioActualizacionId",
                table: "Provincias");

            migrationBuilder.DropIndex(
                name: "IX_Provincias_UsuarioCreacionId",
                table: "Provincias");

            migrationBuilder.DropIndex(
                name: "IX_Provincias_UsuarioEliminacionId",
                table: "Provincias");

            migrationBuilder.DropIndex(
                name: "IX_Paises_UsuarioActualizacionId",
                table: "Paises");

            migrationBuilder.DropIndex(
                name: "IX_Paises_UsuarioCreacionId",
                table: "Paises");

            migrationBuilder.DropIndex(
                name: "IX_Paises_UsuarioEliminacionId",
                table: "Paises");

            migrationBuilder.DropIndex(
                name: "IX_GaleriaElementosGeologicos_ElementoGeologicoId",
                table: "GaleriaElementosGeologicos");

            migrationBuilder.DropIndex(
                name: "IX_GaleriaElementosGeologicos_UsuarioActualizacionId",
                table: "GaleriaElementosGeologicos");

            migrationBuilder.DropIndex(
                name: "IX_GaleriaElementosGeologicos_UsuarioCreacionId",
                table: "GaleriaElementosGeologicos");

            migrationBuilder.DropIndex(
                name: "IX_GaleriaElementosGeologicos_UsuarioEliminacionId",
                table: "GaleriaElementosGeologicos");

            migrationBuilder.DropIndex(
                name: "IX_FotosElementos_UsuarioActualizacionId",
                table: "FotosElementos");

            migrationBuilder.DropIndex(
                name: "IX_FotosElementos_UsuarioCreacionId",
                table: "FotosElementos");

            migrationBuilder.DropIndex(
                name: "IX_FotosElementos_UsuarioEliminacionId",
                table: "FotosElementos");

            migrationBuilder.DropIndex(
                name: "IX_ElementosGeologicos_UsuarioActualizacionId",
                table: "ElementosGeologicos");

            migrationBuilder.DropIndex(
                name: "IX_ElementosGeologicos_UsuarioCreacionId",
                table: "ElementosGeologicos");

            migrationBuilder.DropIndex(
                name: "IX_ElementosGeologicos_UsuarioEliminacionId",
                table: "ElementosGeologicos");

            migrationBuilder.DropColumn(
                name: "FechaEliminacion",
                table: "Ubicaciones");

            migrationBuilder.DropColumn(
                name: "UsuarioActualizacionId",
                table: "Ubicaciones");

            migrationBuilder.DropColumn(
                name: "UsuarioCreacionId",
                table: "Ubicaciones");

            migrationBuilder.DropColumn(
                name: "FechaEliminacion",
                table: "Provincias");

            migrationBuilder.DropColumn(
                name: "UsuarioActualizacionId",
                table: "Provincias");

            migrationBuilder.DropColumn(
                name: "UsuarioCreacionId",
                table: "Provincias");

            migrationBuilder.DropColumn(
                name: "UsuarioEliminacionId",
                table: "Provincias");

            migrationBuilder.DropColumn(
                name: "FechaEliminacion",
                table: "Paises");

            migrationBuilder.DropColumn(
                name: "UsuarioActualizacionId",
                table: "Paises");

            migrationBuilder.DropColumn(
                name: "UsuarioCreacionId",
                table: "Paises");

            migrationBuilder.DropColumn(
                name: "UsuarioEliminacionId",
                table: "Paises");

            migrationBuilder.DropColumn(
                name: "EstadoActivo",
                table: "GaleriaElementosGeologicos");

            migrationBuilder.DropColumn(
                name: "FechaActualizacion",
                table: "GaleriaElementosGeologicos");

            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "GaleriaElementosGeologicos");

            migrationBuilder.DropColumn(
                name: "FechaEliminacion",
                table: "GaleriaElementosGeologicos");

            migrationBuilder.DropColumn(
                name: "UsuarioActualizacionId",
                table: "GaleriaElementosGeologicos");

            migrationBuilder.DropColumn(
                name: "UsuarioCreacionId",
                table: "GaleriaElementosGeologicos");

            migrationBuilder.DropColumn(
                name: "UsuarioEliminacionId",
                table: "GaleriaElementosGeologicos");

            migrationBuilder.DropColumn(
                name: "EstadoActivo",
                table: "FotosElementos");

            migrationBuilder.DropColumn(
                name: "FechaActualizacion",
                table: "FotosElementos");

            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "FotosElementos");

            migrationBuilder.DropColumn(
                name: "FechaEliminacion",
                table: "FotosElementos");

            migrationBuilder.DropColumn(
                name: "UsuarioActualizacionId",
                table: "FotosElementos");

            migrationBuilder.DropColumn(
                name: "UsuarioCreacionId",
                table: "FotosElementos");

            migrationBuilder.DropColumn(
                name: "UsuarioEliminacionId",
                table: "FotosElementos");

            migrationBuilder.DropColumn(
                name: "FechaEliminacion",
                table: "ElementosGeologicos");

            migrationBuilder.DropColumn(
                name: "UsuarioActualizacionId",
                table: "ElementosGeologicos");

            migrationBuilder.DropColumn(
                name: "UsuarioCreacionId",
                table: "ElementosGeologicos");

            migrationBuilder.DropColumn(
                name: "UsuarioEliminacionId",
                table: "ElementosGeologicos");

            migrationBuilder.RenameColumn(
                name: "UsuarioEliminacionId",
                table: "Ubicaciones",
                newName: "PaisId");

            migrationBuilder.RenameIndex(
                name: "IX_Ubicaciones_UsuarioEliminacionId",
                table: "Ubicaciones",
                newName: "IX_Ubicaciones_PaisId");

            migrationBuilder.AlterColumn<int>(
                name: "UbicacionId",
                table: "ElementosGeologicos",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GaleriaElementosGeologicoId",
                table: "ElementosGeologicos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "HistorialAccesos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ElementoGeologicoId = table.Column<int>(type: "int", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    Accion = table.Column<int>(type: "int", nullable: false),
                    FechaAcceso = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialAccesos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistorialAccesos_ElementosGeologicos_ElementoGeologicoId",
                        column: x => x.ElementoGeologicoId,
                        principalTable: "ElementosGeologicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HistorialAccesos_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ElementosGeologicos_GaleriaElementosGeologicoId",
                table: "ElementosGeologicos",
                column: "GaleriaElementosGeologicoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HistorialAccesos_ElementoGeologicoId",
                table: "HistorialAccesos",
                column: "ElementoGeologicoId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialAccesos_UsuarioId",
                table: "HistorialAccesos",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_ElementosGeologicos_GaleriaElementosGeologicos_GaleriaElementosGeologicoId",
                table: "ElementosGeologicos",
                column: "GaleriaElementosGeologicoId",
                principalTable: "GaleriaElementosGeologicos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Ubicaciones_Paises_PaisId",
                table: "Ubicaciones",
                column: "PaisId",
                principalTable: "Paises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
