namespace Servidor_Sistema_Geologia.Repositories.Interfaces;

/// <summary>
/// Agrupa varias escrituras de repositorios en una sola transacción de base de datos.
/// Si la función lanza una excepción, se revierte todo (REV-01, H-08 y H-14).
/// </summary>
public interface IUnitOfWork
{
    Task<TResult> EnTransaccionAsync<TResult>(Func<Task<TResult>> trabajo);

    /// <summary>Revierte la transacción en curso sin lanzar (para errores de negocio detectados al final).</summary>
    void MarcarParaRevertir();
}
