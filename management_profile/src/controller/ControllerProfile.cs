using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controlador encargado de la gestión de perfiles de empleados.
/// Permite consultar, crear y actualizar perfiles almacenados en el sistema.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProfilesController : ControllerBase
{
    private readonly IProfileService _profileService;

    /// <summary>
    /// Constructor del controlador de perfiles.
    /// </summary>
    /// <param name="profileService">
    /// Servicio encargado de la lógica de negocio relacionada con perfiles.
    /// </param>
    public ProfilesController(IProfileService profileService)
    {
        _profileService = profileService;
    }

    /// <summary>
    /// Obtiene la lista completa de perfiles registrados en el sistema.
    /// </summary>
    /// <returns>
    /// Lista de perfiles existentes en la base de datos.
    /// </returns>
    /// <response code="200">
    /// Retorna la lista de perfiles correctamente.
    /// </response>
    [HttpGet]
    public async Task<ActionResult<List<Profile>>> GetProfiles()
    {
        return Ok(await _profileService.GetProfilesAsync());
    }

    /// <summary>
    /// Obtiene la información de un perfil específico a partir de su identificador.
    /// </summary>
    /// <param name="id">
    /// Identificador único del perfil.
    /// </param>
    /// <returns>
    /// Perfil correspondiente al identificador proporcionado.
    /// </returns>
    /// <response code="200">
    /// Perfil encontrado correctamente.
    /// </response>
    /// <response code="404">
    /// No se encontró un perfil con el identificador especificado.
    /// </response>
    [HttpGet("{id}")]
    public async Task<ActionResult<Profile>> GetProfile(string id)
    {
        var profile = await _profileService.GetProfileByIdAsync(id);

        if (profile == null)
            return NotFound();

        return Ok(profile);
    }

    /// <summary>
    /// Crea un nuevo perfil en el sistema.
    /// </summary>
    /// <param name="profile">
    /// Información del perfil que se desea registrar.
    /// </param>
    /// <returns>
    /// Perfil creado correctamente.
    /// </returns>
    /// <remarks>
    /// Este endpoint permite registrar perfiles manualmente mediante la API.
    /// En escenarios automatizados, los perfiles también pueden ser creados a través
    /// del consumidor de mensajes RabbitMQ.
    /// </remarks>
    /// <response code="201">
    /// Perfil creado correctamente.
    /// </response>
    /// <response code="400">
    /// Error en los datos enviados para la creación del perfil.
    /// </response>
    [HttpPost]
    public async Task<ActionResult<Profile>> AddProfile(Profile profile)
    {
        var nuevo = await _profileService.AddProfileAsync(profile);

        return CreatedAtAction(nameof(GetProfile), new { id = nuevo.Id }, nuevo);
    }

    /// <summary>
    /// Actualiza la información de un perfil existente.
    /// </summary>
    /// <param name="id">
    /// Identificador del perfil que se desea actualizar.
    /// </param>
    /// <param name="profile">
    /// Datos actualizados del perfil.
    /// </param>
    /// <returns>
    /// Perfil actualizado correctamente.
    /// </returns>
    /// <response code="200">
    /// Perfil actualizado correctamente.
    /// </response>
    /// <response code="400">
    /// El identificador enviado no coincide con el perfil proporcionado.
    /// </response>
    /// <response code="404">
    /// No se encontró el perfil a actualizar.
    /// </response>
    [HttpPut("{id}")]
    public async Task<ActionResult<Profile>> UpdateProfile(string id, Profile profile)
    {
        if (id != profile.Id)
            return BadRequest();

        var actualizado = await _profileService.UpdateProfileAsync(profile);

        if (actualizado == null)
            return NotFound();

        return Ok(actualizado);
    }
}