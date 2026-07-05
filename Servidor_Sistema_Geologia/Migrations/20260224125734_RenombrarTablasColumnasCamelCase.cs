using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Servidor_Sistema_Geologia.Migrations
{
    /// <inheritdoc />
    public partial class RenombrarTablasColumnasCamelCase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ElementosGeologicos_Ubicaciones_UbicacionId",
                table: "ElementosGeologicos");

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
                name: "FK_FotosElementos_GaleriaElementosGeologicos_GaleriaElementosGeologicoId",
                table: "FotosElementos");

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
                name: "FK_Provincias_Paises_PaisId",
                table: "Provincias");

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
                name: "FK_Ubicaciones_Paises_PaisId",
                table: "Ubicaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_Ubicaciones_Provincias_ProvinciaId",
                table: "Ubicaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_Ubicaciones_Usuarios_UsuarioActualizacionId",
                table: "Ubicaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_Ubicaciones_Usuarios_UsuarioCreacionId",
                table: "Ubicaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_Ubicaciones_Usuarios_UsuarioEliminacionId",
                table: "Ubicaciones");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Ubicaciones",
                table: "Ubicaciones");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Provincias",
                table: "Provincias");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Paises",
                table: "Paises");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GaleriaElementosGeologicos",
                table: "GaleriaElementosGeologicos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FotosElementos",
                table: "FotosElementos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ElementosGeologicos",
                table: "ElementosGeologicos");

            migrationBuilder.RenameTable(
                name: "Ubicaciones",
                newName: "tb_Ubicaciones");

            migrationBuilder.RenameTable(
                name: "Provincias",
                newName: "tb_Provincias");

            migrationBuilder.RenameTable(
                name: "Paises",
                newName: "tb_Paises");

            migrationBuilder.RenameTable(
                name: "GaleriaElementosGeologicos",
                newName: "tb_GaleriaElementosGeologicos");

            migrationBuilder.RenameTable(
                name: "FotosElementos",
                newName: "tb_FotosElementos");

            migrationBuilder.RenameTable(
                name: "ElementosGeologicos",
                newName: "tb_ElementosGeologicos");

            migrationBuilder.RenameColumn(
                name: "UsuarioEliminacionId",
                table: "tb_Ubicaciones",
                newName: "usuarioEliminacionId");

            migrationBuilder.RenameColumn(
                name: "UsuarioCreacionId",
                table: "tb_Ubicaciones",
                newName: "usuarioCreacionId");

            migrationBuilder.RenameColumn(
                name: "UsuarioActualizacionId",
                table: "tb_Ubicaciones",
                newName: "usuarioActualizacionId");

            migrationBuilder.RenameColumn(
                name: "ProvinciaId",
                table: "tb_Ubicaciones",
                newName: "provinciaId");

            migrationBuilder.RenameColumn(
                name: "PaisId",
                table: "tb_Ubicaciones",
                newName: "paisId");

            migrationBuilder.RenameColumn(
                name: "Longitud",
                table: "tb_Ubicaciones",
                newName: "longitud");

            migrationBuilder.RenameColumn(
                name: "Localidad",
                table: "tb_Ubicaciones",
                newName: "localidad");

            migrationBuilder.RenameColumn(
                name: "Leyenda",
                table: "tb_Ubicaciones",
                newName: "leyenda");

            migrationBuilder.RenameColumn(
                name: "Latitud",
                table: "tb_Ubicaciones",
                newName: "latitud");

            migrationBuilder.RenameColumn(
                name: "FechaEliminacion",
                table: "tb_Ubicaciones",
                newName: "fechaEliminacion");

            migrationBuilder.RenameColumn(
                name: "FechaCreacion",
                table: "tb_Ubicaciones",
                newName: "fechaCreacion");

            migrationBuilder.RenameColumn(
                name: "FechaActualizacion",
                table: "tb_Ubicaciones",
                newName: "fechaActualizacion");

            migrationBuilder.RenameColumn(
                name: "EstadoActivo",
                table: "tb_Ubicaciones",
                newName: "estadoActivo");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "tb_Ubicaciones",
                newName: "id");

            migrationBuilder.RenameIndex(
                name: "IX_Ubicaciones_UsuarioEliminacionId",
                table: "tb_Ubicaciones",
                newName: "IX_tb_Ubicaciones_usuarioEliminacionId");

            migrationBuilder.RenameIndex(
                name: "IX_Ubicaciones_UsuarioCreacionId",
                table: "tb_Ubicaciones",
                newName: "IX_tb_Ubicaciones_usuarioCreacionId");

            migrationBuilder.RenameIndex(
                name: "IX_Ubicaciones_UsuarioActualizacionId",
                table: "tb_Ubicaciones",
                newName: "IX_tb_Ubicaciones_usuarioActualizacionId");

            migrationBuilder.RenameIndex(
                name: "IX_Ubicaciones_ProvinciaId",
                table: "tb_Ubicaciones",
                newName: "IX_tb_Ubicaciones_provinciaId");

            migrationBuilder.RenameIndex(
                name: "IX_Ubicaciones_PaisId",
                table: "tb_Ubicaciones",
                newName: "IX_tb_Ubicaciones_paisId");

            migrationBuilder.RenameColumn(
                name: "UsuarioEliminacionId",
                table: "tb_Provincias",
                newName: "usuarioEliminacionId");

            migrationBuilder.RenameColumn(
                name: "UsuarioCreacionId",
                table: "tb_Provincias",
                newName: "usuarioCreacionId");

            migrationBuilder.RenameColumn(
                name: "UsuarioActualizacionId",
                table: "tb_Provincias",
                newName: "usuarioActualizacionId");

            migrationBuilder.RenameColumn(
                name: "PaisId",
                table: "tb_Provincias",
                newName: "paisId");

            migrationBuilder.RenameColumn(
                name: "NombreProvincia",
                table: "tb_Provincias",
                newName: "nombreProvincia");

            migrationBuilder.RenameColumn(
                name: "FechaEliminacion",
                table: "tb_Provincias",
                newName: "fechaEliminacion");

            migrationBuilder.RenameColumn(
                name: "FechaCreacion",
                table: "tb_Provincias",
                newName: "fechaCreacion");

            migrationBuilder.RenameColumn(
                name: "FechaActualizacion",
                table: "tb_Provincias",
                newName: "fechaActualizacion");

            migrationBuilder.RenameColumn(
                name: "EstadoActivo",
                table: "tb_Provincias",
                newName: "estadoActivo");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "tb_Provincias",
                newName: "id");

            migrationBuilder.RenameIndex(
                name: "IX_Provincias_UsuarioEliminacionId",
                table: "tb_Provincias",
                newName: "IX_tb_Provincias_usuarioEliminacionId");

            migrationBuilder.RenameIndex(
                name: "IX_Provincias_UsuarioCreacionId",
                table: "tb_Provincias",
                newName: "IX_tb_Provincias_usuarioCreacionId");

            migrationBuilder.RenameIndex(
                name: "IX_Provincias_UsuarioActualizacionId",
                table: "tb_Provincias",
                newName: "IX_tb_Provincias_usuarioActualizacionId");

            migrationBuilder.RenameIndex(
                name: "IX_Provincias_PaisId",
                table: "tb_Provincias",
                newName: "IX_tb_Provincias_paisId");

            migrationBuilder.RenameColumn(
                name: "UsuarioEliminacionId",
                table: "tb_Paises",
                newName: "usuarioEliminacionId");

            migrationBuilder.RenameColumn(
                name: "UsuarioCreacionId",
                table: "tb_Paises",
                newName: "usuarioCreacionId");

            migrationBuilder.RenameColumn(
                name: "UsuarioActualizacionId",
                table: "tb_Paises",
                newName: "usuarioActualizacionId");

            migrationBuilder.RenameColumn(
                name: "NombrePais",
                table: "tb_Paises",
                newName: "nombrePais");

            migrationBuilder.RenameColumn(
                name: "FechaEliminacion",
                table: "tb_Paises",
                newName: "fechaEliminacion");

            migrationBuilder.RenameColumn(
                name: "FechaCreacion",
                table: "tb_Paises",
                newName: "fechaCreacion");

            migrationBuilder.RenameColumn(
                name: "FechaActualizacion",
                table: "tb_Paises",
                newName: "fechaActualizacion");

            migrationBuilder.RenameColumn(
                name: "EstadoActivo",
                table: "tb_Paises",
                newName: "estadoActivo");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "tb_Paises",
                newName: "id");

            migrationBuilder.RenameIndex(
                name: "IX_Paises_UsuarioEliminacionId",
                table: "tb_Paises",
                newName: "IX_tb_Paises_usuarioEliminacionId");

            migrationBuilder.RenameIndex(
                name: "IX_Paises_UsuarioCreacionId",
                table: "tb_Paises",
                newName: "IX_tb_Paises_usuarioCreacionId");

            migrationBuilder.RenameIndex(
                name: "IX_Paises_UsuarioActualizacionId",
                table: "tb_Paises",
                newName: "IX_tb_Paises_usuarioActualizacionId");

            migrationBuilder.RenameColumn(
                name: "UsuarioEliminacionId",
                table: "tb_GaleriaElementosGeologicos",
                newName: "usuarioEliminacionId");

            migrationBuilder.RenameColumn(
                name: "UsuarioCreacionId",
                table: "tb_GaleriaElementosGeologicos",
                newName: "usuarioCreacionId");

            migrationBuilder.RenameColumn(
                name: "UsuarioActualizacionId",
                table: "tb_GaleriaElementosGeologicos",
                newName: "usuarioActualizacionId");

            migrationBuilder.RenameColumn(
                name: "FechaEliminacion",
                table: "tb_GaleriaElementosGeologicos",
                newName: "fechaEliminacion");

            migrationBuilder.RenameColumn(
                name: "FechaCreacion",
                table: "tb_GaleriaElementosGeologicos",
                newName: "fechaCreacion");

            migrationBuilder.RenameColumn(
                name: "FechaActualizacion",
                table: "tb_GaleriaElementosGeologicos",
                newName: "fechaActualizacion");

            migrationBuilder.RenameColumn(
                name: "EstadoActivo",
                table: "tb_GaleriaElementosGeologicos",
                newName: "estadoActivo");

            migrationBuilder.RenameColumn(
                name: "ElementoGeologicoId",
                table: "tb_GaleriaElementosGeologicos",
                newName: "elementoGeologicoId");

            migrationBuilder.RenameColumn(
                name: "DetalleGrupo",
                table: "tb_GaleriaElementosGeologicos",
                newName: "detalleGrupo");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "tb_GaleriaElementosGeologicos",
                newName: "id");

            migrationBuilder.RenameIndex(
                name: "IX_GaleriaElementosGeologicos_UsuarioEliminacionId",
                table: "tb_GaleriaElementosGeologicos",
                newName: "IX_tb_GaleriaElementosGeologicos_usuarioEliminacionId");

            migrationBuilder.RenameIndex(
                name: "IX_GaleriaElementosGeologicos_UsuarioCreacionId",
                table: "tb_GaleriaElementosGeologicos",
                newName: "IX_tb_GaleriaElementosGeologicos_usuarioCreacionId");

            migrationBuilder.RenameIndex(
                name: "IX_GaleriaElementosGeologicos_UsuarioActualizacionId",
                table: "tb_GaleriaElementosGeologicos",
                newName: "IX_tb_GaleriaElementosGeologicos_usuarioActualizacionId");

            migrationBuilder.RenameIndex(
                name: "IX_GaleriaElementosGeologicos_ElementoGeologicoId",
                table: "tb_GaleriaElementosGeologicos",
                newName: "IX_tb_GaleriaElementosGeologicos_elementoGeologicoId");

            migrationBuilder.RenameColumn(
                name: "UsuarioEliminacionId",
                table: "tb_FotosElementos",
                newName: "usuarioEliminacionId");

            migrationBuilder.RenameColumn(
                name: "UsuarioCreacionId",
                table: "tb_FotosElementos",
                newName: "usuarioCreacionId");

            migrationBuilder.RenameColumn(
                name: "UsuarioActualizacionId",
                table: "tb_FotosElementos",
                newName: "usuarioActualizacionId");

            migrationBuilder.RenameColumn(
                name: "TipoFoto",
                table: "tb_FotosElementos",
                newName: "tipoFoto");

            migrationBuilder.RenameColumn(
                name: "Imagen",
                table: "tb_FotosElementos",
                newName: "imagen");

            migrationBuilder.RenameColumn(
                name: "GaleriaElementosGeologicoId",
                table: "tb_FotosElementos",
                newName: "galeriaElementosGeologicoId");

            migrationBuilder.RenameColumn(
                name: "FechaEliminacion",
                table: "tb_FotosElementos",
                newName: "fechaEliminacion");

            migrationBuilder.RenameColumn(
                name: "FechaCreacion",
                table: "tb_FotosElementos",
                newName: "fechaCreacion");

            migrationBuilder.RenameColumn(
                name: "FechaActualizacion",
                table: "tb_FotosElementos",
                newName: "fechaActualizacion");

            migrationBuilder.RenameColumn(
                name: "EstadoActivo",
                table: "tb_FotosElementos",
                newName: "estadoActivo");

            migrationBuilder.RenameColumn(
                name: "DescripcionEspecifica",
                table: "tb_FotosElementos",
                newName: "descripcionEspecifica");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "tb_FotosElementos",
                newName: "id");

            migrationBuilder.RenameIndex(
                name: "IX_FotosElementos_UsuarioEliminacionId",
                table: "tb_FotosElementos",
                newName: "IX_tb_FotosElementos_usuarioEliminacionId");

            migrationBuilder.RenameIndex(
                name: "IX_FotosElementos_UsuarioCreacionId",
                table: "tb_FotosElementos",
                newName: "IX_tb_FotosElementos_usuarioCreacionId");

            migrationBuilder.RenameIndex(
                name: "IX_FotosElementos_UsuarioActualizacionId",
                table: "tb_FotosElementos",
                newName: "IX_tb_FotosElementos_usuarioActualizacionId");

            migrationBuilder.RenameIndex(
                name: "IX_FotosElementos_GaleriaElementosGeologicoId",
                table: "tb_FotosElementos",
                newName: "IX_tb_FotosElementos_galeriaElementosGeologicoId");

            migrationBuilder.RenameColumn(
                name: "UsuarioEliminacionId",
                table: "tb_ElementosGeologicos",
                newName: "usuarioEliminacionId");

            migrationBuilder.RenameColumn(
                name: "UsuarioCreacionId",
                table: "tb_ElementosGeologicos",
                newName: "usuarioCreacionId");

            migrationBuilder.RenameColumn(
                name: "UsuarioActualizacionId",
                table: "tb_ElementosGeologicos",
                newName: "usuarioActualizacionId");

            migrationBuilder.RenameColumn(
                name: "UbicacionId",
                table: "tb_ElementosGeologicos",
                newName: "ubicacionId");

            migrationBuilder.RenameColumn(
                name: "TipoRoca",
                table: "tb_ElementosGeologicos",
                newName: "tipoRoca");

            migrationBuilder.RenameColumn(
                name: "TipoMineral",
                table: "tb_ElementosGeologicos",
                newName: "tipoMineral");

            migrationBuilder.RenameColumn(
                name: "TipoFosil",
                table: "tb_ElementosGeologicos",
                newName: "tipoFosil");

            migrationBuilder.RenameColumn(
                name: "TipoElemento",
                table: "tb_ElementosGeologicos",
                newName: "tipoElemento");

            migrationBuilder.RenameColumn(
                name: "Periodo",
                table: "tb_ElementosGeologicos",
                newName: "periodo");

            migrationBuilder.RenameColumn(
                name: "Nombre",
                table: "tb_ElementosGeologicos",
                newName: "nombre");

            migrationBuilder.RenameColumn(
                name: "Litologia",
                table: "tb_ElementosGeologicos",
                newName: "litologia");

            migrationBuilder.RenameColumn(
                name: "LaminaExiste",
                table: "tb_ElementosGeologicos",
                newName: "laminaExiste");

            migrationBuilder.RenameColumn(
                name: "FechaIngreso",
                table: "tb_ElementosGeologicos",
                newName: "fechaIngreso");

            migrationBuilder.RenameColumn(
                name: "FechaEliminacion",
                table: "tb_ElementosGeologicos",
                newName: "fechaEliminacion");

            migrationBuilder.RenameColumn(
                name: "FechaCreacion",
                table: "tb_ElementosGeologicos",
                newName: "fechaCreacion");

            migrationBuilder.RenameColumn(
                name: "FechaActualizacion",
                table: "tb_ElementosGeologicos",
                newName: "fechaActualizacion");

            migrationBuilder.RenameColumn(
                name: "EstadoActivo",
                table: "tb_ElementosGeologicos",
                newName: "estadoActivo");

            migrationBuilder.RenameColumn(
                name: "Especie",
                table: "tb_ElementosGeologicos",
                newName: "especie");

            migrationBuilder.RenameColumn(
                name: "Ejemplares",
                table: "tb_ElementosGeologicos",
                newName: "ejemplares");

            migrationBuilder.RenameColumn(
                name: "Edad",
                table: "tb_ElementosGeologicos",
                newName: "edad");

            migrationBuilder.RenameColumn(
                name: "Donante",
                table: "tb_ElementosGeologicos",
                newName: "donante");

            migrationBuilder.RenameColumn(
                name: "DocumentosRelacionados",
                table: "tb_ElementosGeologicos",
                newName: "documentosRelacionados");

            migrationBuilder.RenameColumn(
                name: "Codigo",
                table: "tb_ElementosGeologicos",
                newName: "codigo");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "tb_ElementosGeologicos",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Roca_Litologia",
                table: "tb_ElementosGeologicos",
                newName: "litologia_roca");

            migrationBuilder.RenameIndex(
                name: "IX_ElementosGeologicos_UsuarioEliminacionId",
                table: "tb_ElementosGeologicos",
                newName: "IX_tb_ElementosGeologicos_usuarioEliminacionId");

            migrationBuilder.RenameIndex(
                name: "IX_ElementosGeologicos_UsuarioCreacionId",
                table: "tb_ElementosGeologicos",
                newName: "IX_tb_ElementosGeologicos_usuarioCreacionId");

            migrationBuilder.RenameIndex(
                name: "IX_ElementosGeologicos_UsuarioActualizacionId",
                table: "tb_ElementosGeologicos",
                newName: "IX_tb_ElementosGeologicos_usuarioActualizacionId");

            migrationBuilder.RenameIndex(
                name: "IX_ElementosGeologicos_UbicacionId",
                table: "tb_ElementosGeologicos",
                newName: "IX_tb_ElementosGeologicos_ubicacionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tb_Ubicaciones",
                table: "tb_Ubicaciones",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tb_Provincias",
                table: "tb_Provincias",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tb_Paises",
                table: "tb_Paises",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tb_GaleriaElementosGeologicos",
                table: "tb_GaleriaElementosGeologicos",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tb_FotosElementos",
                table: "tb_FotosElementos",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tb_ElementosGeologicos",
                table: "tb_ElementosGeologicos",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_tb_ElementosGeologicos_Usuarios_usuarioActualizacionId",
                table: "tb_ElementosGeologicos",
                column: "usuarioActualizacionId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_tb_ElementosGeologicos_Usuarios_usuarioCreacionId",
                table: "tb_ElementosGeologicos",
                column: "usuarioCreacionId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_tb_ElementosGeologicos_Usuarios_usuarioEliminacionId",
                table: "tb_ElementosGeologicos",
                column: "usuarioEliminacionId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_tb_ElementosGeologicos_tb_Ubicaciones_ubicacionId",
                table: "tb_ElementosGeologicos",
                column: "ubicacionId",
                principalTable: "tb_Ubicaciones",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_tb_FotosElementos_Usuarios_usuarioActualizacionId",
                table: "tb_FotosElementos",
                column: "usuarioActualizacionId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_tb_FotosElementos_Usuarios_usuarioCreacionId",
                table: "tb_FotosElementos",
                column: "usuarioCreacionId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_tb_FotosElementos_Usuarios_usuarioEliminacionId",
                table: "tb_FotosElementos",
                column: "usuarioEliminacionId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_tb_FotosElementos_tb_GaleriaElementosGeologicos_galeriaElementosGeologicoId",
                table: "tb_FotosElementos",
                column: "galeriaElementosGeologicoId",
                principalTable: "tb_GaleriaElementosGeologicos",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tb_GaleriaElementosGeologicos_Usuarios_usuarioActualizacionId",
                table: "tb_GaleriaElementosGeologicos",
                column: "usuarioActualizacionId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_tb_GaleriaElementosGeologicos_Usuarios_usuarioCreacionId",
                table: "tb_GaleriaElementosGeologicos",
                column: "usuarioCreacionId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_tb_GaleriaElementosGeologicos_Usuarios_usuarioEliminacionId",
                table: "tb_GaleriaElementosGeologicos",
                column: "usuarioEliminacionId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_tb_GaleriaElementosGeologicos_tb_ElementosGeologicos_elementoGeologicoId",
                table: "tb_GaleriaElementosGeologicos",
                column: "elementoGeologicoId",
                principalTable: "tb_ElementosGeologicos",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tb_Paises_Usuarios_usuarioActualizacionId",
                table: "tb_Paises",
                column: "usuarioActualizacionId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_tb_Paises_Usuarios_usuarioCreacionId",
                table: "tb_Paises",
                column: "usuarioCreacionId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_tb_Paises_Usuarios_usuarioEliminacionId",
                table: "tb_Paises",
                column: "usuarioEliminacionId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_tb_Provincias_Usuarios_usuarioActualizacionId",
                table: "tb_Provincias",
                column: "usuarioActualizacionId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_tb_Provincias_Usuarios_usuarioCreacionId",
                table: "tb_Provincias",
                column: "usuarioCreacionId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_tb_Provincias_Usuarios_usuarioEliminacionId",
                table: "tb_Provincias",
                column: "usuarioEliminacionId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_tb_Provincias_tb_Paises_paisId",
                table: "tb_Provincias",
                column: "paisId",
                principalTable: "tb_Paises",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_tb_Ubicaciones_Usuarios_usuarioActualizacionId",
                table: "tb_Ubicaciones",
                column: "usuarioActualizacionId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_tb_Ubicaciones_Usuarios_usuarioCreacionId",
                table: "tb_Ubicaciones",
                column: "usuarioCreacionId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_tb_Ubicaciones_Usuarios_usuarioEliminacionId",
                table: "tb_Ubicaciones",
                column: "usuarioEliminacionId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_tb_Ubicaciones_tb_Paises_paisId",
                table: "tb_Ubicaciones",
                column: "paisId",
                principalTable: "tb_Paises",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_tb_Ubicaciones_tb_Provincias_provinciaId",
                table: "tb_Ubicaciones",
                column: "provinciaId",
                principalTable: "tb_Provincias",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tb_ElementosGeologicos_Usuarios_usuarioActualizacionId",
                table: "tb_ElementosGeologicos");

            migrationBuilder.DropForeignKey(
                name: "FK_tb_ElementosGeologicos_Usuarios_usuarioCreacionId",
                table: "tb_ElementosGeologicos");

            migrationBuilder.DropForeignKey(
                name: "FK_tb_ElementosGeologicos_Usuarios_usuarioEliminacionId",
                table: "tb_ElementosGeologicos");

            migrationBuilder.DropForeignKey(
                name: "FK_tb_ElementosGeologicos_tb_Ubicaciones_ubicacionId",
                table: "tb_ElementosGeologicos");

            migrationBuilder.DropForeignKey(
                name: "FK_tb_FotosElementos_Usuarios_usuarioActualizacionId",
                table: "tb_FotosElementos");

            migrationBuilder.DropForeignKey(
                name: "FK_tb_FotosElementos_Usuarios_usuarioCreacionId",
                table: "tb_FotosElementos");

            migrationBuilder.DropForeignKey(
                name: "FK_tb_FotosElementos_Usuarios_usuarioEliminacionId",
                table: "tb_FotosElementos");

            migrationBuilder.DropForeignKey(
                name: "FK_tb_FotosElementos_tb_GaleriaElementosGeologicos_galeriaElementosGeologicoId",
                table: "tb_FotosElementos");

            migrationBuilder.DropForeignKey(
                name: "FK_tb_GaleriaElementosGeologicos_Usuarios_usuarioActualizacionId",
                table: "tb_GaleriaElementosGeologicos");

            migrationBuilder.DropForeignKey(
                name: "FK_tb_GaleriaElementosGeologicos_Usuarios_usuarioCreacionId",
                table: "tb_GaleriaElementosGeologicos");

            migrationBuilder.DropForeignKey(
                name: "FK_tb_GaleriaElementosGeologicos_Usuarios_usuarioEliminacionId",
                table: "tb_GaleriaElementosGeologicos");

            migrationBuilder.DropForeignKey(
                name: "FK_tb_GaleriaElementosGeologicos_tb_ElementosGeologicos_elementoGeologicoId",
                table: "tb_GaleriaElementosGeologicos");

            migrationBuilder.DropForeignKey(
                name: "FK_tb_Paises_Usuarios_usuarioActualizacionId",
                table: "tb_Paises");

            migrationBuilder.DropForeignKey(
                name: "FK_tb_Paises_Usuarios_usuarioCreacionId",
                table: "tb_Paises");

            migrationBuilder.DropForeignKey(
                name: "FK_tb_Paises_Usuarios_usuarioEliminacionId",
                table: "tb_Paises");

            migrationBuilder.DropForeignKey(
                name: "FK_tb_Provincias_Usuarios_usuarioActualizacionId",
                table: "tb_Provincias");

            migrationBuilder.DropForeignKey(
                name: "FK_tb_Provincias_Usuarios_usuarioCreacionId",
                table: "tb_Provincias");

            migrationBuilder.DropForeignKey(
                name: "FK_tb_Provincias_Usuarios_usuarioEliminacionId",
                table: "tb_Provincias");

            migrationBuilder.DropForeignKey(
                name: "FK_tb_Provincias_tb_Paises_paisId",
                table: "tb_Provincias");

            migrationBuilder.DropForeignKey(
                name: "FK_tb_Ubicaciones_Usuarios_usuarioActualizacionId",
                table: "tb_Ubicaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_tb_Ubicaciones_Usuarios_usuarioCreacionId",
                table: "tb_Ubicaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_tb_Ubicaciones_Usuarios_usuarioEliminacionId",
                table: "tb_Ubicaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_tb_Ubicaciones_tb_Paises_paisId",
                table: "tb_Ubicaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_tb_Ubicaciones_tb_Provincias_provinciaId",
                table: "tb_Ubicaciones");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tb_Ubicaciones",
                table: "tb_Ubicaciones");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tb_Provincias",
                table: "tb_Provincias");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tb_Paises",
                table: "tb_Paises");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tb_GaleriaElementosGeologicos",
                table: "tb_GaleriaElementosGeologicos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tb_FotosElementos",
                table: "tb_FotosElementos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tb_ElementosGeologicos",
                table: "tb_ElementosGeologicos");

            migrationBuilder.RenameTable(
                name: "tb_Ubicaciones",
                newName: "Ubicaciones");

            migrationBuilder.RenameTable(
                name: "tb_Provincias",
                newName: "Provincias");

            migrationBuilder.RenameTable(
                name: "tb_Paises",
                newName: "Paises");

            migrationBuilder.RenameTable(
                name: "tb_GaleriaElementosGeologicos",
                newName: "GaleriaElementosGeologicos");

            migrationBuilder.RenameTable(
                name: "tb_FotosElementos",
                newName: "FotosElementos");

            migrationBuilder.RenameTable(
                name: "tb_ElementosGeologicos",
                newName: "ElementosGeologicos");

            migrationBuilder.RenameColumn(
                name: "usuarioEliminacionId",
                table: "Ubicaciones",
                newName: "UsuarioEliminacionId");

            migrationBuilder.RenameColumn(
                name: "usuarioCreacionId",
                table: "Ubicaciones",
                newName: "UsuarioCreacionId");

            migrationBuilder.RenameColumn(
                name: "usuarioActualizacionId",
                table: "Ubicaciones",
                newName: "UsuarioActualizacionId");

            migrationBuilder.RenameColumn(
                name: "provinciaId",
                table: "Ubicaciones",
                newName: "ProvinciaId");

            migrationBuilder.RenameColumn(
                name: "paisId",
                table: "Ubicaciones",
                newName: "PaisId");

            migrationBuilder.RenameColumn(
                name: "longitud",
                table: "Ubicaciones",
                newName: "Longitud");

            migrationBuilder.RenameColumn(
                name: "localidad",
                table: "Ubicaciones",
                newName: "Localidad");

            migrationBuilder.RenameColumn(
                name: "leyenda",
                table: "Ubicaciones",
                newName: "Leyenda");

            migrationBuilder.RenameColumn(
                name: "latitud",
                table: "Ubicaciones",
                newName: "Latitud");

            migrationBuilder.RenameColumn(
                name: "fechaEliminacion",
                table: "Ubicaciones",
                newName: "FechaEliminacion");

            migrationBuilder.RenameColumn(
                name: "fechaCreacion",
                table: "Ubicaciones",
                newName: "FechaCreacion");

            migrationBuilder.RenameColumn(
                name: "fechaActualizacion",
                table: "Ubicaciones",
                newName: "FechaActualizacion");

            migrationBuilder.RenameColumn(
                name: "estadoActivo",
                table: "Ubicaciones",
                newName: "EstadoActivo");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Ubicaciones",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_tb_Ubicaciones_usuarioEliminacionId",
                table: "Ubicaciones",
                newName: "IX_Ubicaciones_UsuarioEliminacionId");

            migrationBuilder.RenameIndex(
                name: "IX_tb_Ubicaciones_usuarioCreacionId",
                table: "Ubicaciones",
                newName: "IX_Ubicaciones_UsuarioCreacionId");

            migrationBuilder.RenameIndex(
                name: "IX_tb_Ubicaciones_usuarioActualizacionId",
                table: "Ubicaciones",
                newName: "IX_Ubicaciones_UsuarioActualizacionId");

            migrationBuilder.RenameIndex(
                name: "IX_tb_Ubicaciones_provinciaId",
                table: "Ubicaciones",
                newName: "IX_Ubicaciones_ProvinciaId");

            migrationBuilder.RenameIndex(
                name: "IX_tb_Ubicaciones_paisId",
                table: "Ubicaciones",
                newName: "IX_Ubicaciones_PaisId");

            migrationBuilder.RenameColumn(
                name: "usuarioEliminacionId",
                table: "Provincias",
                newName: "UsuarioEliminacionId");

            migrationBuilder.RenameColumn(
                name: "usuarioCreacionId",
                table: "Provincias",
                newName: "UsuarioCreacionId");

            migrationBuilder.RenameColumn(
                name: "usuarioActualizacionId",
                table: "Provincias",
                newName: "UsuarioActualizacionId");

            migrationBuilder.RenameColumn(
                name: "paisId",
                table: "Provincias",
                newName: "PaisId");

            migrationBuilder.RenameColumn(
                name: "nombreProvincia",
                table: "Provincias",
                newName: "NombreProvincia");

            migrationBuilder.RenameColumn(
                name: "fechaEliminacion",
                table: "Provincias",
                newName: "FechaEliminacion");

            migrationBuilder.RenameColumn(
                name: "fechaCreacion",
                table: "Provincias",
                newName: "FechaCreacion");

            migrationBuilder.RenameColumn(
                name: "fechaActualizacion",
                table: "Provincias",
                newName: "FechaActualizacion");

            migrationBuilder.RenameColumn(
                name: "estadoActivo",
                table: "Provincias",
                newName: "EstadoActivo");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Provincias",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_tb_Provincias_usuarioEliminacionId",
                table: "Provincias",
                newName: "IX_Provincias_UsuarioEliminacionId");

            migrationBuilder.RenameIndex(
                name: "IX_tb_Provincias_usuarioCreacionId",
                table: "Provincias",
                newName: "IX_Provincias_UsuarioCreacionId");

            migrationBuilder.RenameIndex(
                name: "IX_tb_Provincias_usuarioActualizacionId",
                table: "Provincias",
                newName: "IX_Provincias_UsuarioActualizacionId");

            migrationBuilder.RenameIndex(
                name: "IX_tb_Provincias_paisId",
                table: "Provincias",
                newName: "IX_Provincias_PaisId");

            migrationBuilder.RenameColumn(
                name: "usuarioEliminacionId",
                table: "Paises",
                newName: "UsuarioEliminacionId");

            migrationBuilder.RenameColumn(
                name: "usuarioCreacionId",
                table: "Paises",
                newName: "UsuarioCreacionId");

            migrationBuilder.RenameColumn(
                name: "usuarioActualizacionId",
                table: "Paises",
                newName: "UsuarioActualizacionId");

            migrationBuilder.RenameColumn(
                name: "nombrePais",
                table: "Paises",
                newName: "NombrePais");

            migrationBuilder.RenameColumn(
                name: "fechaEliminacion",
                table: "Paises",
                newName: "FechaEliminacion");

            migrationBuilder.RenameColumn(
                name: "fechaCreacion",
                table: "Paises",
                newName: "FechaCreacion");

            migrationBuilder.RenameColumn(
                name: "fechaActualizacion",
                table: "Paises",
                newName: "FechaActualizacion");

            migrationBuilder.RenameColumn(
                name: "estadoActivo",
                table: "Paises",
                newName: "EstadoActivo");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Paises",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_tb_Paises_usuarioEliminacionId",
                table: "Paises",
                newName: "IX_Paises_UsuarioEliminacionId");

            migrationBuilder.RenameIndex(
                name: "IX_tb_Paises_usuarioCreacionId",
                table: "Paises",
                newName: "IX_Paises_UsuarioCreacionId");

            migrationBuilder.RenameIndex(
                name: "IX_tb_Paises_usuarioActualizacionId",
                table: "Paises",
                newName: "IX_Paises_UsuarioActualizacionId");

            migrationBuilder.RenameColumn(
                name: "usuarioEliminacionId",
                table: "GaleriaElementosGeologicos",
                newName: "UsuarioEliminacionId");

            migrationBuilder.RenameColumn(
                name: "usuarioCreacionId",
                table: "GaleriaElementosGeologicos",
                newName: "UsuarioCreacionId");

            migrationBuilder.RenameColumn(
                name: "usuarioActualizacionId",
                table: "GaleriaElementosGeologicos",
                newName: "UsuarioActualizacionId");

            migrationBuilder.RenameColumn(
                name: "fechaEliminacion",
                table: "GaleriaElementosGeologicos",
                newName: "FechaEliminacion");

            migrationBuilder.RenameColumn(
                name: "fechaCreacion",
                table: "GaleriaElementosGeologicos",
                newName: "FechaCreacion");

            migrationBuilder.RenameColumn(
                name: "fechaActualizacion",
                table: "GaleriaElementosGeologicos",
                newName: "FechaActualizacion");

            migrationBuilder.RenameColumn(
                name: "estadoActivo",
                table: "GaleriaElementosGeologicos",
                newName: "EstadoActivo");

            migrationBuilder.RenameColumn(
                name: "elementoGeologicoId",
                table: "GaleriaElementosGeologicos",
                newName: "ElementoGeologicoId");

            migrationBuilder.RenameColumn(
                name: "detalleGrupo",
                table: "GaleriaElementosGeologicos",
                newName: "DetalleGrupo");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "GaleriaElementosGeologicos",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_tb_GaleriaElementosGeologicos_usuarioEliminacionId",
                table: "GaleriaElementosGeologicos",
                newName: "IX_GaleriaElementosGeologicos_UsuarioEliminacionId");

            migrationBuilder.RenameIndex(
                name: "IX_tb_GaleriaElementosGeologicos_usuarioCreacionId",
                table: "GaleriaElementosGeologicos",
                newName: "IX_GaleriaElementosGeologicos_UsuarioCreacionId");

            migrationBuilder.RenameIndex(
                name: "IX_tb_GaleriaElementosGeologicos_usuarioActualizacionId",
                table: "GaleriaElementosGeologicos",
                newName: "IX_GaleriaElementosGeologicos_UsuarioActualizacionId");

            migrationBuilder.RenameIndex(
                name: "IX_tb_GaleriaElementosGeologicos_elementoGeologicoId",
                table: "GaleriaElementosGeologicos",
                newName: "IX_GaleriaElementosGeologicos_ElementoGeologicoId");

            migrationBuilder.RenameColumn(
                name: "usuarioEliminacionId",
                table: "FotosElementos",
                newName: "UsuarioEliminacionId");

            migrationBuilder.RenameColumn(
                name: "usuarioCreacionId",
                table: "FotosElementos",
                newName: "UsuarioCreacionId");

            migrationBuilder.RenameColumn(
                name: "usuarioActualizacionId",
                table: "FotosElementos",
                newName: "UsuarioActualizacionId");

            migrationBuilder.RenameColumn(
                name: "tipoFoto",
                table: "FotosElementos",
                newName: "TipoFoto");

            migrationBuilder.RenameColumn(
                name: "imagen",
                table: "FotosElementos",
                newName: "Imagen");

            migrationBuilder.RenameColumn(
                name: "galeriaElementosGeologicoId",
                table: "FotosElementos",
                newName: "GaleriaElementosGeologicoId");

            migrationBuilder.RenameColumn(
                name: "fechaEliminacion",
                table: "FotosElementos",
                newName: "FechaEliminacion");

            migrationBuilder.RenameColumn(
                name: "fechaCreacion",
                table: "FotosElementos",
                newName: "FechaCreacion");

            migrationBuilder.RenameColumn(
                name: "fechaActualizacion",
                table: "FotosElementos",
                newName: "FechaActualizacion");

            migrationBuilder.RenameColumn(
                name: "estadoActivo",
                table: "FotosElementos",
                newName: "EstadoActivo");

            migrationBuilder.RenameColumn(
                name: "descripcionEspecifica",
                table: "FotosElementos",
                newName: "DescripcionEspecifica");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "FotosElementos",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_tb_FotosElementos_usuarioEliminacionId",
                table: "FotosElementos",
                newName: "IX_FotosElementos_UsuarioEliminacionId");

            migrationBuilder.RenameIndex(
                name: "IX_tb_FotosElementos_usuarioCreacionId",
                table: "FotosElementos",
                newName: "IX_FotosElementos_UsuarioCreacionId");

            migrationBuilder.RenameIndex(
                name: "IX_tb_FotosElementos_usuarioActualizacionId",
                table: "FotosElementos",
                newName: "IX_FotosElementos_UsuarioActualizacionId");

            migrationBuilder.RenameIndex(
                name: "IX_tb_FotosElementos_galeriaElementosGeologicoId",
                table: "FotosElementos",
                newName: "IX_FotosElementos_GaleriaElementosGeologicoId");

            migrationBuilder.RenameColumn(
                name: "usuarioEliminacionId",
                table: "ElementosGeologicos",
                newName: "UsuarioEliminacionId");

            migrationBuilder.RenameColumn(
                name: "usuarioCreacionId",
                table: "ElementosGeologicos",
                newName: "UsuarioCreacionId");

            migrationBuilder.RenameColumn(
                name: "usuarioActualizacionId",
                table: "ElementosGeologicos",
                newName: "UsuarioActualizacionId");

            migrationBuilder.RenameColumn(
                name: "ubicacionId",
                table: "ElementosGeologicos",
                newName: "UbicacionId");

            migrationBuilder.RenameColumn(
                name: "tipoRoca",
                table: "ElementosGeologicos",
                newName: "TipoRoca");

            migrationBuilder.RenameColumn(
                name: "tipoMineral",
                table: "ElementosGeologicos",
                newName: "TipoMineral");

            migrationBuilder.RenameColumn(
                name: "tipoFosil",
                table: "ElementosGeologicos",
                newName: "TipoFosil");

            migrationBuilder.RenameColumn(
                name: "tipoElemento",
                table: "ElementosGeologicos",
                newName: "TipoElemento");

            migrationBuilder.RenameColumn(
                name: "periodo",
                table: "ElementosGeologicos",
                newName: "Periodo");

            migrationBuilder.RenameColumn(
                name: "nombre",
                table: "ElementosGeologicos",
                newName: "Nombre");

            migrationBuilder.RenameColumn(
                name: "litologia",
                table: "ElementosGeologicos",
                newName: "Litologia");

            migrationBuilder.RenameColumn(
                name: "laminaExiste",
                table: "ElementosGeologicos",
                newName: "LaminaExiste");

            migrationBuilder.RenameColumn(
                name: "fechaIngreso",
                table: "ElementosGeologicos",
                newName: "FechaIngreso");

            migrationBuilder.RenameColumn(
                name: "fechaEliminacion",
                table: "ElementosGeologicos",
                newName: "FechaEliminacion");

            migrationBuilder.RenameColumn(
                name: "fechaCreacion",
                table: "ElementosGeologicos",
                newName: "FechaCreacion");

            migrationBuilder.RenameColumn(
                name: "fechaActualizacion",
                table: "ElementosGeologicos",
                newName: "FechaActualizacion");

            migrationBuilder.RenameColumn(
                name: "estadoActivo",
                table: "ElementosGeologicos",
                newName: "EstadoActivo");

            migrationBuilder.RenameColumn(
                name: "especie",
                table: "ElementosGeologicos",
                newName: "Especie");

            migrationBuilder.RenameColumn(
                name: "ejemplares",
                table: "ElementosGeologicos",
                newName: "Ejemplares");

            migrationBuilder.RenameColumn(
                name: "edad",
                table: "ElementosGeologicos",
                newName: "Edad");

            migrationBuilder.RenameColumn(
                name: "donante",
                table: "ElementosGeologicos",
                newName: "Donante");

            migrationBuilder.RenameColumn(
                name: "documentosRelacionados",
                table: "ElementosGeologicos",
                newName: "DocumentosRelacionados");

            migrationBuilder.RenameColumn(
                name: "codigo",
                table: "ElementosGeologicos",
                newName: "Codigo");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "ElementosGeologicos",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "litologia_roca",
                table: "ElementosGeologicos",
                newName: "Roca_Litologia");

            migrationBuilder.RenameIndex(
                name: "IX_tb_ElementosGeologicos_usuarioEliminacionId",
                table: "ElementosGeologicos",
                newName: "IX_ElementosGeologicos_UsuarioEliminacionId");

            migrationBuilder.RenameIndex(
                name: "IX_tb_ElementosGeologicos_usuarioCreacionId",
                table: "ElementosGeologicos",
                newName: "IX_ElementosGeologicos_UsuarioCreacionId");

            migrationBuilder.RenameIndex(
                name: "IX_tb_ElementosGeologicos_usuarioActualizacionId",
                table: "ElementosGeologicos",
                newName: "IX_ElementosGeologicos_UsuarioActualizacionId");

            migrationBuilder.RenameIndex(
                name: "IX_tb_ElementosGeologicos_ubicacionId",
                table: "ElementosGeologicos",
                newName: "IX_ElementosGeologicos_UbicacionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Ubicaciones",
                table: "Ubicaciones",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Provincias",
                table: "Provincias",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Paises",
                table: "Paises",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GaleriaElementosGeologicos",
                table: "GaleriaElementosGeologicos",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FotosElementos",
                table: "FotosElementos",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ElementosGeologicos",
                table: "ElementosGeologicos",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ElementosGeologicos_Ubicaciones_UbicacionId",
                table: "ElementosGeologicos",
                column: "UbicacionId",
                principalTable: "Ubicaciones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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
                name: "FK_FotosElementos_GaleriaElementosGeologicos_GaleriaElementosGeologicoId",
                table: "FotosElementos",
                column: "GaleriaElementosGeologicoId",
                principalTable: "GaleriaElementosGeologicos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
                name: "FK_Provincias_Paises_PaisId",
                table: "Provincias",
                column: "PaisId",
                principalTable: "Paises",
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
                name: "FK_Ubicaciones_Paises_PaisId",
                table: "Ubicaciones",
                column: "PaisId",
                principalTable: "Paises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Ubicaciones_Provincias_ProvinciaId",
                table: "Ubicaciones",
                column: "ProvinciaId",
                principalTable: "Provincias",
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
    }
}
