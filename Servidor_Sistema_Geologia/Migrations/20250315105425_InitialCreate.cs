using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Servidor_Sistema_Geologia.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EstadosElementos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DescripcionEstado = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadosElementos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GaleriaElementosGeologicos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ElementoGeologicoId = table.Column<int>(type: "int", nullable: true),
                    DetalleGrupo = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GaleriaElementosGeologicos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Paises",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombrePais = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Paises", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreCompleto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NombreUsuario = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Rol = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FotosElementos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GaleriaElementosGeologicoId = table.Column<int>(type: "int", nullable: true),
                    Imagen = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    TipoFoto = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FechaSubida = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreadoPor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DescripcionEspecifica = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Etiquetas = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FotosElementos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FotosElementos_GaleriaElementosGeologicos_GaleriaElementosGeologicoId",
                        column: x => x.GaleriaElementosGeologicoId,
                        principalTable: "GaleriaElementosGeologicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Provincias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaisId = table.Column<int>(type: "int", nullable: true),
                    NombreProvincia = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provincias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Provincias_Paises_PaisId",
                        column: x => x.PaisId,
                        principalTable: "Paises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Ubicaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProvinciaId = table.Column<int>(type: "int", nullable: true),
                    PaisId = table.Column<int>(type: "int", nullable: true),
                    Latitud = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    Longitud = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    Localidad = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Leyenda = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ubicaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ubicaciones_Paises_PaisId",
                        column: x => x.PaisId,
                        principalTable: "Paises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ubicaciones_Provincias_ProvinciaId",
                        column: x => x.ProvinciaId,
                        principalTable: "Provincias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ElementosGeologicos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EstadoElementoId = table.Column<int>(type: "int", nullable: true),
                    UbicacionId = table.Column<int>(type: "int", nullable: true),
                    GaleriaElementosGeologicoId = table.Column<int>(type: "int", nullable: true),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Edad = table.Column<decimal>(type: "decimal(20,0)", nullable: true),
                    Donante = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FechaIngreso = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Codigo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Ejemplares = table.Column<long>(type: "bigint", nullable: true),
                    DocumentosRelacionados = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LaminaURL = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    LaminaExiste = table.Column<bool>(type: "bit", nullable: true),
                    TipoElemento = table.Column<string>(type: "nvarchar(21)", maxLength: 21, nullable: false),
                    Especie = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Periodo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TipoRoca = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Litologia = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElementosGeologicos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ElementosGeologicos_EstadosElementos_EstadoElementoId",
                        column: x => x.EstadoElementoId,
                        principalTable: "EstadosElementos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ElementosGeologicos_GaleriaElementosGeologicos_GaleriaElementosGeologicoId",
                        column: x => x.GaleriaElementosGeologicoId,
                        principalTable: "GaleriaElementosGeologicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ElementosGeologicos_Ubicaciones_UbicacionId",
                        column: x => x.UbicacionId,
                        principalTable: "Ubicaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Accesos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: true),
                    ElementoGeologicoId = table.Column<int>(type: "int", nullable: true),
                    FechaAcceso = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Accion = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accesos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Accesos_ElementosGeologicos_ElementoGeologicoId",
                        column: x => x.ElementoGeologicoId,
                        principalTable: "ElementosGeologicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Accesos_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accesos_ElementoGeologicoId",
                table: "Accesos",
                column: "ElementoGeologicoId");

            migrationBuilder.CreateIndex(
                name: "IX_Accesos_UsuarioId",
                table: "Accesos",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_ElementosGeologicos_EstadoElementoId",
                table: "ElementosGeologicos",
                column: "EstadoElementoId");

            migrationBuilder.CreateIndex(
                name: "IX_ElementosGeologicos_GaleriaElementosGeologicoId",
                table: "ElementosGeologicos",
                column: "GaleriaElementosGeologicoId",
                unique: true,
                filter: "[GaleriaElementosGeologicoId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ElementosGeologicos_UbicacionId",
                table: "ElementosGeologicos",
                column: "UbicacionId");

            migrationBuilder.CreateIndex(
                name: "IX_FotosElementos_GaleriaElementosGeologicoId",
                table: "FotosElementos",
                column: "GaleriaElementosGeologicoId");

            migrationBuilder.CreateIndex(
                name: "IX_Provincias_PaisId",
                table: "Provincias",
                column: "PaisId");

            migrationBuilder.CreateIndex(
                name: "IX_Ubicaciones_PaisId",
                table: "Ubicaciones",
                column: "PaisId");

            migrationBuilder.CreateIndex(
                name: "IX_Ubicaciones_ProvinciaId",
                table: "Ubicaciones",
                column: "ProvinciaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Accesos");

            migrationBuilder.DropTable(
                name: "FotosElementos");

            migrationBuilder.DropTable(
                name: "ElementosGeologicos");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "EstadosElementos");

            migrationBuilder.DropTable(
                name: "GaleriaElementosGeologicos");

            migrationBuilder.DropTable(
                name: "Ubicaciones");

            migrationBuilder.DropTable(
                name: "Provincias");

            migrationBuilder.DropTable(
                name: "Paises");
        }
    }
}
