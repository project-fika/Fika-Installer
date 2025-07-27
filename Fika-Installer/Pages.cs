using Fika_Installer.Models;

namespace Fika_Installer
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
    }
}
