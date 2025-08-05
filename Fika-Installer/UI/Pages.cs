using Fika_Installer.Controllers;
using Fika_Installer.Models;

namespace Fika_Installer.UI
{   
    public static class Pages
    {
        public static void InstallFikaPage()
        {
            Page page = new(FikaInstaller.InstallFika);
            page.Show();
        }

        public static void UpdateFikaPage()
        {
            Page page = new(FikaInstaller.UpdateFika);
            page.Show();
        }

        public static void InstallFikaHeadlessPage()
        {
            Page page = new(FikaInstaller.InstallFikaHeadless);
            page.Show();
        }

        public static void UpdateFikaHeadlessPage()
        {
            Page page = new(FikaInstaller.UpdateFikaHeadless);
            page.Show();
        }

        public static void SetupHeadlessProfilePage(SptProfile sptProfile, string sptFolder)
        {
            FikaHeadless headless = new();
            void action() => headless.SetupProfile(sptProfile, sptFolder);

            Page page = new(action);
            page.Show();
        }

        public static void SetupNewHeadlessProfilePage(string sptFolder)
        {
            FikaHeadless headless = new();
            void action() => headless.SetupNewProfile(sptFolder);
            
            Page page = new(action);
            page.Show();
        }
    }
}
