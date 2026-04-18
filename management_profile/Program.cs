using Microsoft.EntityFrameworkCore;

/// <summary>
/// Punto de entrada principal del microservicio de perfiles.
/// Configura el contenedor de dependencias, la conexión a la base de datos,
/// el consumidor de RabbitMQ, el servicio de correo electrónico y la documentación OpenAPI.
/// </summary>
var builder = WebApplication.CreateBuilder(args);

/// <summary>
/// Agrega soporte para documentación automática de la API mediante OpenAPI.
/// Permite visualizar los endpoints disponibles en entorno de desarrollo.
/// </summary>
builder.Services.AddOpenApi();

/// <summary>
/// Agrega soporte para controladores REST.
/// Habilita el uso de endpoints definidos mediante atributos [ApiController].
/// </summary>
builder.Services.AddControllers();

/// <summary>
/// Configura el contexto de base de datos utilizando Entity Framework Core
/// con proveedor PostgreSQL.
/// La cadena de conexión se obtiene desde appsettings.json.
/// </summary>
builder.Services.AddDbContext<DataContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

/// <summary>
/// Registra el servicio de lógica de negocio de perfiles en el contenedor
/// de dependencias para permitir su inyección en controladores y servicios.
/// </summary>
builder.Services.AddScoped<IProfileService, ProfileService>();

/// <summary>
/// Registra el servicio en segundo plano encargado de consumir mensajes
/// desde RabbitMQ.
/// Permite sincronizar automáticamente la creación y eliminación de perfiles
/// desde otros microservicios.
/// </summary>
builder.Services.AddHostedService<RabbitMqConsumerService>();

/// <summary>
/// Registra el servicio de envío de correos electrónicos.
/// Se utiliza para notificar al usuario cuando su perfil ha sido creado.
/// </summary>
builder.Services.AddScoped<IEmailService, EmailService>();

/// <summary>
/// Registra el servicio de generación de documentación OpenAPI mediante Swagger.
/// </summary>
builder.Services.AddSwaggerGen();

var app = builder.Build();

/// <summary>
/// Aplica automáticamente las migraciones pendientes de Entity Framework Core
/// al iniciar la aplicación.
/// Garantiza que la base de datos esté sincronizada con el modelo actual.
/// </summary>
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DataContext>();

    db.Database.Migrate();
}

/// <summary>
/// Configura la documentación interactiva de la API únicamente en entorno
/// de desarrollo.
/// Habilita la generación de la especificación OpenAPI y la interfaz Swagger UI,
/// permitiendo inspeccionar y probar los endpoints directamente desde el navegador.
/// </summary>
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

/// <summary>
/// Habilita redirección automática hacia HTTPS.
/// Mejora la seguridad en las comunicaciones cliente-servidor.
/// </summary>
app.UseHttpsRedirection();

/// <summary>
/// Mapea los endpoints definidos en los controladores REST.
/// Activa el enrutamiento de la API.
/// </summary>
app.MapControllers();

/// <summary>
/// Inicia la ejecución del microservicio.
/// </summary>
app.Run();