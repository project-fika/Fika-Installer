namespace Fika_Installer.Spt
{
    public static class SptUtils
    {
        public static bool IsSptInstalled(string path)
        {
            string sptPath = Path.Combine(path, "SPT");
            string sptServerPath = Path.Combine(sptPath, SptConstants.ServerExeName);
            string sptLauncherPath = Path.Combine(sptPath, SptConstants.LauncherExeName);

            bool sptServerFound = File.Exists(sptServerPath);
            bool sptLauncherFound = File.Exists(sptLauncherPath);

            return sptServerFound && sptLauncherFound;
        }
    }
}
