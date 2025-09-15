using Fika_Installer.Utils;

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

        public static string? BrowseAndValidateSptDir(CompositeLogger? logger = null)
        {
            logger?.Confirm("SPT not detected. Press ENTER to browse for your SPT folder.");

            string sptDir = FileUtils.BrowseFolder("Please select your SPT installation folder.");

            if (string.IsNullOrEmpty(sptDir))
            {
                return null;
            }

            if (!IsSptInstalled(sptDir))
            {
                logger?.Error("The selected folder does not contain a valid SPT installation.", true);
                return null;
            }

            return sptDir;
        }
    }
}
