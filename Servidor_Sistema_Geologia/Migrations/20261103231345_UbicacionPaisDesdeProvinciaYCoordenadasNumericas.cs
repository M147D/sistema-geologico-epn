using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Servidor_Sistema_Geologia.Migrations
{
    /// <inheritdoc />
    public partial class UbicacionPaisDesdeProvinciaYCoordenadasNumericas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tb_Ubicaciones_tb_Paises_paisId",
                table: "tb_Ubicaciones");

            migrationBuilder.DropIndex(
                name: "IX_tb_Ubicaciones_paisId",
                table: "tb_Ubicaciones");

            migrationBuilder.DropColumn(
                name: "paisId",
                table: "tb_Ubicaciones");

            // Npgsql no genera USING para varchar -> double precision: SQL explícito.
            // Datos existentes: vacío o no numérico -> NULL; el par 0.0/0.0 (valor por
            // defecto anterior para "desconocido") -> NULL; fuera de rango -> NULL.
            migrationBuilder.Sql(@"
                ALTER TABLE ""tb_Ubicaciones"" ALTER COLUMN latitud DROP NOT NULL;
                ALTER TABLE ""tb_Ubicaciones"" ALTER COLUMN longitud DROP NOT NULL;
                UPDATE ""tb_Ubicaciones"" SET latitud = NULL, longitud = NULL
                WHERE latitud ~ '^\s*-?0+(\.0+)?\s*$' AND longitud ~ '^\s*-?0+(\.0+)?\s*$';
                UPDATE ""tb_Ubicaciones"" SET latitud = NULL
                WHERE latitud IS NOT NULL AND latitud !~ '^\s*-?[0-9]+(\.[0-9]+)?\s*$';
                UPDATE ""tb_Ubicaciones"" SET longitud = NULL
                WHERE longitud IS NOT NULL AND longitud !~ '^\s*-?[0-9]+(\.[0-9]+)?\s*$';
                ALTER TABLE ""tb_Ubicaciones"" ALTER COLUMN latitud TYPE double precision USING btrim(latitud)::double precision;
                ALTER TABLE ""tb_Ubicaciones"" ALTER COLUMN longitud TYPE double precision USING btrim(longitud)::double precision;
                UPDATE ""tb_Ubicaciones"" SET latitud = NULL WHERE latitud NOT BETWEEN -90 AND 90;
                UPDATE ""tb_Ubicaciones"" SET longitud = NULL WHERE longitud NOT BETWEEN -180 AND 180;
            ");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Ubicaciones_Latitud",
                table: "tb_Ubicaciones",
                sql: "latitud IS NULL OR (latitud >= -90 AND latitud <= 90)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Ubicaciones_Longitud",
                table: "tb_Ubicaciones",
                sql: "longitud IS NULL OR (longitud >= -180 AND longitud <= 180)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Ubicaciones_Latitud",
                table: "tb_Ubicaciones");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Ubicaciones_Longitud",
                table: "tb_Ubicaciones");

            // Inverso explícito: coordenadas a texto (NULL -> '0.0') y paisId rellenado
            // desde la provincia antes de volverlo NOT NULL.
            migrationBuilder.Sql(@"
                ALTER TABLE ""tb_Ubicaciones"" ALTER COLUMN latitud TYPE character varying(60) USING COALESCE(latitud::text, '0.0');
                ALTER TABLE ""tb_Ubicaciones"" ALTER COLUMN longitud TYPE character varying(60) USING COALESCE(longitud::text, '0.0');
                ALTER TABLE ""tb_Ubicaciones"" ALTER COLUMN latitud SET NOT NULL;
                ALTER TABLE ""tb_Ubicaciones"" ALTER COLUMN longitud SET NOT NULL;
                ALTER TABLE ""tb_Ubicaciones"" ADD COLUMN ""paisId"" integer;
                UPDATE ""tb_Ubicaciones"" u SET ""paisId"" = p.""paisId"" FROM ""tb_Provincias"" p WHERE p.id = u.""provinciaId"";
                ALTER TABLE ""tb_Ubicaciones"" ALTER COLUMN ""paisId"" SET NOT NULL;
            ");

            migrationBuilder.CreateIndex(
                name: "IX_tb_Ubicaciones_paisId",
                table: "tb_Ubicaciones",
                column: "paisId");

            migrationBuilder.AddForeignKey(
                name: "FK_tb_Ubicaciones_tb_Paises_paisId",
                table: "tb_Ubicaciones",
                column: "paisId",
                principalTable: "tb_Paises",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
