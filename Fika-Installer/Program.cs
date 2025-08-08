using Fika_Installer.UI;
using Fika_Installer.Utils;
using System.Reflection;

namespace Fika_Installer
{
    public class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.Title = Constants.FikaInstallerVersionString;

            if (args.Length > 0)
            {
                ProcessArgs(args);
            }

            string fikaCoreReleaseUrl = Constants.FikaReleaseUrls["Fika.Core"];
            string fikaServerReleaseUrl = Constants.FikaReleaseUrls["Fika.Server"];
            string fikaHeadlessReleaseUrl = Constants.FikaReleaseUrls["Fika.Headless"];
            string installerDirectory = Constants.InstallerDirectory;

            MenuFactory menuFactory = new MenuFactory(fikaCoreReleaseUrl, fikaServerReleaseUrl, fikaHeadlessReleaseUrl, installerDirectory);

            while (true)
            {
                Menu mainMenu = menuFactory.CreateMainMenu();
                mainMenu.Show();
            }
        }

        static void ProcessArgs(string[] args)
        {
            if (args.Length == 3 && args[0] == "-symlink")
            {
                string fromPath = args[1];
                string toPath = args[2];

                bool createSymlinkResult = FileUtils.CreateFolderSymlink(fromPath, toPath);

                int exitCode = 0;

                if (!createSymlinkResult)
                {
                    exitCode = 1;
                }

                Environment.Exit(exitCode);
            }

            if (args.Length == 2 && args[0] == "-firewall")
            {
                string firewallScripPath = args[1];
                FwUtils.ExecuteFirewallScript(firewallScripPath);

                Environment.Exit(0);
            }
        }
    }
}