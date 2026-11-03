using Servidor_Sistema_Geologia.Repositories.Interfaces;

namespace Servidor_Sistema_Geologia.Repositories.Implementation;

/// <summary>
/// Transacción explícita sobre el DbContext del request. Todos los repositorios del
/// request comparten ese DbContext (Scoped), así que sus SaveChanges quedan dentro.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly SistemaGeologicoDbContext _context;
    private bool _revertir;

    public UnitOfWork(SistemaGeologicoDbContext context) => _context = context;

    public void MarcarParaRevertir() => _revertir = true;

    public async Task<TResult> EnTransaccionAsync<TResult>(Func<Task<TResult>> trabajo)
    {
        // Transacción ya abierta (llamada anidada): se suma a la exterior.
        if (_context.Database.CurrentTransaction != null)
            return await trabajo();

        _revertir = false;
        await using var transaccion = await _context.Database.BeginTransactionAsync();
        try
        {
            var resultado = await trabajo();
            if (_revertir)
            {
                await transaccion.RollbackAsync();
                _context.ChangeTracker.Clear();
            }
            else
            {
                await transaccion.CommitAsync();
            }
            return resultado;
        }
        catch
        {
            await transaccion.RollbackAsync();
            _context.ChangeTracker.Clear();
            throw;
        }
    }
}
