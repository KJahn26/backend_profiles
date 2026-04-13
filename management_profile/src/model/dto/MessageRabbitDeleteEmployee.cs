using System.Text.Json.Serialization;

public record MessageRabbitDeleteEmployee(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("nameUser")] string NameUser,
    [property: JsonPropertyName("email")] string Email
);
