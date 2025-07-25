namespace Fika_Installer
{   
    public static class Installer
    {
        public static void InstallFika(string fikaReleaseUrl)
        {
            string sptFolder = Utils.BrowseFolder("Please select your SPT installation folder.");
            string fikaFolder = Constants.FikaDirectory;

            if (string.IsNullOrEmpty(sptFolder))
            {
                Menus.MainMenu();
            }

            bool sptValidationResult = ValidateSptFolder(sptFolder);

            if (!sptValidationResult)
            {
                Utils.WriteLineConfirmation("An error occurred during installation of Fika.");
                Menus.MainMenu();
            }

            Console.WriteLine($"Found valid installation of SPT: {sptFolder}");

            bool copySptResult = Utils.CopyFolderWithProgress(sptFolder, fikaFolder);

            if (!copySptResult)
            {
                Utils.WriteLineConfirmation("An error occurred during copy of SPT folder.");
                Menus.MainMenu();
            }

            string[] assetsUrl = GitHub.RetrieveAssetsUrl(fikaReleaseUrl);

            foreach (string assetUrl in assetsUrl)
            {
                Console.WriteLine(assetUrl);
            }

            Console.ReadKey();

            Menus.MainMenu();
        }

        public static bool ValidateSptFolder(string sptFolder)
        {
            // Initial checks
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

            string fikaPath = Path.Combine(sptFolder, @"BepInEx\plugins\Fika.Core.dll");
            string fikaHeadlessPath = Path.Combine(sptFolder, @"BepInEx\plugins\Fika.Headless.dll");

            if (File.Exists(fikaPath) || File.Exists(fikaHeadlessPath))
            {
                Console.WriteLine("The selected folder already contains Fika and/or Fika headless. Please select a fresh SPT install folder.");
                return false;
            }

            return true;
        }
    }
}
