namespace Fika_Installer.Spt;

public static class SptUtils
{
    public static bool IsSptInstalled(string path)
    {
        var sptPath = Path.Combine(path, "SPT_Runtime");
        var sptServerPath = Path.Combine(sptPath, SptConstants.ServerExeName);
        var sptLauncherPath = Path.Combine(sptPath, SptConstants.LauncherExeName);

        var sptServerFound = File.Exists(sptServerPath);
        var sptLauncherFound = File.Exists(sptLauncherPath);

        return sptServerFound && sptLauncherFound;
    }

    public static bool IsSptFolderDetected(string path)
    {
        var sptFolder = Path.Combine(path, "SPT_Runtime");
        return Directory.Exists(sptFolder);
    }
}
