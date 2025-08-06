using System.Reflection;

namespace Fika_Installer
{
    public static class Constants
    {
        public static readonly string FikaInstallerVersionString = $"Fika Installer v{Assembly.GetExecutingAssembly().GetName().Version}";
        
        public static readonly string InstallerDirectory = Directory.GetCurrentDirectory();
        public static readonly string FikaCorePath = Path.Combine(InstallerDirectory, @"BepInEx\plugins\Fika.Core.dll");
        public static readonly string FikaHeadlessPath = Path.Combine(InstallerDirectory, @"BepInEx\plugins\Fika.Headless.dll");

        public static readonly Dictionary<string, string> FikaReleasesUrl = new()
        {
            { "Fika.Core", "https://api.github.com/repos/project-fika/Fika-Plugin/releases/latest" },
            { "Fika.Headless", "https://api.github.com/repos/project-fika/Fika-Headless/releases/latest" },
            { "Fika.Server", "https://api.github.com/repos/project-fika/Fika-Server/releases/latest" }
        };
    }
}
