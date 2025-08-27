using Fika_Installer.Models;
using Fika_Installer.Models.GitHub;
using Fika_Installer.Spt;
using Fika_Installer.Utils;
using System.Text.RegularExpressions;

namespace Fika_Installer
{
    public partial class FikaInstaller
    {
        private string _installDir;
        private SptInstance _sptInstance;
        private string _tempDir;
        
        [GeneratedRegex(@"Compatible with EFT ([\d.]+)", RegexOptions.IgnoreCase)]
        private static partial Regex CompatibleWithEftVersionRegex();

        public FikaInstaller(string installDir, string sptFolder)
        {
            _installDir = installDir;
            _sptInstance = new(sptFolder);
            _tempDir = Path.Combine(_installDir, "FikaInstallerTemp");
        }

        private bool ValidateSptFolder(string sptFolder)
        {
            if (!File.Exists(_sptInstance.ServerExePath) || !File.Exists(_sptInstance.LauncherExePath))
            {
                ConUtils.WriteError("The selected folder does not contain a valid SPT installation.", true);
                return false;
            }

            // TODO: add in SptInstance?
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

            _sptInstance = new(sptFolder);

            bool sptValidationResult = ValidateSptFolder(sptFolder);

            if (!sptValidationResult)
            {
                ConUtils.WriteError("An error occurred during validation of SPT folder.", true);
                return string.Empty;
            }

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

                string escapeFromTarkovDataPath = Path.Combine(_sptInstance.SptPath, "EscapeFromTarkov_Data");
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

            bool copySptFolderResult = FileUtils.CopyFolderWithProgress(_sptInstance.SptPath, _installDir, excludeFiles);

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
            GitHubRelease? gitHubRelease = GitHub.GetReleaseFromUrl(url);

            if (gitHubRelease == null)
            {
                return false;
            }

            string? compatibleEftVersion = GetCompatibleEftVersionFromRelease(gitHubRelease);
            string? eftVersion = _sptInstance.EftVersion;

            // Skip the check if the versions are not available for some reason...
            if (!string.IsNullOrEmpty(compatibleEftVersion) || !string.IsNullOrEmpty(eftVersion))
            {
                if (compatibleEftVersion != eftVersion)
                {
                    ConUtils.WriteError($"{gitHubRelease.Name} is not compatible with your Escape From Tarkov version.");
                    Console.WriteLine();
                    Console.WriteLine();
                    ConUtils.WriteError($"Your version:         {eftVersion}");
                    ConUtils.WriteError($"Compatible version:   {compatibleEftVersion}", true);
                    
                    return false;
                }
            }

            // Always pick the first asset
            GitHubAsset? asset = gitHubRelease.Assets.FirstOrDefault();

            if (asset == null)
            {
                return false;
            }
            
            DownloadReleaseResult downloadReleaseResult = DownloadRelease(asset, _tempDir);

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

        private DownloadReleaseResult DownloadRelease(GitHubAsset asset, string outputDir)
        {
            string assetName = asset.Name;
            string assetUrl = asset.BrowserDownloadUrl;

            Console.WriteLine($"Downloading {assetName}...");

            string outputPath = Path.Combine(outputDir, assetName);
            bool downloadResult = FileUtils.DownloadFileWithProgress(assetUrl, outputPath);

            if (!downloadResult)
            {
                ConUtils.WriteError($"An error occurred while downloading {assetName}.", true);
            }

            DownloadReleaseResult downloadReleaseResult = new(assetName, assetUrl, downloadResult);

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

        public string? GetCompatibleEftVersionFromRelease(GitHubRelease gitHubRelease)
        {
            string body = gitHubRelease.Body;

            Match match = CompatibleWithEftVersionRegex().Match(body);

            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            return null;
        }
    }
}
