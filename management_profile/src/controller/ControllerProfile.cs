using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ProfilesController : ControllerBase
{
    private readonly IProfileService _profileService;

    public ProfilesController(IProfileService profileService)
    {
        _profileService = profileService;
    }

    [HttpGet]
    public async Task<ActionResult<List<Profile>>> GetProfiles()
    {
        return Ok(await _profileService.GetProfilesAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Profile>> GetProfile(string id)
    {
        var profile = await _profileService.GetProfileByIdAsync(id);
        if (profile == null) return NotFound();
        return Ok(profile);
    }
    [HttpPost]
    public async Task<ActionResult<Profile>> AddProfile(Profile profile)
    {
        //se procesa el mensaje recibido por RabbitMQ, se crea un nuevo perfil con el nombre del mensaje y se guarda en la base de datos
        //await _profileService.ProcessMessage(profile.Name);
        var nuevo = await _profileService.AddProfileAsync(profile);
        return CreatedAtAction(nameof(GetProfile), new { id = nuevo.Id }, nuevo);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Profile>> UpdateProfile(string id, Profile profile)
    {
        if (id != profile.Id) return BadRequest();

        var actualizado = await _profileService.UpdateProfileAsync(profile);
        if (actualizado == null) return NotFound();

        return Ok(actualizado);
    }
}
