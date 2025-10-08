using Fika_Installer.Models;
using Fika_Installer.Models.GitHub;
using Fika_Installer.Utils;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Fika_Installer
{
    public partial class FikaInstaller(string installDir)
    {
        [GeneratedRegex(@"Compatible with EFT ([\d.]+)", RegexOptions.IgnoreCase)]
        private partial Regex CompatibleWithEftVersionRegex();

        public bool InstallRelease(FikaRelease fikaRelease)
        {
            GitHubRelease? gitHubRelease = GitHub.GetReleaseFromUrl(fikaRelease.Url);

            if (gitHubRelease == null)
            {
                return false;
            }

            string? compatibleEftVersion = GetCompatibleEftVersionFromRelease(gitHubRelease);
            string? currentEftVersion = GetEftVersion();

            /*
            if (!string.IsNullOrEmpty(compatibleEftVersion) && !string.IsNullOrEmpty(currentEftVersion))
            {
                if (compatibleEftVersion != currentEftVersion)
                {
                    Logger.Error($"{gitHubRelease.Name} is not compatible with your Escape From Tarkov version.");
                    Logger.Error($"Your version:         {currentEftVersion}");
                    Logger.Error($"Compatible version:   {compatibleEftVersion}", true);

                    return false;
                }
            }
            else
            {
                Logger.Warning($"Could not verify compatibility of {gitHubRelease.Name} with your Escape From Tarkov version.");
            }
            */
            GitHubAsset? asset = gitHubRelease.Assets.FirstOrDefault(asset => asset.Name.Contains(fikaRelease.Name));

            if (asset == null)
            {
                return false;
            }

            string tempDir = InstallerConstants.InstallerTempDir;

            if (!DownloadRelease(asset, tempDir))
            {
                return false;
            }

            string assetReleaseName = asset.Name;
            string releaseZipFilePath = Path.Combine(tempDir, assetReleaseName);

            if (!ExtractRelease(releaseZipFilePath, installDir))
            {
                return false;
            }

            return true;
        }

        public bool UninstallFika()
        {
            string bepInExPluginsPath = Path.Combine(installDir, @"BepInEx\plugins");
            string bepInExConfigPath = Path.Combine(installDir, @"BepInEx\config");
            string userModsPath = Path.Combine(installDir, @"SPT\user\mods");

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
                        Logger.Log($"Removing {fileName}...");

                        File.Delete(file);
                    }

                    if (Directory.Exists(file))
                    {
                        string folderName = Path.GetFileName(file);
                        Logger.Log($"Removing {folderName}...");

                        Directory.Delete(file, true);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"An error occurred while uninstalling Fika: {ex.Message}", true);
                return false;
            }

            return true;
        }

        public void ApplyFirewallRules()
        {
            Logger.Log("Applying Fika firewall rules...");

            string sptPath = Path.Combine(installDir, "SPT");
            string sptServerPath = Path.Combine(sptPath, SptConstants.ServerExeName);

            if (File.Exists(sptServerPath))
            {
                FwUtils.CreateFirewallRule("Fika (SPT) - TCP 6969", "Inbound", "TCP", "6969", sptServerPath);
            }

            string escapeFromTarkovPath = Path.Combine(installDir, EftConstants.GameExeName);

            if (File.Exists(escapeFromTarkovPath))
            {
                FwUtils.CreateFirewallRule("Fika (Core) - UDP 25565", "Inbound", "UDP", "25565", escapeFromTarkovPath);
            }
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

        private string? GetEftVersion()
        {
            string eftExePath = Path.Combine(installDir, EftConstants.GameExeName);

            if (File.Exists(eftExePath))
            {
                FileVersionInfo? tarkovVersionInfo = FileVersionInfo.GetVersionInfo(eftExePath);

                if (tarkovVersionInfo.FileVersion != null)
                {
                    return tarkovVersionInfo.FileVersion;
                }
            }

            return null;
        }

        private bool DownloadRelease(GitHubAsset asset, string outputDir)
        {
            string assetName = asset.Name;
            string assetUrl = asset.BrowserDownloadUrl;

            Logger.Log($"Downloading {assetName}...");

            string outputPath = Path.Combine(outputDir, assetName);
            bool downloadResult = FileUtils.DownloadFileWithProgress(assetUrl, outputPath);

            if (!downloadResult)
            {
                Logger.Error($"An error occurred while downloading {assetName}.", true);
            }

            return downloadResult;
        }

        private bool ExtractRelease(string releasePath, string outputDir)
        {
            try
            {
                string fileName = Path.GetFileName(releasePath);

                Logger.Log($"Extracting {fileName}...");
                FileUtils.ExtractZip(releasePath, outputDir);

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error($"An error occurred when extracting the ZIP archive: {ex.Message}", true);
            }

            return false;
        }
    }
}
