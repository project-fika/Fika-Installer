namespace Fika_Installer.Models.Spt
{
    public class SptProfile(string profileId, string name, string password, bool headless)
    {
        public string ProfileId { get; set; } = profileId;
        public string Name { get; set; } = name;
        public string Password { get; set; } = password;
        public bool Headless { get; set; } = headless;
    }
}
