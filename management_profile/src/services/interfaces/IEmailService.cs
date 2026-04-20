/// <summary>
/// Define el contrato del servicio encargado del envío de notificaciones
/// por correo electrónico dentro del sistema.
/// Permite desacoplar la lógica de envío de correos de la lógica de negocio
/// del microservicio de perfiles.
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Envía un correo electrónico de notificación cuando se crea un nuevo perfil.
    /// </summary>
    /// <param name="toEmail">
    /// Dirección de correo electrónico del destinatario.
    /// </param>
    /// <param name="name">
    /// Nombre del usuario al que pertenece el perfil creado.
    /// Se utiliza para personalizar el contenido del mensaje.
    /// </param>
    /// <returns>
    /// Una tarea asincrónica que representa la operación de envío del correo.
    /// </returns>
    Task SendProfileCreatedEmailAsync(string toEmail, string name);

    /// <summary>
    /// Envía un correo electrónico de notificación cuando se elimina un perfil.
    /// </summary>
    /// <param name="toEmail">
    /// Dirección de correo electrónico del destinatario.
    /// </param>
    /// <param name="name">
    /// Nombre del usuario al que pertenece el perfil eliminado.
    /// Se utiliza para personalizar el contenido del mensaje.
    /// </param>
    /// <returns>
    /// Una tarea asincrónica que representa la operación de envío del correo.
    /// </returns>
    Task SendProfileDeletedEmailAsync(string toEmail, string name);
}