using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fika_Installer
{
    public enum Pages
    {
        MainMenu,
        InstallFika,
        InstallFikaHeadless,
        UpdateFika,
        UpdateFikaHeadless,
    }
    
    public static class Page
    {
        public static void Show(Pages page)
        {
            InitPage();
            
            switch (page)
            {
                case Pages.MainMenu:
                    MainMenu();
                    break;
                case Pages.InstallFika:
                    InstallFikaPage();
                    break;
                case Pages.InstallFikaHeadless:
                    InstallFikaHeadlessPage();
                    break;
                case Pages.UpdateFika:
                    UpdateFikaPage();
                    break;
                case Pages.UpdateFikaHeadless:
                    UpdateFikaHeadlessPage();
                    break;
            }
        }
        private static void InitPage()
        {
            Console.Clear();

            Console.WriteLine("Fika-Installer v0.1");
            Console.WriteLine("");
        }

        private static void MainMenu()
        {
            bool isFikaDetected = false;
            
            if (File.Exists(Constants.FikaPath))
            {
                isFikaDetected = true;
            }

            bool isFikaHeadlessDetected = false;

            if (File.Exists(Constants.FikaHeadlessPath))
            {
                isFikaHeadlessDetected = true;
            }

            if (isFikaDetected)
            {
                Console.WriteLine("[1] Update Fika");
            }
            else
            {
                Console.WriteLine("[1] Install Fika");
            }

            if (isFikaHeadlessDetected)
            {
                Console.WriteLine("[2] Update Fika Headless");
            }
            else
            {
                Console.WriteLine("[2] Install Fika Headless");
            }

            ConsoleKeyInfo consoleKey = Console.ReadKey();

            switch (consoleKey.Key)
            {
                case ConsoleKey.D1:
                    if (isFikaDetected)
                    {
                        Show(Pages.UpdateFika);
                    }
                    else
                    {
                        Show(Pages.InstallFika);
                    }
                    break;

                case ConsoleKey.D2:
                    if (isFikaHeadlessDetected)
                    {
                        Show(Pages.UpdateFikaHeadless);
                    }
                    else
                    {
                        Show(Pages.InstallFikaHeadless);
                    }
                    break;
            }
        }
        private static void InstallFikaPage()
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Please select your SPT installation directory.";

                DialogResult result = dialog.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {
                    string sptFolder = dialog.SelectedPath;
                    string fikaFolder = Directory.GetCurrentDirectory();

                    if (Installer.InstallFika(sptFolder, fikaFolder))
                    {
                        Console.WriteLine("Installation successfull!");
                    }
                    else
                    {
                        Console.WriteLine("Installation failed!");
                    }

                    Console.ReadKey();
                }
            }

            Show(Pages.MainMenu);
        }

        private static void UpdateFikaPage()
        {

        }

        private static void InstallFikaHeadlessPage()
        {

        }

        private static void UpdateFikaHeadlessPage()
        {

        }
    }
}
