/// <summary>
/// Representa la información del perfil de un empleado dentro del sistema.
/// Contiene los datos personales y de contacto asociados al usuario.
/// </summary>
public class Profile
{
    /// <summary>
    /// Identificador único del perfil.
    /// Corresponde al identificador principal del registro en la base de datos.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Identificador del empleado asociado al perfil.
    /// Permite relacionar el perfil con el servicio de empleados.
    /// </summary>
    public string? EmpleadoId { get; set; }

    /// <summary>
    /// Nombre completo del empleado.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Dirección de correo electrónico del empleado.
    /// Se utiliza para notificaciones del sistema.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Número de teléfono del empleado.
    /// </summary>
    public string? Telefono { get; set; }

    /// <summary>
    /// Dirección de residencia del empleado.
    /// </summary>
    public string? Direccion { get; set; }

    /// <summary>
    /// Ciudad de residencia del empleado.
    /// </summary>
    public string? Ciudad { get; set; }

    /// <summary>
    /// Biografía o descripción personal del empleado.
    /// Permite almacenar información adicional relevante.
    /// </summary>
    public string? Biografia { get; set; }

    /// <summary>
    /// Fecha en la que fue creado el perfil dentro del sistema.
    /// Se asigna automáticamente al momento del registro.
    /// </summary>
    public DateTime FechaCreacion { get; set; }
}