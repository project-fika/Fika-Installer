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

        [GeneratedRegex(@"Compatible with EFT ([\d.]+)", RegexOptions.IgnoreCase)]
        private static partial Regex CompatibleWithEftVersionRegex();

        public FikaInstaller(string installDir, SptInstance sptInstance)
        {
            _installDir = installDir;
            _sptInstance = sptInstance;
        }

        public bool InstallReleaseFromUrl(string url)
        {
            GitHubRelease? gitHubRelease = GitHub.GetReleaseFromUrl(url);

            if (gitHubRelease == null)
            {
                return false;
            }

            string? compatibleEftVersion = GetCompatibleEftVersionFromRelease(gitHubRelease);
            string? eftVersion = _sptInstance.EftVersion;

            if (!string.IsNullOrEmpty(compatibleEftVersion) || !string.IsNullOrEmpty(eftVersion))
            {
                if (compatibleEftVersion != eftVersion)
                {
                    ConUtils.WriteError($"{gitHubRelease.Name} is not compatible with your Escape From Tarkov version.");
                    Console.WriteLine();
                    ConUtils.WriteError($"Your version:         {eftVersion}");
                    ConUtils.WriteError($"Compatible version:   {compatibleEftVersion}", true);

                    return false;
                }
            }
            else
            {
                ConUtils.WriteWarning($"WARNING: Could not verify compatibility of {gitHubRelease.Name} with your Escape From Tarkov version. Your Fika installation may not work!");
            }

            // Pick the first asset from release
            GitHubAsset? asset = gitHubRelease.Assets.FirstOrDefault();

            if (asset == null)
            {
                return false;
            }

            string tempDir = InstallerConstants.InstallerTempDir;

            bool downloadReleaseResult = DownloadRelease(asset, tempDir);

            if (!downloadReleaseResult)
            {
                return false;
            }

            string releaseName = asset.Name;
            string releaseZipFilePath = Path.Combine(tempDir, releaseName);

            bool extractResult = ExtractRelease(releaseZipFilePath, _installDir);

            if (!extractResult)
            {
                return false;
            }

            return true;
        }

        public void ApplyFirewallRules()
        {
            Console.WriteLine("Applying Fika firewall rules...");

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

            Console.WriteLine($"Downloading {assetName}...");

            string outputPath = Path.Combine(outputDir, assetName);
            bool downloadResult = FileUtils.DownloadFileWithProgress(assetUrl, outputPath);

            if (!downloadResult)
            {
                ConUtils.WriteError($"An error occurred while downloading {assetName}.", true);
            }

            return downloadResult;
        }

        private bool ExtractRelease(string releasePath, string outputDir)
        {
            try
            {
                string fileName = Path.GetFileName(releasePath);

                Console.WriteLine($"Extracting {fileName}...");
                FileUtils.ExtractZip(releasePath, outputDir);

                return true;
            }
            catch (Exception ex)
            {
                ConUtils.WriteError($"An error occurred when extracting the ZIP archive: {ex.Message}", true);
            }

            return false;
        }
    }
}
