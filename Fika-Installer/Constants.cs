using System.Reflection;

namespace Fika_Installer
{
    public static class Constants
    {
        public static readonly string FikaInstallerVersionString = $"Fika Installer v{Assembly.GetExecutingAssembly().GetName().Version}";
        
        public static readonly string FikaDirectory = Directory.GetCurrentDirectory();
        public static readonly string FikaCorePath = Path.Combine(FikaDirectory, @"BepInEx\plugins\Fika.Core.dll");
        public static readonly string FikaHeadlessPath = Path.Combine(FikaDirectory, @"BepInEx\plugins\Fika.Headless.dll");
        public static readonly string SptServerPath = Path.Combine(FikaDirectory, "SPT.Server.exe");
        public static readonly string SptLauncherPath = Path.Combine(FikaDirectory, "SPT.Launcher.exe");
        public static readonly string SptUserModsPath = Path.Combine(FikaDirectory, @"user\mods");
        public static readonly string SptProfilesPath = Path.Combine(FikaDirectory, @"user\profiles");

        public static readonly Dictionary<string, string> FikaReleases = new()
        {
            { "Fika", "https://api.github.com/repos/project-fika/Fika-Plugin/releases/latest" },
            { "Fika Headless", "https://api.github.com/repos/project-fika/Fika-Headless/releases/latest" },
        };
    }
}
