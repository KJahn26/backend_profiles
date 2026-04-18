using System.Text.Json.Serialization;

/// <summary>
/// Representa el mensaje recibido desde RabbitMQ cuando un empleado es eliminado
/// en el microservicio de empleados.
/// Este evento permite sincronizar la eliminación del perfil asociado dentro
/// del microservicio de perfiles.
/// </summary>
/// <param name="Id">
/// Identificador único del empleado eliminado.
/// Se utiliza para localizar y eliminar el perfil asociado.
/// </param>
/// <param name="NameUser">
/// Nombre del empleado eliminado.
/// Se incluye como información adicional del evento.
/// </param>
/// <param name="Email">
/// Dirección de correo electrónico del empleado eliminado.
/// Permite validar o registrar la eliminación del perfil relacionado.
/// </param>
public record MessageRabbitDeleteEmployee(
    [property: JsonPropertyName("id")] long Id,

    [property: JsonPropertyName("nameUser")] string NameUser,

    [property: JsonPropertyName("email")] string Email
);