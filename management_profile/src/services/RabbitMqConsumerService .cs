using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

public class RabbitMqConsumerService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public RabbitMqConsumerService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost"
        };

        var connection = await factory.CreateConnectionAsync();
        var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(
            queue: "hello",
            durable: true,
            exclusive: false,
            autoDelete: false
        );

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (model, ea) =>
        {
            var json = Encoding.UTF8.GetString(ea.Body.ToArray());

            Console.WriteLine($"Mensaje recibido: {json}");

            // usamos scope porque estamos fuera del pipeline HTTP
            using var scope = _scopeFactory.CreateScope();

            var profileService =
                scope.ServiceProvider.GetRequiredService<IProfileService>();

            try
            {
                var employeeMessage = System.Text.Json.JsonSerializer.Deserialize<MessageRabbitMq>(json);
                if (employeeMessage != null)
                {
                    Profile profile = new Profile
                    {
                        Id = employeeMessage.Id.ToString(),
                        Name = employeeMessage.NameUser,
                        Email = employeeMessage.Email
                        
                    };
                    await profileService.AddProfileAsync(profile);
                    return ;
                }
            }
            catch (System.Text.Json.JsonException ex)
            {
                Console.WriteLine($"Error al deserializar el mensaje: {ex.Message}");
            }

            try
            {
                var deleteMessage = System.Text.Json.JsonSerializer.Deserialize<MessageRabbitDeleteEmployee>(json);

                if (deleteMessage != null)
                {
                    await profileService.DeleteProfileAsync(deleteMessage.Id.ToString());
                    return;
                }
            }
            catch (System.Text.Json.JsonException ex)
            {
                Console.WriteLine($"Error al deserializar el mensaje: {ex.Message}");
            }
        };

        await channel.BasicConsumeAsync(
            queue: "hello",
            autoAck: true,
            consumer: consumer
        );

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}