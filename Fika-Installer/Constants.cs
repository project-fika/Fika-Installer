using Fika_Installer.Models;
using System.Reflection;

namespace Fika_Installer
{
    public static class InstallerConstants
    {
        private static readonly string _versionMajor = Assembly.GetExecutingAssembly().GetName().Version.Major.ToString();
        private static readonly string _versionMinor = Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString();
        private static readonly string _versionBuild = Assembly.GetExecutingAssembly().GetName().Version.Build.ToString();

        public static readonly string VersionString = $"Fika Installer v{_versionMajor}.{_versionMinor}.{_versionBuild}";
        public static readonly string InstallerDir = Directory.GetCurrentDirectory();
        public static readonly string InstallerTempDir = Path.Combine(InstallerDir, "FikaInstallerTemp");
    }

    public static class FikaConstants
    {
        public static readonly Dictionary<string, FikaRelease> FikaReleases = new()
        {
            { "Fika.Core", new("Fika.Release", "https://api.github.com/repos/project-fika/Fika-Plugin/releases/latest") },
            { "Fika.Headless", new("Fika.Headless", "https://api.github.com/repos/project-fika/Fika-Headless/releases/latest") },
            { "Fika.Server", new("fika-server", "https://api.github.com/repos/project-fika/Fika-Server/releases/latest") }
        };
    }

    public static class SptConstants
    {
        public static readonly string ServerExeName = "SPT.Server.exe";
        public static readonly string LauncherExeName = "SPT.Launcher.exe";
    }

    public static class EftConstants
    {
        public static readonly string GameExeName = "EscapeFromTarkov.exe";
    }
}