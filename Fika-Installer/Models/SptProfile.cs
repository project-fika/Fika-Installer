namespace Fika_Installer.Models
{
    public class SptProfile
    {
        public string ProfileId { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }

        public SptProfile()
        {
            ProfileId = "";
            Name = "";
            Password = "";
        }

        public SptProfile(string profileId, string name, string password)
        {
            ProfileId = profileId;
            Name = name;
            Password = password;
        }
    }
}
