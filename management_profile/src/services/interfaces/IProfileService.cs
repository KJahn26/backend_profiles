/// <summary>
/// Define el contrato del servicio encargado de gestionar la lógica de negocio
/// relacionada con los perfiles de empleados dentro del sistema.
/// Incluye operaciones CRUD y procesamiento de eventos provenientes de RabbitMQ.
/// </summary>
public interface IProfileService
{
    /// <summary>
    /// Obtiene la lista completa de perfiles registrados en el sistema.
    /// </summary>
    /// <returns>
    /// Una lista asincrónica de perfiles almacenados en la base de datos.
    /// </returns>
    Task<List<Profile>> GetProfilesAsync();

    /// <summary>
    /// Obtiene un perfil específico a partir de su identificador único.
    /// </summary>
    /// <param name="id">
    /// Identificador único del perfil.
    /// </param>
    /// <returns>
    /// El perfil encontrado o null si no existe en la base de datos.
    /// </returns>
    Task<Profile?> GetProfileByIdAsync(string id);

    /// <summary>
    /// Crea un nuevo perfil en el sistema.
    /// </summary>
    /// <param name="profile">
    /// Objeto que contiene la información del perfil a registrar.
    /// </param>
    /// <returns>
    /// El perfil creado después de ser almacenado en la base de datos.
    /// </returns>
    Task<Profile> AddProfileAsync(Profile profile);

    /// <summary>
    /// Actualiza la información de un perfil existente.
    /// </summary>
    /// <param name="profile">
    /// Objeto con la información actualizada del perfil.
    /// </param>
    /// <returns>
    /// El perfil actualizado o null si el perfil no existe.
    /// </returns>
    Task<Profile?> UpdateProfileAsync(Profile profile);

    /// <summary>
    /// Elimina un perfil del sistema a partir de su identificador único.
    /// </summary>
    /// <param name="id">
    /// Identificador único del perfil a eliminar.
    /// </param>
    /// <returns>
    /// true si el perfil fue eliminado correctamente; false en caso contrario.
    /// </returns>
    Task<bool> DeleteProfileAsync(string id);

    /// <summary>
    /// Procesa un mensaje recibido desde RabbitMQ para ejecutar acciones
    /// relacionadas con la creación automática de perfiles.
    /// </summary>
    /// <param name="message">
    /// Contenido del mensaje recibido desde el sistema de mensajería.
    /// </param>
    /// <returns>
    /// Una tarea asincrónica que representa la operación de procesamiento del mensaje.
    /// </returns>
    Task ProcessMessage(string message);
}