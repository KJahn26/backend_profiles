using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Services.AddDbContext<DataContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IProfileService, ProfileService>();

// // agregamos el rabbitMQ
builder.Services.AddHostedService<RabbitMqConsumerService>();


var app = builder.Build();

// Aplica migraciones automáticamente al arrancar
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DataContext>();
    db.Database.Migrate();
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

    app.MapOpenApi();

}


// var factory = new ConnectionFactory { HostName = "localhost" };
// using var connection = await factory.CreateConnectionAsync();
// using var channel = await connection.CreateChannelAsync();

// await channel.QueueDeclareAsync(queue: "hello", durable: true, exclusive: false, autoDelete: false,
//     arguments: new Dictionary<string, object?> { { "x-queue-type", "quorum" } });

// Console.WriteLine(" [*] Waiting for messages.");

// var consumer = new AsyncEventingBasicConsumer(channel);
// consumer.ReceivedAsync += (model, ea) =>
// {
//     var body = ea.Body.ToArray();
//     var message = Encoding.UTF8.GetString(body);
//     Console.WriteLine($" [x] Received {message}");
//     return Task.CompletedTask;
// };

// await channel.BasicConsumeAsync("hello", autoAck: true, consumer: consumer);

// Console.WriteLine(" Press [enter] to exit.");
// Console.ReadLine();

app.UseHttpsRedirection();

app.MapControllers();
app.Run();
