using Fika_Installer.Models;

namespace Fika_Installer
{   
    public static class Installer
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

            string fikaReleaseUrl = Constants.FikaReleases["Fika.Core"];
            string fikaTempPath = Constants.FikaInstallerTemp;

            DownloadReleaseResult downloadFikaResult = DownloadRelease(fikaReleaseUrl, fikaTempPath);

            if (!downloadFikaResult.Result)
            {
                return;
            }

            string fikaReleaseName = downloadFikaResult.Name;
            string fikaReleaseZipFilePath = Path.Combine(fikaTempPath, fikaReleaseName);

            bool extractFikaResult = ExtractRelease(fikaReleaseZipFilePath, Constants.FikaDirectory);

            if (!extractFikaResult)
            {
                return;
            }

            Utils.WriteLineConfirmation("Fika installed successfully.");
        }

        public static void UpdateFika()
        {

        }

        public static void InstallFikaHeadless()
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

            string fikaHeadlessReleaseUrl = Constants.FikaReleases["Fika.Headless"];
            string fikaTempPath = Constants.FikaInstallerTemp;
            
            DownloadReleaseResult downloadHeadlessResult = DownloadRelease(fikaHeadlessReleaseUrl, fikaTempPath);

            if (!downloadHeadlessResult.Result)
            {
                return;
            }
            
            string fikaHeadlessReleaseName = downloadHeadlessResult.Name;
            string fikaHeadlessReleaseZipFilePath = Path.Combine(fikaTempPath, fikaHeadlessReleaseName);
            
            bool extractHeadlessResult = ExtractRelease(fikaHeadlessReleaseZipFilePath, Constants.FikaDirectory);

            if (!extractHeadlessResult)
            {
                return;
            }

            string fikaCorePath = Constants.FikaCorePath;

            if (!File.Exists(fikaCorePath))
            {
                string fikaReleaseUrl = Constants.FikaReleases["Fika.Core"];

                DownloadReleaseResult downloadFikaResult = DownloadRelease(fikaReleaseUrl, fikaTempPath);

                if (!downloadFikaResult.Result)
                {
                    return;
                }

                string fikaReleaseName = downloadFikaResult.Name;
                string fikaReleaseZipFilePath = Path.Combine(fikaTempPath, fikaReleaseName);

                bool extractfikaResult = ExtractRelease(fikaReleaseZipFilePath, Constants.FikaDirectory);

                if (!extractfikaResult)
                {
                    return;
                }
            }

            Utils.WriteLineConfirmation("Fika Headless installed successfully.");
        }

        public static void UpdateFikaHeadless()
        {

        }

        public static bool ValidateSptFolder(string sptFolder)
        {
            string sptServerPath = Path.Combine(sptFolder, "SPT.Server.exe");
            string sptLauncherPath = Path.Combine(sptFolder, "SPT.Launcher.exe");

            if (!File.Exists(sptServerPath) || !File.Exists(sptLauncherPath))
            {
                Console.WriteLine("The selected folder does not contain a valid SPT installation.");
                return false;
            }

            string sptAssemblyCSharpBak = Path.Combine(sptFolder, @"EscapeFromTarkov_Data\Managed\Assembly-CSharp.dll.spt-bak");

            if (!File.Exists(sptAssemblyCSharpBak))
            {
                Console.WriteLine("You must run SPT.Launcher.exe and start the game at least once before you attempt to install Fika using the selected SPT folder.");
                return false;
            }

            return true;
        }

        public static string BrowseSptFolderAndValidate()
        {
            string sptFolder = Utils.BrowseFolder("Please select your SPT installation folder.");

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
                Utils.WriteLineConfirmation("An error occurred during validation of SPT folder.");
                return string.Empty;
            }

            return sptFolder;
        }

        public static bool CopySptFolder(string sptFolder, string fikaFolder)
        {
            bool copySptResult = Utils.CopyFolderWithProgress(sptFolder, fikaFolder);

            if (copySptResult)
            {
                Console.WriteLine("SPT folder copied successfully.");
            }
            else
            {
                Utils.WriteLineConfirmation("An error occurred during copy of SPT folder.");
            }

            return copySptResult;
        }

        public static bool IsSptInstalled(string path)
        {
            string sptServerPath = Constants.SptServerPath;
            string sptLauncherPath = Constants.SptLauncherPath;

            bool sptServerFound = File.Exists(sptServerPath);
            bool sptLauncherFound = File.Exists(sptLauncherPath);

            if (sptServerFound && sptLauncherFound)
            {
                return true;
            }

            return false;
        }

        public static DownloadReleaseResult DownloadRelease(string releaseUrl, string outputDir)
        {
            GitHubAsset[] githubAssets = GitHub.FetchGitHubAssets(releaseUrl);

            DownloadReleaseResult downloadReleaseResult = new();

            if (githubAssets.Length > 0)
            {
                // TODO: Make sure we grab the correct file if there's more than 1 file...
                string releaseName = githubAssets[0].Name;
                string assetUrl = githubAssets[0].Url;

                string outputPath = Path.Combine(outputDir, releaseName);
                bool downloadResult = Utils.DownloadFileWithProgress(assetUrl, outputPath);

                if (downloadResult)
                {
                    Console.WriteLine($"{releaseName} downloaded successfully.");
                }
                else
                {
                    Utils.WriteLineConfirmation($"An error occurred while downloading {releaseName}.");
                }

                downloadReleaseResult.Name = releaseName;
                downloadReleaseResult.Url = assetUrl;
                downloadReleaseResult.Result = downloadResult;
            }

            return downloadReleaseResult;
        }

        public static bool ExtractRelease(string releasePath, string outputDir)
        {
            bool extractResult = false;

            try
            {
                string fileName = Path.GetFileName(releasePath);

                Console.WriteLine($"Extracting {fileName}...");
                Utils.ExtractZip(releasePath, outputDir);

                extractResult = true;
            }
            catch (Exception ex)  
            {
                Console.WriteLine(ex.Message);
            }

            return extractResult;
        }
    }
}
