using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Servidor_Sistema_Geologia.Migrations
{
    /// <inheritdoc />
    public partial class RestaurarRelacionUbicacionPais : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ProvinciaId",
                table: "Ubicaciones",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaisId",
                table: "Ubicaciones",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Ubicaciones_PaisId",
                table: "Ubicaciones",
                column: "PaisId");

            // Actualizar PaisId basándose en Provincia.PaisId si existe, o usar el primer país disponible
            migrationBuilder.Sql(@"
                -- Actualizar PaisId desde Provincia para ubicaciones que tienen provincia
                UPDATE u
                SET u.PaisId = p.PaisId
                FROM Ubicaciones u
                INNER JOIN Provincias p ON u.ProvinciaId = p.Id
                WHERE u.ProvinciaId IS NOT NULL AND u.ProvinciaId > 0;

                -- Para ubicaciones sin provincia válida, usar el primer país disponible
                DECLARE @primerPaisId INT;
                SELECT TOP 1 @primerPaisId = Id FROM Paises WHERE EstadoActivo = 1 ORDER BY Id;

                IF @primerPaisId IS NOT NULL
                BEGIN
                    UPDATE Ubicaciones
                    SET PaisId = @primerPaisId
                    WHERE PaisId = 0 OR PaisId IS NULL;

                    -- También actualizar ProvinciaId si es 0 a la primera provincia de ese país
                    DECLARE @primerProvinciaId INT;
                    SELECT TOP 1 @primerProvinciaId = Id FROM Provincias WHERE PaisId = @primerPaisId AND EstadoActivo = 1 ORDER BY Id;

                    IF @primerProvinciaId IS NOT NULL
                    BEGIN
                        UPDATE Ubicaciones
                        SET ProvinciaId = @primerProvinciaId
                        WHERE ProvinciaId = 0 OR ProvinciaId IS NULL;
                    END
                END
            ");

            migrationBuilder.AddForeignKey(
                name: "FK_Ubicaciones_Paises_PaisId",
                table: "Ubicaciones",
                column: "PaisId",
                principalTable: "Paises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ubicaciones_Paises_PaisId",
                table: "Ubicaciones");

            migrationBuilder.DropIndex(
                name: "IX_Ubicaciones_PaisId",
                table: "Ubicaciones");

            migrationBuilder.DropColumn(
                name: "PaisId",
                table: "Ubicaciones");

            migrationBuilder.AlterColumn<int>(
                name: "ProvinciaId",
                table: "Ubicaciones",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
