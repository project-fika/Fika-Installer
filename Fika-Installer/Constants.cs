namespace Fika_Installer
{
    public static class Constants
    {
        private static readonly string currentDirectory = Directory.GetCurrentDirectory();

        public static readonly string FikaPath = Path.Combine(currentDirectory, @"BepInEx\plugins\Fika.Core.dll");
        public static readonly string FikaHeadlessPath = Path.Combine(currentDirectory, @"BepInEx\plugins\Fika.Headless.dll");
        public static readonly string SptServerPath = Path.Combine(currentDirectory, "SPT.Server.exe");
        public static readonly string SptLauncherPath = Path.Combine(currentDirectory, "SPT.Launcher.exe");
        public static readonly string SptUserModsPath = Path.Combine(currentDirectory, @"user\mods");
        public static readonly string SptProfilesPath = Path.Combine(currentDirectory, @"user\profiles");
    }
}
