namespace Fika_Installer.Models.Spt
{
    public class SptProfile(string profileId, string name, bool headless)
    {
        public string ProfileId { get; set; } = profileId;
        public string Name { get; set; } = name;
        public bool Headless { get; set; } = headless;
    }
}
