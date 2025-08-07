using System.Reflection;

namespace Fika_Installer
{
    public static class Constants
    {
        public static readonly string FikaInstallerVersionString = $"Fika Installer v{Assembly.GetExecutingAssembly().GetName().Version}";
        public static readonly string InstallerDirectory = Directory.GetCurrentDirectory();

        public static readonly Dictionary<string, string> FikaReleaseUrls = new()
        {
            { "Fika.Core", "https://api.github.com/repos/project-fika/Fika-Plugin/releases/latest" },
            { "Fika.Headless", "https://api.github.com/repos/project-fika/Fika-Headless/releases/latest" },
            { "Fika.Server", "https://api.github.com/repos/project-fika/Fika-Server/releases/latest" }
        };
    }
}
