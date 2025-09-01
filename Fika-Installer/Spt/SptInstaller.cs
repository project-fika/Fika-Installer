using Fika_Installer.Models;
using Fika_Installer.Utils;

namespace Fika_Installer.Spt
{
    public class SptInstaller(string installDir, string sptDir)
    {
        private string _installDir = installDir;
        private string _sptDir = sptDir;

        public bool InstallSpt(InstallMethod installType, bool headless = false)
        {
            List<string> excludeFiles = ["FikaInstallerTemp", "Fika-Installer.exe", "SPTInstaller.exe"];

            if (headless)
            {
                excludeFiles.AddRange(
                [
                    SptConstants.ServerExeName,
                    SptConstants.LauncherExeName,
                    "SPT_Data",
                    "user",
                ]);
            }

            if (installType == InstallMethod.Symlink)
            {
                excludeFiles.Add("EscapeFromTarkov_Data");

                string escapeFromTarkovDataPath = Path.Combine(_sptDir, "EscapeFromTarkov_Data");
                string escapeFromTarkovDataFikaPath = Path.Combine(_installDir, "EscapeFromTarkov_Data");

                Console.WriteLine("Creating symlink...");

                bool createSymlinkResult = FileUtils.CreateFolderSymlink(escapeFromTarkovDataPath, escapeFromTarkovDataFikaPath);

                if (!createSymlinkResult)
                {
                    ConUtils.WriteError($"An error occurred when creating the symlink.", true);
                    return false;
                }
            }

            Console.WriteLine("Copying SPT folder...");

            bool copySptFolderResult = FileUtils.CopyFolderWithProgress(_sptDir, _installDir, excludeFiles);

            if (!copySptFolderResult)
            {
                return false;
            }

            return true;
        }
    }
}
