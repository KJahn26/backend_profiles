using System.Net;
using System.Net.Mail;

/// <summary>
/// Servicio encargado del envío de correos electrónicos utilizando SMTP.
/// Implementa el contrato definido en <see cref="IEmailService"/> para enviar
/// notificaciones cuando se crea un nuevo perfil en el sistema.
/// </summary>
public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Inicializa una nueva instancia del servicio de envío de correos electrónicos.
    /// </summary>
    /// <param name="configuration">
    /// Proporciona acceso a la configuración de la aplicación, incluyendo
    /// las credenciales SMTP almacenadas en appsettings.json.
    /// </param>
    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Envía un correo electrónico de notificación cuando se crea un perfil.
    /// Utiliza el servidor SMTP de Gmail configurado en la aplicación.
    /// </summary>
    /// <param name="toEmail">
    /// Dirección de correo electrónico del destinatario.
    /// </param>
    /// <param name="name">
    /// Nombre del usuario al que pertenece el perfil creado.
    /// Se utiliza para personalizar el mensaje enviado.
    /// </param>
    /// <returns>
    /// Una tarea asincrónica que representa la operación de envío del correo.
    /// </returns>
    public async Task SendProfileCreatedEmailAsync(string toEmail, string name)
    {
        try
        {
            var email = _configuration["EmailSettings:Email"];
            var password = _configuration["EmailSettings:Password"];

            Console.WriteLine($"Preparando envío a {toEmail}");

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                /// <summary>
                /// Puerto estándar TLS para SMTP en Gmail.
                /// </summary>
                Port = 587,

                /// <summary>
                /// Credenciales utilizadas para autenticación SMTP.
                /// </summary>
                Credentials = new NetworkCredential(email, password),

                /// <summary>
                /// Habilita conexión segura mediante SSL/TLS.
                /// </summary>
                EnableSsl = true
            };

            var message = new MailMessage
            {
                /// <summary>
                /// Dirección remitente del correo.
                /// </summary>
                From = new MailAddress(email!),

                /// <summary>
                /// Asunto del correo electrónico.
                /// </summary>
                Subject = "Perfil creado correctamente",

                /// <summary>
                /// Contenido del mensaje enviado al usuario.
                /// </summary>
                Body = $"Hola {name}, tu perfil ha sido creado exitosamente.",

                /// <summary>
                /// Indica si el cuerpo del mensaje está en formato HTML.
                /// </summary>
                IsBodyHtml = false
            };

            message.To.Add(toEmail);

            await smtpClient.SendMailAsync(message);

            Console.WriteLine($"Correo enviado correctamente a {toEmail}");
        }
        catch (Exception ex)
        {
            /// <summary>
            /// Manejo básico de errores durante el envío del correo.
            /// </summary>
            Console.WriteLine($"Error enviando correo: {ex.Message}");
        }
    }

    public async Task SendProfileDeletedEmailAsync(string toEmail, string name)
    {
        try
        {
            var email = _configuration["EmailSettings:Email"];
            var password = _configuration["EmailSettings:Password"];

            Console.WriteLine($"Preparando envío a {toEmail}");

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                /// <summary>
                /// Puerto estándar TLS para SMTP en Gmail.
                /// </summary>
                Port = 587,

                /// <summary>
                /// Credenciales utilizadas para autenticación SMTP.
                /// </summary>
                Credentials = new NetworkCredential(email, password),

                /// <summary>
                /// Habilita conexión segura mediante SSL/TLS.
                /// </summary>
                EnableSsl = true
            };

            var message = new MailMessage
            {
                /// <summary>
                /// Dirección remitente del correo.
                /// </summary>
                From = new MailAddress(email!),

                /// <summary>
                /// Asunto del correo electrónico.
                /// </summary>
                Subject = "Perfil eliminado correctamente",

                /// <summary>
                /// Contenido del mensaje enviado al usuario.
                /// </summary>
                Body = $"Hola {name}, para notificarte que tu perfil ha sido eliminado.",

                /// <summary>
                /// Indica si el cuerpo del mensaje está en formato HTML.
                /// </summary>
                IsBodyHtml = false
            };

            message.To.Add(toEmail);

            await smtpClient.SendMailAsync(message);

            Console.WriteLine($"Correo enviado correctamente a {toEmail}");
        }
        catch (Exception ex)
        {
            /// <summary>
            /// Manejo básico de errores durante el envío del correo.
            /// </summary>
            Console.WriteLine($"Error enviando correo: {ex.Message}");
        }
    }
}