using Fika_Installer.Models;
using Fika_Installer.Utils;

namespace Fika_Installer
{
    public class FikaInstaller
    {
        private string _installDir;
        private string _sptFolder;
        private string _tempDir;

        public FikaInstaller(string installDir, string sptFolder)
        {
            _installDir = installDir;
            _sptFolder = sptFolder;
            _tempDir = Path.Combine(_installDir, "FikaInstallerTemp");
        }

        private bool ValidateSptFolder(string sptFolder)
        {
            string sptServerPath = Path.Combine(sptFolder, "SPT.Server.exe");
            string sptLauncherPath = Path.Combine(sptFolder, "SPT.Launcher.exe");

            if (!File.Exists(sptServerPath) || !File.Exists(sptLauncherPath))
            {
                ConUtils.WriteError("The selected folder does not contain a valid SPT installation.", true);
                return false;
            }

            string sptAssemblyCSharpBak = Path.Combine(sptFolder, @"EscapeFromTarkov_Data\Managed\Assembly-CSharp.dll.spt-bak");

            if (!File.Exists(sptAssemblyCSharpBak))
            {
                ConUtils.WriteError("You must run SPT.Launcher.exe and start the game at least once before you attempt to install Fika using the selected SPT folder.", true);
                return false;
            }

            return true;
        }

        public string BrowseSptFolderAndValidate()
        {
            ConUtils.WriteConfirm("SPT not detected. Press ENTER to browse for your SPT folder.");

            string sptFolder = FileUtils.BrowseFolder("Please select your SPT installation folder.");

            if (string.IsNullOrEmpty(sptFolder))
            {
                return string.Empty;
            }

            bool sptValidationResult = ValidateSptFolder(sptFolder);

            if (!sptValidationResult)
            {
                ConUtils.WriteError("An error occurred during validation of SPT folder.", true);
                return string.Empty;
            }

            _sptFolder = sptFolder;

            return sptFolder;
        }

        public bool InstallSpt(InstallMethod installType, bool headless = false)
        {
            List<string> excludeFiles = ["FikaInstallerTemp"];

            if (headless)
            {
                excludeFiles.AddRange(
                [
                    "SPT.Launcher.exe",
                    "SPT.Server.exe",
                    "SPTInstaller.exe",
                    "SPT_Data",
                    "user",
                ]);
            }

            if (installType == InstallMethod.Symlink)
            {
                excludeFiles.Add("EscapeFromTarkov_Data");

                string escapeFromTarkovDataPath = Path.Combine(_sptFolder, "EscapeFromTarkov_Data");
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

            bool copySptFolderResult = FileUtils.CopyFolderWithProgress(_sptFolder, _installDir, excludeFiles);

            if (!copySptFolderResult)
            {
                return false;
            }

            return true;
        }

        public bool IsSptInstalled()
        {
            string sptServerPath = Path.Combine(_installDir, "SPT.Server.exe");
            string sptLauncherPath = Path.Combine(_installDir, "SPT.Launcher.exe");

            bool sptServerFound = File.Exists(sptServerPath);
            bool sptLauncherFound = File.Exists(sptLauncherPath);

            if (sptServerFound && sptLauncherFound)
            {
                return true;
            }

            return false;
        }

        public bool InstallRelease(string url)
        {
            DownloadReleaseResult downloadReleaseResult = DownloadRelease(url, _tempDir);

            if (!downloadReleaseResult.Result)
            {
                return false;
            }

            string releaseName = downloadReleaseResult.Name;
            string releaseZipFilePath = Path.Combine(_tempDir, releaseName);

            bool extractResult = ExtractRelease(releaseZipFilePath, _installDir);

            if (!extractResult)
            {
                return false;
            }

            return true;
        }

        private DownloadReleaseResult DownloadRelease(string releaseUrl, string outputDir)
        {
            GitHubAsset[] githubAssets = GitHub.FetchGitHubAssets(releaseUrl);

            DownloadReleaseResult downloadReleaseResult = new();

            if (githubAssets.Length > 0)
            {
                // TODO: Make sure we grab the correct file if there's more than 1 file...
                string releaseName = githubAssets[0].Name;
                string assetUrl = githubAssets[0].Url;

                Console.WriteLine($"Downloading {releaseName}...");

                string outputPath = Path.Combine(outputDir, releaseName);
                bool downloadResult = FileUtils.DownloadFileWithProgress(assetUrl, outputPath);

                if (!downloadResult)
                {
                    ConUtils.WriteError($"An error occurred while downloading {releaseName}.", true);
                }

                downloadReleaseResult.Name = releaseName;
                downloadReleaseResult.Url = assetUrl;
                downloadReleaseResult.Result = downloadResult;
            }

            return downloadReleaseResult;
        }

        private bool ExtractRelease(string releasePath, string outputDir)
        {
            bool extractResult = false;

            try
            {
                string fileName = Path.GetFileName(releasePath);

                Console.WriteLine($"Extracting {fileName}...");
                FileUtils.ExtractZip(releasePath, outputDir);

                extractResult = true;
            }
            catch (Exception ex)
            {
                ConUtils.WriteError($"An error occurred when extracting the ZIP archive.", true);
            }

            return extractResult;
        }

        public void ApplyFirewallRules(string installDir, string sptFolder)
        {
            Console.WriteLine("Applying Fika firewall rules...");

            string sptServerPath = Path.Combine(sptFolder, "SPT.Server.exe");
            FwUtils.CreateFirewallRule("Fika (SPT) - TCP 6969", "Inbound", "TCP", "6969", sptServerPath);

            string escapeFromTarkovPath = Path.Combine(installDir, "EscapeFromTarkov.exe");
            FwUtils.CreateFirewallRule("Fika (Core) - UDP 25565", "Inbound", "UDP", "25565", escapeFromTarkovPath);
        }
    }
}
