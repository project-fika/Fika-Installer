using Fika_Installer.Models;
using Fika_Installer.Utils;
using Newtonsoft.Json.Linq;

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
            List<string> excludeFiles = [];

            if (headless)
            {
                excludeFiles =
                [
                    "SPT.Launcher.exe",
                    "SPT.Server.exe",
                    "SPTInstaller.exe",
                    "SPT_Data",
                    "user",
                ];
            }

            if (installType == InstallMethod.HardCopy)
            {
                bool copySptFolderResult = CopySptFolder(_sptFolder, _installDir, excludeFiles);

                if (!copySptFolderResult)
                {
                    return false;
                }
            }

            if (installType == InstallMethod.Symlink)
            {
                string escapeFromTarkovDataPath = Path.Combine(_sptFolder, "EscapeFromTarkov_Data");
                string escapeFromTarkovDataFikaPath = Path.Combine(_installDir, "EscapeFromTarkov_Data");

                bool createSymlinkResult = FileUtils.CreateFolderSymlink(escapeFromTarkovDataPath, escapeFromTarkovDataFikaPath);

                if (!createSymlinkResult)
                {
                    ConUtils.WriteError($"An error occurred when creating the symlink.", true);
                }

                excludeFiles.Add("EscapeFromTarkov_Data");

                bool copySptFolderResult = CopySptFolder(_sptFolder, _installDir, excludeFiles);

                if (!copySptFolderResult)
                {
                    return false;
                }
            }

            return true;
        }

        private bool CopySptFolder(string sptFolder, string fikaFolder, List<string> excludeFiles)
        {
            Console.WriteLine("Copying SPT folder...");

            bool copySptResult = FileUtils.CopyFolderWithProgress(sptFolder, fikaFolder, excludeFiles);

            if (copySptResult)
            {
                Console.WriteLine("SPT folder copied successfully.");
            }
            else
            {
                ConUtils.WriteError("An error occurred during copy of SPT folder.", true);
            }

            return copySptResult;
        }

        public bool IsFikaServerInstalled()
        {
            string fikaServerPath = Path.Combine(_sptFolder, @"user\mods\fika-server");

            if (Directory.Exists(fikaServerPath))
            {
                return true;
            }

            return false;
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

                string outputPath = Path.Combine(outputDir, releaseName);
                bool downloadResult = FileUtils.DownloadFileWithProgress(assetUrl, outputPath);

                if (downloadResult)
                {
                    Console.WriteLine($"{releaseName} downloaded.");
                }
                else
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

        public void ApplyFirewallRules()
        {
            if (!Directory.Exists(_tempDir))
            {
                Directory.CreateDirectory(_tempDir);
            }

            string firewallScriptPath = Path.Combine(_tempDir, @"FikaFirewall.ps1");

            Console.WriteLine("Applying Fika firewall rules...");

            FwUtils.BuildFirewallScript(_installDir, _tempDir);
            FwUtils.ExecuteFirewallScript(firewallScriptPath);
        }

        public void ConfigureSptLauncherConfig()
        {
            string launcherConfigPath = Path.Combine(_installDir, @"user\launcher\config.json");

            JObject launcherConfig = JsonUtils.ReadJson(launcherConfigPath);

            launcherConfig["IsDevMode"] = true;
            launcherConfig["GamePath"] = _installDir;
            launcherConfig["Server"]["Url"] = "https://127.0.0.1:6969";

            JsonUtils.WriteJson(launcherConfig, launcherConfigPath);
        }
    }
}
