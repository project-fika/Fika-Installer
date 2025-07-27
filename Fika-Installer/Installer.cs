namespace Fika_Installer
{   
    public static class Installer
    {
        public static void InstallFika()
        {
            string sptFolder = BrowseSptFolderAndValidate();

            if (string.IsNullOrEmpty(sptFolder))
            {
                return;
            }

            string fikaFolder = Constants.FikaDirectory;

            if (!CopySptFolder(sptFolder, fikaFolder))
            {
                return;
            }

            string fikaReleaseUrl = Constants.FikaReleases["Fika.Core"];
            string outputDir = Path.Combine(Constants.FikaDirectory, @"FikaInstallerTemp\");
            
            DownloadRelease(fikaReleaseUrl, outputDir);
        }

        public static void UpdateFika()
        {

        }

        public static void InstallFikaHeadless()
        {
            string sptFolder = BrowseSptFolderAndValidate();

            if (string.IsNullOrEmpty(sptFolder))
            {
                return;
            }

            string fikaFolder = Constants.FikaDirectory;

            if (!CopySptFolder(sptFolder, fikaFolder))
            {
                return;
            }

            string fikaHeadlessReleaseUrl = Constants.FikaReleases["Fika.Headless"];
            string outputDir = Path.Combine(Constants.FikaDirectory, @"FikaInstallerTemp\");
            
            bool downloadHeadlessResult = DownloadRelease(fikaHeadlessReleaseUrl, outputDir);

            if (!downloadHeadlessResult)
            {
                return;
            }

            string fikaCorePath = Constants.FikaCorePath;

            if (!File.Exists(fikaCorePath))
            {
                string fikaReleaseUrl = Constants.FikaReleases["Fika.Core"];

                bool downloadFikaResult = DownloadRelease(fikaReleaseUrl, outputDir);

                if (!downloadFikaResult)
                {
                    return;
                }
            }
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

            // TODO: check for existing mods (exclude fika.core and fika.headless)

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

            if (!sptValidationResult)
            {
                Utils.WriteLineConfirmation("An error occurred during validation of SPT folder.");
                return string.Empty;
            }

            Console.WriteLine($"Found valid installation of SPT: {sptFolder}");

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

        public static bool DownloadRelease(string releaseUrl, string outputDir)
        {
            GitHubAsset[] githubAssets = GitHub.FetchGitHubAssets(releaseUrl);

            bool downloadResult = false;

            if (githubAssets.Length > 0)
            {
                // TODO: Make sure we grab the correct file if there's more than 1 file...
                string releaseName = githubAssets[0].Name;
                string assetUrl = githubAssets[0].Url;

                string outputPath = Path.Combine(outputDir, releaseName);
                downloadResult = Utils.DownloadFileWithProgress(assetUrl, outputPath);

                if (downloadResult)
                {
                    Console.WriteLine($"{releaseName} downloaded successfully.");
                }
                else
                {
                    Utils.WriteLineConfirmation($"An error occurred while downloading {releaseName}");
                }
            }

            return downloadResult;
        }
    }
}
