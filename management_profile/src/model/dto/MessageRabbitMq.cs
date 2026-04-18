using System.Text.Json.Serialization;

/// <summary>
/// Representa el mensaje recibido desde RabbitMQ cuando se crea un nuevo empleado.
/// Este DTO permite transportar la información necesaria para generar automáticamente
/// el perfil correspondiente dentro del microservicio de perfiles.
/// </summary>
/// <param name="Id">
/// Identificador único del empleado generado en el microservicio de empleados.
/// </param>
/// <param name="NameUser">
/// Nombre del empleado registrado en el sistema.
/// </param>
/// <param name="Email">
/// Dirección de correo electrónico del empleado.
/// Se utiliza para la creación del perfil y envío de notificaciones.
/// </param>
/// <param name="DepartmentID">
/// Identificador del departamento al que pertenece el empleado.
/// Permite establecer relaciones organizacionales.
/// </param>
/// <param name="DateEnter">
/// Fecha de ingreso del empleado a la organización.
/// </param>
public record MessageRabbitMq(
    [property: JsonPropertyName("id")] long Id,

    [property: JsonPropertyName("nameUser")] string NameUser,

    [property: JsonPropertyName("email")] string Email,

    [property: JsonPropertyName("departmentID")] long DepartmentID,

    [property: JsonPropertyName("dateEnter")] DateTime DateEnter
);