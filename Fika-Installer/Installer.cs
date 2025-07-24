using Fika_Installer.Models;

namespace Fika_Installer
{
    public static class Installer
    {
        public static void InstallFika()
        {
            string sptFolder = Utils.BrowseFolder("Please select your SPT installation folder.");
            string fikaFolder = Constants.FikaDirectory;

            if (string.IsNullOrEmpty(sptFolder))
            {
                Menus.MainMenu();
            }

            SptValidationResult sptValidationResult = Utils.ValidateSptFolder(sptFolder);

            if (sptValidationResult != SptValidationResult.OK)
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



            Menus.MainMenu();
        }

        public static void InstallFikaHeadless()
        {

        }
    }
}
