using Fika_Installer.Models.GitHub;
using Fika_Installer.Spt;
using Fika_Installer.Utils;
using System.Text.RegularExpressions;

namespace Fika_Installer
{
    public partial class FikaInstaller(string installDir, SptInstance sptInstance, CompositeLogger logger)
    {
        private string _installDir = installDir;
        private SptInstance _sptInstance = sptInstance;
        private CompositeLogger _logger = logger;

        [GeneratedRegex(@"Compatible with EFT ([\d.]+)", RegexOptions.IgnoreCase)]
        private static partial Regex CompatibleWithEftVersionRegex();

        public bool InstallReleaseFromUrl(string url)
        {
            GitHubRelease? gitHubRelease = GitHub.GetReleaseFromUrl(url);

            if (gitHubRelease == null)
            {
                return false;
            }

            string? compatibleEftVersion = GetCompatibleEftVersionFromRelease(gitHubRelease);
            string? eftVersion = _sptInstance.EftVersion;

            if (!string.IsNullOrEmpty(compatibleEftVersion) && !string.IsNullOrEmpty(eftVersion))
            {
                if (compatibleEftVersion != eftVersion)
                {
                    _logger?.Error($"{gitHubRelease.Name} is not compatible with your Escape From Tarkov version.");
                    _logger?.Log("");
                    _logger?.Error($"Your version:         {eftVersion}");
                    _logger?.Error($"Compatible version:   {compatibleEftVersion}", true);

                    return false;
                }
            }
            else
            {
                _logger?.Warning($"Could not verify compatibility of {gitHubRelease.Name} with your Escape From Tarkov version.");
            }

            // Pick the first asset from release
            GitHubAsset? asset = gitHubRelease.Assets.FirstOrDefault();

            if (asset == null)
            {
                return false;
            }

            string tempDir = InstallerConstants.InstallerTempDir;

            if (!DownloadRelease(asset, tempDir))
            {
                return false;
            }

            string releaseName = asset.Name;
            string releaseZipFilePath = Path.Combine(tempDir, releaseName);

            if (!ExtractRelease(releaseZipFilePath, _installDir))
            {
                return false;
            }

            return true;
        }

        public bool UninstallFika()
        {
            string bepInExPluginsPath = Path.Combine(_installDir, @"BepInEx\plugins");
            string bepInExConfigPath = Path.Combine(_installDir, @"BepInEx\config");
            string userModsPath = Path.Combine(_installDir, @"user\mods");

            string[] filesToDelete =
            [
                Path.Combine(bepInExPluginsPath, "Fika.Core.dll"),
                Path.Combine(bepInExPluginsPath, "Fika.Headless.dll"),
                Path.Combine(bepInExConfigPath, "com.fika.core.cfg"),
                Path.Combine(bepInExConfigPath, "com.fika.headless.cfg"),
                Path.Combine(userModsPath, "fika-server"),
            ];

            try
            {
                foreach (string file in filesToDelete)
                {
                    if (File.Exists(file))
                    {
                        string fileName = Path.GetFileName(file);
                        _logger?.Log($"Removing {fileName}...");

                        File.Delete(file);
                    }

                    if (Directory.Exists(file))
                    {
                        string folderName = Path.GetFileName(file);
                        _logger?.Log($"Removing {folderName}...");

                        Directory.Delete(file, true);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger?.Error($"An error occurred during uninstalling Fika: {ex.Message}", true);
                return false;
            }

            return true;
        }

        public void ApplyFirewallRules()
        {
            _logger?.Log("Applying Fika firewall rules...");

            string sptServerPath = Path.Combine(_sptInstance.SptPath, SptConstants.ServerExeName);
            FwUtils.CreateFirewallRule("Fika (SPT) - TCP 6969", "Inbound", "TCP", "6969", sptServerPath);

            string escapeFromTarkovPath = Path.Combine(_installDir, EftConstants.GameExeName);
            FwUtils.CreateFirewallRule("Fika (Core) - UDP 25565", "Inbound", "UDP", "25565", escapeFromTarkovPath);
        }

        public string? GetCompatibleEftVersionFromRelease(GitHubRelease gitHubRelease)
        {
            Match match = CompatibleWithEftVersionRegex().Match(gitHubRelease.Body);

            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            return null;
        }

        private bool DownloadRelease(GitHubAsset asset, string outputDir)
        {
            string assetName = asset.Name;
            string assetUrl = asset.BrowserDownloadUrl;

            _logger?.Log($"Downloading {assetName}...");

            string outputPath = Path.Combine(outputDir, assetName);
            bool downloadResult = FileUtils.DownloadFileWithProgress(assetUrl, outputPath, _logger);

            if (!downloadResult)
            {
                _logger?.Error($"An error occurred while downloading {assetName}.", true);
            }

            return downloadResult;
        }

        private bool ExtractRelease(string releasePath, string outputDir)
        {
            try
            {
                string fileName = Path.GetFileName(releasePath);

                _logger?.Log($"Extracting {fileName}...");
                FileUtils.ExtractZip(releasePath, outputDir, _logger);

                return true;
            }
            catch (Exception ex)
            {
                _logger?.Error($"An error occurred when extracting the ZIP archive: {ex.Message}", true);
            }

            return false;
        }
    }
}
