using Microsoft.EntityFrameworkCore;


public class ProfileService : IProfileService
{
    private readonly DataContext _context;

    public ProfileService(DataContext context)
    {
        _context = context;
    }

    public async Task<List<Profile>> GetProfilesAsync()
    {
        return await _context.profiles.ToListAsync();
    }

    public async Task<Profile?> GetProfileByIdAsync(string id)
    {
        return await _context.profiles.FindAsync(id);
    }

    public async Task<Profile> AddProfileAsync(Profile profile)
    {
        profile.FechaCreacion = DateTime.UtcNow;
        _context.profiles.Add(profile);
        await _context.SaveChangesAsync();
        return profile;
    }

    public async Task<Profile?> UpdateProfileAsync(Profile profile)
    {
        var existing = await _context.profiles.FindAsync(profile.Id);
        if (existing == null) return null;

        _context.Entry(existing).CurrentValues.SetValues(profile);
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteProfileAsync(string id)
    {
        var profile = await _context.profiles.FindAsync(id);
        if (profile == null) return false;

        _context.profiles.Remove(profile);
        await _context.SaveChangesAsync();
        return true;
    }
}
