namespace Fika_Installer.Spt
{
    public static class SptUtils
    {
        public static bool IsSptInstalled(string sptDir)
        {
            string sptServerPath = Path.Combine(sptDir, SptConstants.ServerExeName);
            string sptLauncherPath = Path.Combine(sptDir, SptConstants.LauncherExeName);

            bool sptServerFound = File.Exists(sptServerPath);
            bool sptLauncherFound = File.Exists(sptLauncherPath);

            return sptServerFound && sptLauncherFound;
        }
    }
}
