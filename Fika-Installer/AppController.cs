using Fika_Installer.UI;

namespace Fika_Installer
{
    public class AppController
    {
        public Menus Menus { get; set; }
        public Pages Pages { get; set; }

        public FikaInstaller FikaInstaller { get; set; }
        public FikaHeadless FikaHeadless { get; set; }

        public AppController()
        {
            FikaInstaller = new(this);
            FikaHeadless = new();
            
            Menus = new Menus(this);
            Pages = new Pages(this);
        }

        public void Start()
        {
            while (true)
            {
                Menus.MainMenu();
            }
        }
    }
}
