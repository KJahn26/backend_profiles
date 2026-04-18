using Microsoft.EntityFrameworkCore;

/// <summary>
/// Representa el contexto de base de datos utilizado por el microservicio de perfiles.
/// Proporciona acceso a las entidades persistidas mediante Entity Framework Core
/// y permite la interacción con la tabla de perfiles.
/// </summary>
public class DataContext : DbContext
{
    /// <summary>
    /// Inicializa una nueva instancia del contexto de base de datos.
    /// </summary>
    /// <param name="options">
    /// Opciones de configuración del contexto, incluyendo la cadena de conexión
    /// y el proveedor de base de datos utilizado (SQL Server, PostgreSQL, etc.).
    /// </param>
    public DataContext(DbContextOptions<DataContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Representa la tabla de perfiles dentro de la base de datos.
    /// Permite realizar operaciones CRUD sobre la entidad <see cref="Profile"/>.
    /// </summary>
    public DbSet<Profile> profiles { get; set; } // profiles table
}