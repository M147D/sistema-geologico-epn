/**
 * @file Definiciones de tipos para autenticación y usuarios
 * Espeja los DTOs del backend: Servidor_Sistema_Geologia/DTO/Auth/
 */

/**
 * Datos para iniciar sesión
 * Backend: LoginDto.cs
 *
 * @typedef {Object} LoginDto
 * @property {string} email
 * @property {string} password
 */

/**
 * Datos para registrar un nuevo usuario
 * Backend: RegisterDto.cs
 *
 * @typedef {Object} RegisterDto
 * @property {string} email
 * @property {string} password
 * @property {string} confirmPassword
 * @property {string} [nombreCompleto]
 * @property {number} rol - Enum RolUsuario (0=Free, 1=Premium, 2=Admin)
 */

/**
 * Respuesta de autenticación
 * Backend: AuthResponseDto.cs
 *
 * @typedef {Object} AuthResponseDto
 * @property {boolean} success
 * @property {string} message
 * @property {UserInfoDto|null} user
 * @property {string|null} token
 * @property {string|null} tokenExpiration - ISO 8601
 * @property {string[]} errors
 */

/**
 * Información del usuario autenticado
 * Backend: UserInfoDto.cs
 *
 * @typedef {Object} UserInfoDto
 * @property {number} id
 * @property {string} email
 * @property {string|null} userName
 * @property {string|null} nombreCompleto
 * @property {number} rol - Enum RolUsuario (0=Free, 1=Premium, 2=Admin)
 * @property {string} fechaCreacion - ISO 8601
 * @property {boolean} estadoActivo
 * @property {boolean} emailConfirmed
 */

/**
 * Datos para cambiar contraseña
 * Backend: ChangePasswordDto.cs
 *
 * @typedef {Object} ChangePasswordDto
 * @property {string} currentPassword
 * @property {string} newPassword
 * @property {string} confirmPassword
 */

export {};
