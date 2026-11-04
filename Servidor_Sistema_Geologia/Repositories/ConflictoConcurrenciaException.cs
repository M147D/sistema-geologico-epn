namespace Servidor_Sistema_Geologia;

/// <summary>
/// La fila cambió (o se borró) entre la lectura y la escritura. Los repositorios la
/// lanzan en lugar de DbUpdateConcurrencyException para que los servicios no dependan
/// de EF; los controladores responden 409 (PEN-12).
/// </summary>
public class ConflictoConcurrenciaException : Exception
{
    public const string Mensaje = "Conflicto: el registro fue modificado o eliminado por otra persona. Recargue e intente de nuevo.";
    public ConflictoConcurrenciaException(Exception inner) : base(Mensaje, inner) { }
}
