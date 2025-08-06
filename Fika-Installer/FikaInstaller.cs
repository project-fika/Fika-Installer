using Fika_Installer.Models;
using Fika_Installer.Models.UI;
using Fika_Installer.UI;
using Fika_Installer.Utils;
using Newtonsoft.Json.Linq;
using System.IO;

namespace Fika_Installer
{   
    public class FikaInstaller(AppController appController)
    {
        private AppController _appController = appController;

        private string _fikaReleaseUrl = Constants.FikaReleasesUrl["Fika.Core"];
        private string _fikaHeadlessReleaseUrl = Constants.FikaReleasesUrl["Fika.Headless"];
        private string _fikaServerReleaseUrl = Constants.FikaReleasesUrl["Fika.Server"];
        private string _fikaDirectory = Constants.FikaDirectory;
        private string _fikaTempPath = Constants.FikaInstallerTemp;

        public void InstallFika()
        {
            bool isSptInstalled = IsSptInstalled(_fikaDirectory);

            if (!isSptInstalled)
            {
                string sptFolder = BrowseSptFolderAndValidate();

                if (string.IsNullOrEmpty(sptFolder))
                {
                    return;
                }

                InstallType installType = _appController.Menus.InstallationTypeMenu();

                bool installSptResult = InstallSpt(installType, sptFolder, _fikaDirectory);

                if (!installSptResult)
                {
                    return;
                }

                ConfigureSptLauncherConfig(_fikaDirectory);
            }

            bool installResult = InstallRelease(_fikaReleaseUrl, _fikaDirectory);

            if (!installResult)
            {
                return;
            }

            bool installFikaServerResult = InstallRelease(_fikaServerReleaseUrl, _fikaDirectory);

            if (!installFikaServerResult)
            {
                return;
            }

            ConUtils.WriteSuccess("Fika installed successfully!", true);
        }

        public void UpdateFika()
        {
            bool installResult = InstallRelease(_fikaReleaseUrl, _fikaDirectory);

            if (!installResult)
            {
                return;
            }

            bool installFikaServerResult = InstallRelease(_fikaServerReleaseUrl, _fikaDirectory);

            if (!installFikaServerResult)
            {
                return;
            }

            ConUtils.WriteSuccess("Fika updated successfully.", true);
        }

        public void InstallFikaHeadless()
        {
            string sptFolder = BrowseSptFolderAndValidate();

            if (string.IsNullOrEmpty(sptFolder))
            {
                return;
            }

            bool isFikaServerInstalled = IsFikaServerInstalled(sptFolder);

            if (!isFikaServerInstalled)
            {
                ConUtils.WriteError("Fika-Server must be installed in your SPT folder before setting up the headless!", true);
                return;
            }

            _appController.Menus.ProfileSelectionMenu(sptFolder);

            bool isSptInstalled = IsSptInstalled(_fikaDirectory);

            if (!isSptInstalled)
            {
                InstallType installType = _appController.Menus.InstallationTypeMenu();

                bool installSptResult = InstallSpt(installType, sptFolder, _fikaDirectory, true);

                if (!installSptResult)
                {
                    return;
                }
            }

            bool installHeadlessResult = InstallRelease(_fikaHeadlessReleaseUrl, _fikaDirectory);

            if (!installHeadlessResult)
            {
                return;
            }

            string fikaCorePath = Constants.FikaCorePath;

            if (!File.Exists(fikaCorePath))
            {
                bool installFikaResult = InstallRelease(_fikaReleaseUrl, _fikaDirectory);

                if (!installFikaResult)
                {
                    return;
                }
            }

            ConUtils.WriteSuccess("Fika Headless installed successfully.", true);
        }

        public void UpdateFikaHeadless()
        {
            bool installHeadlessResult = InstallRelease(_fikaHeadlessReleaseUrl, _fikaDirectory);

            if (!installHeadlessResult)
            {
                return;
            }

            bool installFikaResult = InstallRelease(_fikaReleaseUrl, _fikaDirectory);

            if (!installFikaResult)
            {
                return;
            }

            ConUtils.WriteSuccess("Fika Headless updated successfully.", true);
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

        private string BrowseSptFolderAndValidate()
        {
            Console.WriteLine("You will be prompted to browse for your SPT folder when pressing 'ENTER'.\r\n");
            Console.WriteLine("For Fika install: make sure it is a new SPT installation with no mods installed.");
            Console.WriteLine("For Fika Headless install: make sure to browse for your SPT server with Fika-Server installed.\r\n");
            ConUtils.WriteConfirm("Press ENTER to continue.");
            
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

            return sptFolder;
        }

        private bool InstallSpt(InstallType installType, string sptFolder, string fikaFolder, bool headless = false)
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

            if (installType == InstallType.HardCopy)
            {
                bool copySptFolderResult = CopySptFolder(sptFolder, fikaFolder, excludeFiles);

                if (!copySptFolderResult)
                {
                    return false;
                }
            }

            if (installType == InstallType.Symlink)
            {
                string escapeFromTarkovDataPath = Path.Combine(sptFolder, "EscapeFromTarkov_Data");
                string escapeFromTarkovDataFikaPath = Path.Combine(fikaFolder, "EscapeFromTarkov_Data");

                try
                {
                    Directory.CreateSymbolicLink(escapeFromTarkovDataFikaPath, escapeFromTarkovDataPath);
                }
                catch (Exception ex)
                {
                    ConUtils.WriteError("An error occurred when creating the symlink.", true);
                    return false;
                }

                excludeFiles.Add("EscapeFromTarkov_Data");
                
                bool copySptFolderResult = CopySptFolder(sptFolder, fikaFolder, excludeFiles);

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

        private bool IsFikaServerInstalled(string sptFolder)
        {
            string fikaServerPath = Path.Combine(sptFolder, @"user\mods\fika-server");

            if (Directory.Exists(fikaServerPath))
            {
                return true;
            }

            return false;
        }

        private bool IsSptInstalled(string path)
        {
            string sptServerPath = Path.Combine(path, "SPT.Server.exe");
            string sptLauncherPath = Path.Combine(path, "SPT.Launcher.exe");

            bool sptServerFound = File.Exists(sptServerPath);
            bool sptLauncherFound = File.Exists(sptLauncherPath);

            if (sptServerFound && sptLauncherFound)
            {
                return true;
            }

            return false;
        }

        public bool InstallRelease(string url, string destinationPath)
        {
            DownloadReleaseResult downloadReleaseResult = DownloadRelease(url, _fikaTempPath);

            if (!downloadReleaseResult.Result)
            {
                return false;
            }

            string releaseName = downloadReleaseResult.Name;
            string releaseZipFilePath = Path.Combine(_fikaTempPath, releaseName);

            bool extractResult = ExtractRelease(releaseZipFilePath, destinationPath);

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
                ConUtils.WriteError($"An error occurred when extracting the ZIP archive." , true);
            }

            return extractResult;
        }

        public void ConfigureSptLauncherConfig(string fikaDirectory)
        {
            string launcherConfigPath = Path.Combine(fikaDirectory, @"user\launcher\config.json");

            JObject launcherConfig = JsonUtils.ReadJson(launcherConfigPath);

            launcherConfig["IsDevMode"] = true;
            launcherConfig["GamePath"] = fikaDirectory;
            launcherConfig["Server"]["Url"] = "https://127.0.0.1:6969";

            JsonUtils.WriteJson(launcherConfig, launcherConfigPath);
        }
    }
}
