using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Servicio en segundo plano encargado de consumir mensajes desde RabbitMQ.
/// Permite procesar eventos relacionados con la creación y eliminación de empleados
/// provenientes de otros microservicios, sincronizando la información con el
/// microservicio de perfiles.
/// </summary>
public class RabbitMqConsumerService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    /// <summary>
    /// Inicializa una nueva instancia del consumidor de RabbitMQ.
    /// </summary>
    /// <param name="scopeFactory">
    /// Fábrica de scopes utilizada para resolver servicios con ciclo de vida Scoped
    /// dentro de un BackgroundService fuera del pipeline HTTP.
    /// </param>
    public RabbitMqConsumerService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    /// <summary>
    /// Método principal ejecutado automáticamente al iniciar el servicio.
    /// Se encarga de establecer la conexión con RabbitMQ, declarar la cola
    /// y procesar los mensajes recibidos de manera asincrónica.
    /// </summary>
    /// <param name="stoppingToken">
    /// Token utilizado para cancelar la ejecución del servicio cuando la aplicación se detiene.
    /// </param>
    /// <returns>
    /// Tarea asincrónica que representa la ejecución continua del consumidor.
    /// </returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            /// <summary>
            /// Dirección del servidor RabbitMQ.
            /// </summary>
            HostName = "localhost"
        };

        var connection = await factory.CreateConnectionAsync();

        /// <summary>
        /// Canal de comunicación con RabbitMQ.
        /// </summary>
        var channel = await connection.CreateChannelAsync();

        /// <summary>
        /// Declaración de la cola desde la cual se consumen los mensajes.
        /// </summary>
        await channel.QueueDeclareAsync(
            queue: "hello",
            durable: true,
            exclusive: false,
            autoDelete: false
        );

        /// <summary>
        /// Consumidor asincrónico de mensajes.
        /// </summary>
        var consumer = new AsyncEventingBasicConsumer(channel);

        /// <summary>
        /// Evento ejecutado automáticamente cada vez que llega un mensaje desde RabbitMQ.
        /// </summary>
        consumer.ReceivedAsync += async (model, ea) =>
        {
            /// <summary>
            /// Obtiene la routing key asociada al mensaje recibido.
            /// Permite identificar el tipo de evento publicado por el microservicio emisor
            /// (por ejemplo: creación o eliminación de empleados).
            /// </summary>
            var routingkey = ea.RoutingKey;

            var json = Encoding.UTF8.GetString(ea.Body.ToArray());
            /// <summary>
            /// Convierte el mensaje JSON recibido en una estructura manipulable
            /// para extraer los datos necesarios del empleado.
            /// </summary>
            var jsonDocument = System.Text.Json.JsonDocument.Parse(json);

            /// <summary>
            /// Se crea un scope manual porque el BackgroundService está fuera del pipeline HTTP.
            /// </summary>
            using var scope = _scopeFactory.CreateScope();

            var profileService =
                scope.ServiceProvider.GetRequiredService<IProfileService>();

            var emailService =
                scope.ServiceProvider.GetRequiredService<IEmailService>();

            /// <summary>
            /// Procesa el evento de creación de empleado.
            /// Cuando se recibe este evento desde RabbitMQ, se crea automáticamente
            /// un perfil asociado en el microservicio de perfiles si aún no existe.
            /// </summary>
            if (routingkey == "employee.save")
            {
                Console.WriteLine("Mensaje recibido: Creación de empleado");

                try
                {
                    var employeeMessage = jsonDocument;

                    if (employeeMessage != null)
                    {
                        /// <summary>
                        /// Verifica si el perfil ya existe antes de crearlo.
                        /// </summary>
                        var exists = await profileService.GetProfileByIdAsync(
                            employeeMessage.RootElement.GetProperty("Id").GetString()!
                        );

                        var perfil_creado = exists != null;

                        if (perfil_creado)
                        {
                            Console.WriteLine("Perfil ya existe, ignorando mensaje duplicado");

                            await channel.BasicAckAsync(ea.DeliveryTag, false);

                            return;
                        }

                        /// <summary>
                        /// Creación automática del perfil basado en el evento recibido.
                        /// </summary>
                        Profile profile = new Profile
                        {
                            Id = employeeMessage.RootElement.GetProperty("Id").ToString(),
                            Name = employeeMessage.RootElement.GetProperty("NameUser").GetString(),
                            Email = employeeMessage.RootElement.GetProperty("Email").GetString()
                        };

                        await profileService.AddProfileAsync(profile);

                        /// <summary>
                        /// Envío de notificación por correo electrónico tras la creación del perfil.
                        /// </summary>
                        if (profile.Email != null && profile.Name != null)
                        {
                            await emailService.SendProfileCreatedEmailAsync(
                                profile.Email,
                                profile.Name
                            );

                            /// <summary>
                            /// Confirma manualmente la correcta recepción y procesamiento del mensaje,
                            /// evitando que RabbitMQ lo reenvíe nuevamente.
                            /// </summary>
                            await channel.BasicAckAsync(ea.DeliveryTag, false);
                        }
                        else
                        {
                            Console.WriteLine(
                                "Email o nombre no disponibles, no se enviará notificación."
                            );

                            /// <summary>
                            /// Confirma manualmente la correcta recepción y procesamiento del mensaje,
                            /// evitando que RabbitMQ lo reenvíe nuevamente.
                            /// </summary>
                            await channel.BasicAckAsync(ea.DeliveryTag, false);
                        }

                        return;
                    }
                }
                catch (System.Text.Json.JsonException ex)
                {
                    Console.WriteLine($"Error al deserializar el mensaje: {ex.Message}");
                }

            }
            /// <summary>
            /// Procesa el evento de eliminación de empleado.
            /// Cuando se recibe este evento, se elimina el perfil correspondiente
            /// en el microservicio de perfiles.
            /// </summary>
            else if (routingkey == "employee.delete")
            {
                Console.WriteLine("Mensaje recibido: Eliminación de empleado");

                try
                {
                    /// <summary>
                    /// Procesamiento del evento de eliminación de empleado.
                    /// </summary>
                    var deleteMessage =
                        System.Text.Json.JsonSerializer.Deserialize<MessageRabbitDeleteEmployee>(json);

                    if (deleteMessage != null)
                    {
                        await profileService.DeleteProfileAsync(deleteMessage.Id.ToString());

                        /// <summary> Obtengo las variables para el envio del
                        /// correo electrónico tras la eliminación del perfil.
                        /// </summary>
                        var nombre_empleado = deleteMessage.NameUser;
                        var email_empleado = deleteMessage.Email;

                        /// <summary>
                        /// Envío de notificación por correo electrónico tras la eliminación del perfil.
                        /// </summary>
                        if (email_empleado != null && nombre_empleado != null)
                        {
                            await emailService.SendProfileDeletedEmailAsync(
                                email_empleado,
                                nombre_empleado
                            );

                            /// <summary>
                            /// Confirma manualmente la correcta recepción y procesamiento del mensaje,
                            /// evitando que RabbitMQ lo reenvíe nuevamente.
                            /// </summary>
                            await channel.BasicAckAsync(ea.DeliveryTag, false);
                        }
                        else
                        {
                            Console.WriteLine(
                                "Email o nombre no disponibles, no se enviará notificación."
                            );

                            /// <summary>
                            /// Confirma manualmente la correcta recepción y procesamiento del mensaje,
                            /// evitando que RabbitMQ lo reenvíe nuevamente.
                            /// </summary>
                            await channel.BasicAckAsync(ea.DeliveryTag, false);
                        }

                        return;
                    }
                }
                catch (System.Text.Json.JsonException ex)
                {
                    Console.WriteLine($"Error al deserializar el mensaje: {ex.Message}");
                }
            }
            /// <summary>
            /// Maneja mensajes cuyo routing key no coincide con los eventos esperados.
            /// Permite detectar eventos no soportados o errores de integración entre microservicios.
            /// </summary>
            else
            {
                Console.WriteLine($"Mensaje recibido con routing key desconocida: {routingkey}");
            }
        };

        /// <summary>
        /// Inicio del consumo continuo de mensajes desde la cola.
        /// </summary>
        await channel.BasicConsumeAsync(
            queue: "hello",
            autoAck: false,
            consumer: consumer
        );

        /// <summary>
        /// Mantiene el servicio activo indefinidamente hasta recibir señal de cancelación.
        /// </summary>
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}