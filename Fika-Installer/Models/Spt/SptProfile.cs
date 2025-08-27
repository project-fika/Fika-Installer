namespace Fika_Installer.Models.Spt
{
    public class SptProfile
    {
        public string ProfileId { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public bool Headless { get; set; }

        public SptProfile(string profileId, string name, string password, bool headless)
        {
            ProfileId = profileId;
            Name = name;
            Password = password;
            Headless = headless;
        }
    }
}
