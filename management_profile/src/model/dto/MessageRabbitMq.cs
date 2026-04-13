using System.Text.Json.Serialization;

public record MessageRabbitMq(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("nameUser")] string NameUser,
    [property: JsonPropertyName("email")] string Email,
    [property: JsonPropertyName("departmentID")] long DepartmentID,
    [property: JsonPropertyName("dateEnter")] DateTime DateEnter
);