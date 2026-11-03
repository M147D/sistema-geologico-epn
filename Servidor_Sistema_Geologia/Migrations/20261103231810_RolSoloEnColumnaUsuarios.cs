using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Servidor_Sistema_Geologia.Migrations
{
    /// <inheritdoc />
    public partial class RolSoloEnColumnaUsuarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // El rol vive solo en "Usuarios"."Rol" (lo que va en el JWT). Se vacían las
            // tablas de roles de Identity para que no quede una segunda copia (REV-01, H-07).
            migrationBuilder.Sql(@"DELETE FROM ""UsuarioRoles""; DELETE FROM ""RolClaims""; DELETE FROM ""Roles"";");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Sin inverso: las filas borradas eran una copia no usada del rol.
        }
    }
}
