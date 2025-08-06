using Fika_Installer.Controllers;
using Fika_Installer.Models;

namespace Fika_Installer.UI
{   
    public class Pages
    {
        private AppController _appController;

        public Pages(AppController appController)
        {
            _appController = appController;
        }
        
        public void InstallFikaPage(bool installInCurrentFolder = false)
        {
            void action() => _appController.FikaInstaller.InstallFika(installInCurrentFolder);

            Page page = new(action);
            page.Show();
        }

        public void UpdateFikaPage()
        {
            Page page = new(_appController.FikaInstaller.UpdateFika);
            page.Show();
        }

        public void InstallFikaHeadlessPage()
        {
            Page page = new(_appController.FikaInstaller.InstallFikaHeadless);
            page.Show();
        }

        public void UpdateFikaHeadlessPage()
        {
            Page page = new(_appController.FikaInstaller.UpdateFikaHeadless);
            page.Show();
        }

        public void SetupHeadlessProfilePage(SptProfile sptProfile, string sptFolder)
        {
            void action() => _appController.FikaHeadless.CopyProfileScript(sptProfile, sptFolder);

            Page page = new(action);
            page.Show();
        }

        public void SetupNewHeadlessProfilePage(string sptFolder)
        {
            void action() => _appController.FikaHeadless.SetupNewProfile(sptFolder);
            
            Page page = new(action);
            page.Show();
        }
    }
}
