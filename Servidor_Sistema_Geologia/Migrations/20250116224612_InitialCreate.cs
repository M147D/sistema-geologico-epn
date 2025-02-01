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
                    DescripcionEstado = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadosElementos", x => x.Id);
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
                    Nombres = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Apellidos = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NombreUsuario = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CorreoUsuario = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Rol = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
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
                        principalColumn: "Id");
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
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Ubicaciones_Provincias_ProvinciaId",
                        column: x => x.ProvinciaId,
                        principalTable: "Provincias",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ElementosGeologicos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EstadoElementoId = table.Column<int>(type: "int", nullable: true),
                    UbicacionId = table.Column<int>(type: "int", nullable: true),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Edad = table.Column<int>(type: "int", nullable: true),
                    Donante = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaIngreso = table.Column<int>(type: "int", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ejemplares = table.Column<int>(type: "int", nullable: true),
                    DocumentosRelacionados = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LaminaURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LaminaExiste = table.Column<bool>(type: "bit", nullable: true),
                    Discriminator = table.Column<string>(type: "nvarchar(21)", maxLength: 21, nullable: false),
                    Especie = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Periodo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TipoRoca = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Litologia = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElementosGeologicos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ElementosGeologicos_EstadosElementos_EstadoElementoId",
                        column: x => x.EstadoElementoId,
                        principalTable: "EstadosElementos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ElementosGeologicos_Ubicaciones_UbicacionId",
                        column: x => x.UbicacionId,
                        principalTable: "Ubicaciones",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Accesos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: true),
                    ElementoGeologicoId = table.Column<int>(type: "int", nullable: true),
                    FechaAcceso = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Accion = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accesos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Accesos_ElementosGeologicos_ElementoGeologicoId",
                        column: x => x.ElementoGeologicoId,
                        principalTable: "ElementosGeologicos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Accesos_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id");
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
                    table.ForeignKey(
                        name: "FK_GaleriaElementosGeologicos_ElementosGeologicos_ElementoGeologicoId",
                        column: x => x.ElementoGeologicoId,
                        principalTable: "ElementosGeologicos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FotosElementos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ElementoGeologicoId = table.Column<int>(type: "int", nullable: true),
                    GaleriaElementosGeologicoId = table.Column<int>(type: "int", nullable: true),
                    Imagen = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    TipoFoto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaSubida = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreadoPor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescripcionEspecifica = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Etiquetas = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GaleriaId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FotosElementos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FotosElementos_ElementosGeologicos_ElementoGeologicoId",
                        column: x => x.ElementoGeologicoId,
                        principalTable: "ElementosGeologicos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FotosElementos_GaleriaElementosGeologicos_GaleriaId",
                        column: x => x.GaleriaId,
                        principalTable: "GaleriaElementosGeologicos",
                        principalColumn: "Id");
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
                name: "IX_ElementosGeologicos_UbicacionId",
                table: "ElementosGeologicos",
                column: "UbicacionId");

            migrationBuilder.CreateIndex(
                name: "IX_FotosElementos_ElementoGeologicoId",
                table: "FotosElementos",
                column: "ElementoGeologicoId");

            migrationBuilder.CreateIndex(
                name: "IX_FotosElementos_GaleriaId",
                table: "FotosElementos",
                column: "GaleriaId");

            migrationBuilder.CreateIndex(
                name: "IX_GaleriaElementosGeologicos_ElementoGeologicoId",
                table: "GaleriaElementosGeologicos",
                column: "ElementoGeologicoId");

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
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "GaleriaElementosGeologicos");

            migrationBuilder.DropTable(
                name: "ElementosGeologicos");

            migrationBuilder.DropTable(
                name: "EstadosElementos");

            migrationBuilder.DropTable(
                name: "Ubicaciones");

            migrationBuilder.DropTable(
                name: "Provincias");

            migrationBuilder.DropTable(
                name: "Paises");
        }
    }
}
