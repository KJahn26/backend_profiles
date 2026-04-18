using Microsoft.EntityFrameworkCore;

/// <summary>
/// Servicio encargado de gestionar la lógica de negocio relacionada con los perfiles.
/// Implementa operaciones CRUD sobre la entidad <see cref="Profile"/> y permite
/// procesar mensajes provenientes de RabbitMQ para la creación automática de perfiles.
/// </summary>
public class ProfileService : IProfileService
{
    private readonly DataContext _context;

    /// <summary>
    /// Inicializa una nueva instancia del servicio de perfiles.
    /// </summary>
    /// <param name="context">
    /// Contexto de base de datos utilizado para acceder a la tabla de perfiles.
    /// </param>
    public ProfileService(DataContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Obtiene todos los perfiles registrados en el sistema.
    /// </summary>
    /// <returns>
    /// Lista de perfiles almacenados en la base de datos.
    /// </returns>
    public async Task<List<Profile>> GetProfilesAsync()
    {
        return await _context.profiles.ToListAsync();
    }

    /// <summary>
    /// Obtiene un perfil específico a partir de su identificador único.
    /// </summary>
    /// <param name="id">
    /// Identificador único del perfil.
    /// </param>
    /// <returns>
    /// El perfil encontrado o null si no existe.
    /// </returns>
    public async Task<Profile?> GetProfileByIdAsync(string id)
    {
        return await _context.profiles.FindAsync(id);
    }

    /// <summary>
    /// Crea un nuevo perfil en la base de datos.
    /// Asigna automáticamente la fecha de creación antes de persistir el registro.
    /// </summary>
    /// <param name="profile">
    /// Información del perfil a registrar.
    /// </param>
    /// <returns>
    /// Perfil creado después de guardarse en la base de datos.
    /// </returns>
    public async Task<Profile> AddProfileAsync(Profile profile)
    {
        profile.FechaCreacion = DateTime.UtcNow;

        _context.profiles.Add(profile);

        await _context.SaveChangesAsync();

        return profile;
    }

    /// <summary>
    /// Actualiza la información de un perfil existente.
    /// </summary>
    /// <param name="profile">
    /// Objeto con los nuevos valores del perfil.
    /// </param>
    /// <returns>
    /// Perfil actualizado o null si el perfil no existe.
    /// </returns>
    public async Task<Profile?> UpdateProfileAsync(Profile profile)
    {
        var existing = await _context.profiles.FindAsync(profile.Id);

        if (existing == null)
            return null;

        _context.Entry(existing).CurrentValues.SetValues(profile);

        await _context.SaveChangesAsync();

        return existing;
    }

    /// <summary>
    /// Elimina un perfil de la base de datos utilizando su identificador único.
    /// </summary>
    /// <param name="id">
    /// Identificador del perfil a eliminar.
    /// </param>
    /// <returns>
    /// true si el perfil fue eliminado correctamente; false si no existe.
    /// </returns>
    public async Task<bool> DeleteProfileAsync(string id)
    {
        var profile = await _context.profiles.FindAsync(id);

        if (profile == null)
            return false;

        _context.profiles.Remove(profile);

        await _context.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Procesa un mensaje recibido desde RabbitMQ para crear automáticamente
    /// un perfil en la base de datos basado en la información recibida.
    /// </summary>
    /// <param name="message">
    /// Contenido del mensaje recibido desde el sistema de mensajería.
    /// </param>
    /// <returns>
    /// Tarea asincrónica que representa la operación de procesamiento del mensaje.
    /// </returns>
    public async Task ProcessMessage(string message)
    {
        Console.WriteLine($"Procesando mensaje: {message}");

        var profile = new Profile
        {
            Name = message
        };

        _context.profiles.Add(profile);

        await _context.SaveChangesAsync();
    }
}