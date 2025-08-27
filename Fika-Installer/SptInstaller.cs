using Fika_Installer.Models;
using Fika_Installer.Utils;

namespace Fika_Installer
{
    public class SptInstaller(string installDir, string sptDir)
    {
        private string _installDir = installDir;
        private string _sptDir = sptDir;
        private string _serverExePath = Path.Combine(sptDir, SptConstants.ServerExeName);
        private string _launcherExePath = Path.Combine(sptDir, SptConstants.LauncherExeName);

        public bool InstallSpt(InstallMethod installType, bool headless = false)
        {
            List<string> excludeFiles = ["FikaInstallerTemp"];

            if (headless)
            {
                excludeFiles.AddRange(
                [
                    SptConstants.ServerExeName,
                    SptConstants.LauncherExeName,
                    "SPTInstaller.exe",
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

        public bool IsSptInstalled()
        {
            bool sptServerFound = File.Exists(_serverExePath);
            bool sptLauncherFound = File.Exists(_launcherExePath);

            if (sptServerFound && sptLauncherFound)
            {
                return true;
            }

            return false;
        }

        public string? BrowseAndValidateSptDir()
        {
            ConUtils.WriteConfirm("SPT not detected. Press ENTER to browse for your SPT folder.");

            string sptDir = FileUtils.BrowseFolder("Please select your SPT installation folder.");

            if (string.IsNullOrEmpty(sptDir))
            {
                return null;
            }

            SetSptDir(sptDir);

            bool sptValidationResult = ValidateSptFolder();

            if (!sptValidationResult)
            {
                ConUtils.WriteError("An error occurred during validation of SPT folder.", true);
                return null;
            }

            return sptDir;
        }

        private bool ValidateSptFolder()
        {
            if (!File.Exists(_serverExePath) || !File.Exists(_launcherExePath))
            {
                ConUtils.WriteError("The selected folder does not contain a valid SPT installation.", true);
                return false;
            }

            string sptAssemblyCSharpBak = Path.Combine(_sptDir, @"EscapeFromTarkov_Data\Managed\Assembly-CSharp.dll.spt-bak");

            if (!File.Exists(sptAssemblyCSharpBak))
            {
                ConUtils.WriteError("You must run SPT.Launcher.exe and start the game at least once before you attempt to install Fika using the selected SPT folder.", true);
                return false;
            }

            return true;
        }

        private void SetSptDir(string sptDir)
        {
            _sptDir = sptDir;
            _serverExePath = Path.Combine(sptDir, SptConstants.ServerExeName);
            _launcherExePath = Path.Combine(sptDir, SptConstants.LauncherExeName);
        }
    }
}
