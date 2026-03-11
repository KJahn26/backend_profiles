public interface IProfileService
{
    Task<List<Profile>> GetProfilesAsync();
    Task<Profile?> GetProfileByIdAsync(string id);
    Task<Profile> AddProfileAsync(Profile profile);
    Task<Profile?> UpdateProfileAsync(Profile profile);
    Task<bool> DeleteProfileAsync(string id);
}