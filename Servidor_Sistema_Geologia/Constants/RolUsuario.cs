namespace Servidor_Sistema_Geologia;

public enum RolUsuario
{
	Free,       // 0
	Premium,    // 1
	Admin,      // 2 - NO CAMBIAR, usuarios existentes en BD tienen este valor
	Invitado    // 3
}
