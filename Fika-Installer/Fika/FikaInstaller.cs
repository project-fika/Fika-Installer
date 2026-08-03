using System.Diagnostics;
using System.Text.RegularExpressions;
using Fika_Installer.Models.Fika;
using Fika_Installer.Models.GitHub;
using Fika_Installer.Utils;

namespace Fika_Installer;

public sealed partial class FikaInstaller(string installDir)
{
    [GeneratedRegex(@"Compatible with EFT ([\d.]+)", RegexOptions.IgnoreCase)]
    private partial Regex CompatibleWithEftVersionRegex();

    public bool InstallReleaseList(List<FikaRelease> fikaReleaseList)
    {
        foreach (var release in fikaReleaseList)
        {
            if (!InstallRelease(release))
            {
                return false;
            }
        }

        return true;
    }

    public bool InstallRelease(FikaRelease fikaRelease)
    {
        var gitHubRelease = GitHub.GetReleaseFromUrl(fikaRelease.Url);

        if (gitHubRelease == null)
        {
            Logger.Error("Failed to retrieve release from Github.", true);
            return false;
        }

        var compatibleEftVersion = GetCompatibleEftVersionFromRelease(gitHubRelease);
        var currentEftVersion = GetEftVersion();

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

        // Ensure to grab the correct asset and file name ends with ".zip" for win release
        GitHubAsset? asset = gitHubRelease.Assets.FirstOrDefault(asset => asset.Name.Contains(fikaRelease.Name) && asset.Name.EndsWith(".zip"));

        if (asset == null)
        {
            Logger.Error("Failed to retrieve asset from release.", true);
            return false;
        }

        var tempDir = Installer.TempDir;

        if (!DownloadRelease(asset, tempDir))
        {
            Logger.Error("Failed to download release.", true);
            return false;
        }

        var assetReleaseName = asset.Name;
        var releaseZipFilePath = Path.Combine(tempDir, assetReleaseName);

        if (!ExtractRelease(releaseZipFilePath, installDir))
        {
            Logger.Error("Failed to extract release.", true);
            return false;
        }

        return true;
    }

    public bool UninstallFika()
    {
        var bepInExPluginsPath = Path.Combine(installDir, @"BepInEx\plugins");
        var bepInExConfigPath = Path.Combine(installDir, @"BepInEx\config");
        var userModsPath = Path.Combine(installDir, @"SPT\user\mods");

        string[] filesToDelete =
        [
            Path.Combine(bepInExPluginsPath, "Fika"),
            Path.Combine(bepInExConfigPath, "com.fika.core.cfg"),
            Path.Combine(bepInExConfigPath, "com.fika.headless.cfg"),
            Path.Combine(userModsPath, "fika-server"),
            Path.Combine(installDir, "HeadlessConfig.json"),
            Path.Combine(installDir, "FikaHeadlessManager.exe"),
            Path.Combine(installDir, "FikaInstallerTemp")
        ];

        try
        {
            foreach (var file in filesToDelete)
            {
                if (File.Exists(file))
                {
                    var fileName = Path.GetFileName(file);
                    Logger.Log($"Removing {fileName}...");

                    File.Delete(file);
                }

                if (Directory.Exists(file))
                {
                    var folderName = Path.GetFileName(file);
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

    public string? GetCompatibleEftVersionFromRelease(GitHubRelease gitHubRelease)
    {
        var match = CompatibleWithEftVersionRegex().Match(gitHubRelease.Body);

        if (match.Success)
        {
            return match.Groups[1].Value;
        }

        return null;
    }

    private string? GetEftVersion()
    {
        var eftExePath = Path.Combine(installDir, EftConstants.GameExeName);

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
        var assetName = asset.Name;
        var assetUrl = asset.BrowserDownloadUrl;

        Logger.Log($"Downloading {assetName}...");

        var outputPath = Path.Combine(outputDir, assetName);

        return FileUtils.DownloadFile(assetUrl, outputPath, Logger.IsInteractive);
    }

    private bool ExtractRelease(string releasePath, string outputDir)
    {
        var fileName = Path.GetFileName(releasePath);

        Logger.Log($"Extracting {fileName}...");

        return FileUtils.ExtractZip(releasePath, outputDir);
    }
}
