using Fika_Installer.Models;
using Fika_Installer.UI;
using Fika_Installer.Utils;

namespace Fika_Installer
{   
    public static class FikaInstaller
    {
        public static void InstallFika()
        {
            string fikaFolder = Constants.FikaDirectory;

            bool isSptInstalled = IsSptInstalled(fikaFolder);

            if (!isSptInstalled)
            {
                string sptFolder = BrowseSptFolderAndValidate();

                if (string.IsNullOrEmpty(sptFolder))
                {
                    return;
                }

                bool copySptFolderResult = CopySptFolder(sptFolder, fikaFolder);

                if (!copySptFolderResult)
                {
                    return;
                }
            }

            string fikaReleaseUrl = Constants.FikaReleasesUrl["Fika.Core"];
            string fikaDirectory = Constants.FikaDirectory;

            bool installResult = InstallRelease(fikaReleaseUrl, fikaDirectory);

            if (!installResult)
            {
                return;
            }

            string fikaServerReleaseUrl = Constants.FikaReleasesUrl["Fika.Server"];

            bool installFikaServerResult = InstallRelease(fikaServerReleaseUrl, fikaDirectory);

            if (!installFikaServerResult)
            {
                return;
            }

            ConUtils.WriteSuccess("Fika installed successfully!", true);
        }

        public static void UpdateFika()
        {
            string fikaReleaseUrl = Constants.FikaReleasesUrl["Fika.Core"];
            string fikaDirectory = Constants.FikaDirectory;

            bool installResult = InstallRelease(fikaReleaseUrl, fikaDirectory);

            if (!installResult)
            {
                return;
            }

            string fikaServerReleaseUrl = Constants.FikaReleasesUrl["Fika.Server"];

            bool installFikaServerResult = InstallRelease(fikaServerReleaseUrl, fikaDirectory);

            if (!installFikaServerResult)
            {
                return;
            }

            ConUtils.WriteSuccess("Fika updated successfully.", true);
        }

        public static void InstallFikaHeadless()
        {
            string fikaFolder = Constants.FikaDirectory;

            string sptFolder = BrowseSptFolderAndValidate();

            if (string.IsNullOrEmpty(sptFolder))
            {
                return;
            }

            Menus.ProfileSelectionMenu(sptFolder);

            bool isSptInstalled = IsSptInstalled(fikaFolder);

            if (!isSptInstalled) 
            {
                bool copySptFolderResult = CopySptFolder(sptFolder, fikaFolder, true);

                if (!copySptFolderResult)
                {
                    return;
                }
            }

            string fikaHeadlessReleaseUrl = Constants.FikaReleasesUrl["Fika.Headless"];
            string fikaDirectory = Constants.FikaDirectory;

            bool installHeadlessResult = InstallRelease(fikaHeadlessReleaseUrl, fikaDirectory);

            if (!installHeadlessResult)
            {
                return;
            }

            string fikaCorePath = Constants.FikaCorePath;

            if (!File.Exists(fikaCorePath))
            {
                string fikaReleaseUrl = Constants.FikaReleasesUrl["Fika.Core"];

                bool installFikaResult = InstallRelease(fikaReleaseUrl, fikaDirectory);

                if (!installFikaResult)
                {
                    return;
                }
            }

            ConUtils.WriteSuccess("Fika Headless installed successfully.", true);
        }

        public static void UpdateFikaHeadless()
        {
            string fikaHeadlessReleaseUrl = Constants.FikaReleasesUrl["Fika.Headless"];
            string fikaDirectory = Constants.FikaDirectory;

            bool installHeadlessResult = InstallRelease(fikaHeadlessReleaseUrl, fikaDirectory);

            if (!installHeadlessResult)
            {
                return;
            }

            string fikaReleaseUrl = Constants.FikaReleasesUrl["Fika.Core"];

            bool installFikaResult = InstallRelease(fikaReleaseUrl, fikaDirectory);

            if (!installFikaResult)
            {
                return;
            }

            ConUtils.WriteSuccess("Fika Headless updated successfully.", true);
        }

        private static bool ValidateSptFolder(string sptFolder)
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

        private static string BrowseSptFolderAndValidate()
        {
            string sptFolder = FileUtils.BrowseFolder("Please select your SPT installation folder.");

            if (string.IsNullOrEmpty(sptFolder))
            {
                return string.Empty;
            }

            bool sptValidationResult = ValidateSptFolder(sptFolder);

            if (sptValidationResult)
            {
                Console.WriteLine($"Found valid installation of SPT: {sptFolder}");
            }
            else
            {
                ConUtils.WriteError("An error occurred during validation of SPT folder.", true);
                return string.Empty;
            }

            return sptFolder;
        }

        private static bool CopySptFolder(string sptFolder, string fikaFolder, bool excludeSptFiles = false)
        {
            // TODO: exclude SPT.Server.exe for headless to avoid confusion?
            
            Console.WriteLine("Copying SPT folder...");
            
            bool copySptResult = FileUtils.CopyFolderWithProgress(sptFolder, fikaFolder);

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

        private static bool IsSptInstalled(string path)
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

        public static bool InstallRelease(string url, string destinationPath)
        {
            string fikaTempPath = Constants.FikaInstallerTemp;

            DownloadReleaseResult downloadReleaseResult = DownloadRelease(url, fikaTempPath);

            if (!downloadReleaseResult.Result)
            {
                return false;
            }

            string releaseName = downloadReleaseResult.Name;
            string releaseZipFilePath = Path.Combine(fikaTempPath, releaseName);

            bool extractResult = ExtractRelease(releaseZipFilePath, destinationPath);

            if (!extractResult)
            {
                return false;
            }

            return true;
        }

        private static DownloadReleaseResult DownloadRelease(string releaseUrl, string outputDir)
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

        private static bool ExtractRelease(string releasePath, string outputDir)
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
    }
}
