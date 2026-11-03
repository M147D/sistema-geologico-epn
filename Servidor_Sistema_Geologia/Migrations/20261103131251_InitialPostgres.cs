using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Servidor_Sistema_Geologia.Migrations
{
    /// <inheritdoc />
    public partial class InitialPostgres : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NombreCompleto = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Rol = table.Column<int>(type: "integer", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EstadoActivo = table.Column<bool>(type: "boolean", nullable: false),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RolClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<int>(type: "integer", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RolClaims_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tb_Paises",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombrePais = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    fechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    usuarioCreacionId = table.Column<int>(type: "integer", nullable: true),
                    fechaActualizacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    usuarioActualizacionId = table.Column<int>(type: "integer", nullable: true),
                    fechaEliminacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    usuarioEliminacionId = table.Column<int>(type: "integer", nullable: true),
                    estadoActivo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_Paises", x => x.id);
                    table.ForeignKey(
                        name: "FK_tb_Paises_Usuarios_usuarioActualizacionId",
                        column: x => x.usuarioActualizacionId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tb_Paises_Usuarios_usuarioCreacionId",
                        column: x => x.usuarioCreacionId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tb_Paises_Usuarios_usuarioEliminacionId",
                        column: x => x.usuarioEliminacionId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsuarioClaims_Usuarios_UserId",
                        column: x => x.UserId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_UsuarioLogins_Usuarios_UserId",
                        column: x => x.UserId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    RoleId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UsuarioRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuarioRoles_Usuarios_UserId",
                        column: x => x.UserId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioTokens",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_UsuarioTokens_Usuarios_UserId",
                        column: x => x.UserId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tb_Provincias",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    paisId = table.Column<int>(type: "integer", nullable: false),
                    nombreProvincia = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    fechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    usuarioCreacionId = table.Column<int>(type: "integer", nullable: true),
                    fechaActualizacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    usuarioActualizacionId = table.Column<int>(type: "integer", nullable: true),
                    fechaEliminacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    usuarioEliminacionId = table.Column<int>(type: "integer", nullable: true),
                    estadoActivo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_Provincias", x => x.id);
                    table.ForeignKey(
                        name: "FK_tb_Provincias_Usuarios_usuarioActualizacionId",
                        column: x => x.usuarioActualizacionId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tb_Provincias_Usuarios_usuarioCreacionId",
                        column: x => x.usuarioCreacionId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tb_Provincias_Usuarios_usuarioEliminacionId",
                        column: x => x.usuarioEliminacionId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tb_Provincias_tb_Paises_paisId",
                        column: x => x.paisId,
                        principalTable: "tb_Paises",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tb_Ubicaciones",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    paisId = table.Column<int>(type: "integer", nullable: false),
                    provinciaId = table.Column<int>(type: "integer", nullable: false),
                    latitud = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    longitud = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    localidad = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    leyenda = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    fechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    usuarioCreacionId = table.Column<int>(type: "integer", nullable: true),
                    fechaActualizacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    usuarioActualizacionId = table.Column<int>(type: "integer", nullable: true),
                    fechaEliminacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    usuarioEliminacionId = table.Column<int>(type: "integer", nullable: true),
                    estadoActivo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_Ubicaciones", x => x.id);
                    table.ForeignKey(
                        name: "FK_tb_Ubicaciones_Usuarios_usuarioActualizacionId",
                        column: x => x.usuarioActualizacionId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tb_Ubicaciones_Usuarios_usuarioCreacionId",
                        column: x => x.usuarioCreacionId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tb_Ubicaciones_Usuarios_usuarioEliminacionId",
                        column: x => x.usuarioEliminacionId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tb_Ubicaciones_tb_Paises_paisId",
                        column: x => x.paisId,
                        principalTable: "tb_Paises",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tb_Ubicaciones_tb_Provincias_provinciaId",
                        column: x => x.provinciaId,
                        principalTable: "tb_Provincias",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tb_ElementosGeologicos",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    edad = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    donante = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    fechaIngreso = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    codigo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ejemplares = table.Column<long>(type: "bigint", nullable: false),
                    documentosRelacionados = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    laminaExiste = table.Column<bool>(type: "boolean", nullable: false),
                    ubicacionId = table.Column<int>(type: "integer", nullable: true),
                    tipoElemento = table.Column<string>(type: "character varying(21)", maxLength: 21, nullable: false),
                    tipoFosil = table.Column<int>(type: "integer", nullable: true),
                    especie = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    periodo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    tipoMineral = table.Column<int>(type: "integer", nullable: true),
                    litologia = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    tipoRoca = table.Column<int>(type: "integer", nullable: true),
                    litologia_roca = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    fechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "LOCALTIMESTAMP"),
                    usuarioCreacionId = table.Column<int>(type: "integer", nullable: true),
                    fechaActualizacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    usuarioActualizacionId = table.Column<int>(type: "integer", nullable: true),
                    fechaEliminacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    usuarioEliminacionId = table.Column<int>(type: "integer", nullable: true),
                    estadoActivo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_ElementosGeologicos", x => x.id);
                    table.ForeignKey(
                        name: "FK_tb_ElementosGeologicos_Usuarios_usuarioActualizacionId",
                        column: x => x.usuarioActualizacionId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tb_ElementosGeologicos_Usuarios_usuarioCreacionId",
                        column: x => x.usuarioCreacionId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tb_ElementosGeologicos_Usuarios_usuarioEliminacionId",
                        column: x => x.usuarioEliminacionId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tb_ElementosGeologicos_tb_Ubicaciones_ubicacionId",
                        column: x => x.ubicacionId,
                        principalTable: "tb_Ubicaciones",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tb_GaleriaElementosGeologicos",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    elementoGeologicoId = table.Column<int>(type: "integer", nullable: false),
                    detalleGrupo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    fechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    usuarioCreacionId = table.Column<int>(type: "integer", nullable: true),
                    fechaActualizacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    usuarioActualizacionId = table.Column<int>(type: "integer", nullable: true),
                    fechaEliminacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    usuarioEliminacionId = table.Column<int>(type: "integer", nullable: true),
                    estadoActivo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_GaleriaElementosGeologicos", x => x.id);
                    table.ForeignKey(
                        name: "FK_tb_GaleriaElementosGeologicos_Usuarios_usuarioActualizacion~",
                        column: x => x.usuarioActualizacionId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tb_GaleriaElementosGeologicos_Usuarios_usuarioCreacionId",
                        column: x => x.usuarioCreacionId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tb_GaleriaElementosGeologicos_Usuarios_usuarioEliminacionId",
                        column: x => x.usuarioEliminacionId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tb_GaleriaElementosGeologicos_tb_ElementosGeologicos_elemen~",
                        column: x => x.elementoGeologicoId,
                        principalTable: "tb_ElementosGeologicos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tb_FotosElementos",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    galeriaElementosGeologicoId = table.Column<int>(type: "integer", nullable: false),
                    imagen = table.Column<byte[]>(type: "bytea", nullable: false),
                    tipoFoto = table.Column<int>(type: "integer", nullable: false),
                    descripcionEspecifica = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    fechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    usuarioCreacionId = table.Column<int>(type: "integer", nullable: true),
                    fechaActualizacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    usuarioActualizacionId = table.Column<int>(type: "integer", nullable: true),
                    fechaEliminacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    usuarioEliminacionId = table.Column<int>(type: "integer", nullable: true),
                    estadoActivo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_FotosElementos", x => x.id);
                    table.ForeignKey(
                        name: "FK_tb_FotosElementos_Usuarios_usuarioActualizacionId",
                        column: x => x.usuarioActualizacionId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tb_FotosElementos_Usuarios_usuarioCreacionId",
                        column: x => x.usuarioCreacionId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tb_FotosElementos_Usuarios_usuarioEliminacionId",
                        column: x => x.usuarioEliminacionId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tb_FotosElementos_tb_GaleriaElementosGeologicos_galeriaElem~",
                        column: x => x.galeriaElementosGeologicoId,
                        principalTable: "tb_GaleriaElementosGeologicos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RolClaims_RoleId",
                table: "RolClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "Roles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ElementosGeologicos_Codigo",
                table: "tb_ElementosGeologicos",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tb_ElementosGeologicos_ubicacionId",
                table: "tb_ElementosGeologicos",
                column: "ubicacionId");

            migrationBuilder.CreateIndex(
                name: "IX_tb_ElementosGeologicos_usuarioActualizacionId",
                table: "tb_ElementosGeologicos",
                column: "usuarioActualizacionId");

            migrationBuilder.CreateIndex(
                name: "IX_tb_ElementosGeologicos_usuarioCreacionId",
                table: "tb_ElementosGeologicos",
                column: "usuarioCreacionId");

            migrationBuilder.CreateIndex(
                name: "IX_tb_ElementosGeologicos_usuarioEliminacionId",
                table: "tb_ElementosGeologicos",
                column: "usuarioEliminacionId");

            migrationBuilder.CreateIndex(
                name: "IX_tb_FotosElementos_galeriaElementosGeologicoId",
                table: "tb_FotosElementos",
                column: "galeriaElementosGeologicoId");

            migrationBuilder.CreateIndex(
                name: "IX_tb_FotosElementos_usuarioActualizacionId",
                table: "tb_FotosElementos",
                column: "usuarioActualizacionId");

            migrationBuilder.CreateIndex(
                name: "IX_tb_FotosElementos_usuarioCreacionId",
                table: "tb_FotosElementos",
                column: "usuarioCreacionId");

            migrationBuilder.CreateIndex(
                name: "IX_tb_FotosElementos_usuarioEliminacionId",
                table: "tb_FotosElementos",
                column: "usuarioEliminacionId");

            migrationBuilder.CreateIndex(
                name: "IX_tb_GaleriaElementosGeologicos_elementoGeologicoId",
                table: "tb_GaleriaElementosGeologicos",
                column: "elementoGeologicoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tb_GaleriaElementosGeologicos_usuarioActualizacionId",
                table: "tb_GaleriaElementosGeologicos",
                column: "usuarioActualizacionId");

            migrationBuilder.CreateIndex(
                name: "IX_tb_GaleriaElementosGeologicos_usuarioCreacionId",
                table: "tb_GaleriaElementosGeologicos",
                column: "usuarioCreacionId");

            migrationBuilder.CreateIndex(
                name: "IX_tb_GaleriaElementosGeologicos_usuarioEliminacionId",
                table: "tb_GaleriaElementosGeologicos",
                column: "usuarioEliminacionId");

            migrationBuilder.CreateIndex(
                name: "IX_tb_Paises_usuarioActualizacionId",
                table: "tb_Paises",
                column: "usuarioActualizacionId");

            migrationBuilder.CreateIndex(
                name: "IX_tb_Paises_usuarioCreacionId",
                table: "tb_Paises",
                column: "usuarioCreacionId");

            migrationBuilder.CreateIndex(
                name: "IX_tb_Paises_usuarioEliminacionId",
                table: "tb_Paises",
                column: "usuarioEliminacionId");

            migrationBuilder.CreateIndex(
                name: "IX_tb_Provincias_paisId",
                table: "tb_Provincias",
                column: "paisId");

            migrationBuilder.CreateIndex(
                name: "IX_tb_Provincias_usuarioActualizacionId",
                table: "tb_Provincias",
                column: "usuarioActualizacionId");

            migrationBuilder.CreateIndex(
                name: "IX_tb_Provincias_usuarioCreacionId",
                table: "tb_Provincias",
                column: "usuarioCreacionId");

            migrationBuilder.CreateIndex(
                name: "IX_tb_Provincias_usuarioEliminacionId",
                table: "tb_Provincias",
                column: "usuarioEliminacionId");

            migrationBuilder.CreateIndex(
                name: "IX_tb_Ubicaciones_paisId",
                table: "tb_Ubicaciones",
                column: "paisId");

            migrationBuilder.CreateIndex(
                name: "IX_tb_Ubicaciones_provinciaId",
                table: "tb_Ubicaciones",
                column: "provinciaId");

            migrationBuilder.CreateIndex(
                name: "IX_tb_Ubicaciones_usuarioActualizacionId",
                table: "tb_Ubicaciones",
                column: "usuarioActualizacionId");

            migrationBuilder.CreateIndex(
                name: "IX_tb_Ubicaciones_usuarioCreacionId",
                table: "tb_Ubicaciones",
                column: "usuarioCreacionId");

            migrationBuilder.CreateIndex(
                name: "IX_tb_Ubicaciones_usuarioEliminacionId",
                table: "tb_Ubicaciones",
                column: "usuarioEliminacionId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioClaims_UserId",
                table: "UsuarioClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioLogins_UserId",
                table: "UsuarioLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioRoles_RoleId",
                table: "UsuarioRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "Usuarios",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "Usuarios",
                column: "NormalizedUserName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RolClaims");

            migrationBuilder.DropTable(
                name: "tb_FotosElementos");

            migrationBuilder.DropTable(
                name: "UsuarioClaims");

            migrationBuilder.DropTable(
                name: "UsuarioLogins");

            migrationBuilder.DropTable(
                name: "UsuarioRoles");

            migrationBuilder.DropTable(
                name: "UsuarioTokens");

            migrationBuilder.DropTable(
                name: "tb_GaleriaElementosGeologicos");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "tb_ElementosGeologicos");

            migrationBuilder.DropTable(
                name: "tb_Ubicaciones");

            migrationBuilder.DropTable(
                name: "tb_Provincias");

            migrationBuilder.DropTable(
                name: "tb_Paises");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
