using Fika_Installer.Models;

namespace Fika_Installer.UI
{   
    public static class Pages
    {
        public static void InstallFikaPage()
        {
            Page page = new(Installer.InstallFika);
            page.Show();
        }

        public static void UpdateFikaPage()
        {
            Page page = new(Installer.UpdateFika);
            page.Show();
        }

        public static void InstallFikaHeadlessPage()
        {
            Page page = new(Installer.InstallFikaHeadless);
            page.Show();
        }

        public static void UpdateFikaHeadlessPage()
        {
            Page page = new(Installer.UpdateFikaHeadless);
            page.Show();
        }

        public static void SetupHeadlessProfilePage(SptProfile sptProfile, string sptFolder)
        {
            void action() => Headless.SetupProfile(sptProfile, sptFolder);

            Page page = new(action);
            page.Show();
        }

        public static void SetupNewHeadlessProfilePage(string sptFolder)
        {
            void action() => Headless.SetupNewProfile(sptFolder);
            
            Page page = new(action);
            page.Show();
        }
    }
}
