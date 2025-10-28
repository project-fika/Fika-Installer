using Fika_Installer.Models;

namespace Fika_Installer
{
    internal static class CLI
    {
        private static void PrintHelp(string? message = null)
        {
            if (message != null)
            {
                Logger.Log(message);
            }

            Console.WriteLine("Fika Installer CLI");
            Console.WriteLine("");
            Console.WriteLine("Supported arguments:");
            Console.WriteLine("  install fika");
            Console.WriteLine("  install headless [headlessProfileId] [installMethod]");
            Console.WriteLine("    headlessProfileId: optional, if not provided (or \"new\") a new profile will be created");
            Console.WriteLine("    installMethod: optional, either: hardcopy, symlink");
            Console.WriteLine("  uninstall");
            Console.WriteLine("  update fika");
            Console.WriteLine("  update headless");

            if (message != null)
            {
                Console.WriteLine("");
                Console.WriteLine(message);
            }

            // Pause to allow user to read
            // Normally, for automated use, Exit(1) is preferred, but
            //   wrong-argument cases are very unlikely to be automated.
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey(true);
            Terminate(1);
        }

        public static void Parse(string[] args)
        {
            switch (args[0].ToLower())
            {
                case "install":
                    Logger.Log("Starting installation");
                    if (args.Length < 2) PrintHelp("Install command requires argument; supported arguments: fika, headless");
                    switch (args[1].ToLower())
                    {
                        case "fika":
                            Logger.Log("Installing: fika");
                            UI.Pages.Methods.InstallFika(Installer.CurrentDir);
                            break;
                        case "headless":
                            Logger.Log("Installing: headless");
                            string? headlessProfileId = null;
                            if (args.Length >= 3)
                            {
                                headlessProfileId = args[2];
                            }
                            InstallMethod? installMethod = null;
                            if (args.Length >= 4)
                            {
                                switch (args[3])
                                {
                                    case "hardcopy":
                                        installMethod = InstallMethod.HardCopy;
                                        break;
                                    case "symlink":
                                        installMethod = InstallMethod.Symlink;
                                        break;
                                    default:
                                        Logger.Error("Invalid install method argument; supported arguments: hardcopy, symlink");
                                        break;
                                }
                            }
                            UI.Pages.Methods.InstallHeadless(Installer.CurrentDir, headlessProfileId, installMethod);
                            break;
                        default:
                            PrintHelp("Invalid install argument; supported arguments: fika, headles");
                            break;
                    }
                    break; // end install

                case "uninstall":
                    Logger.Log("Starting uninstallation");
                    UI.Pages.Methods.Uninstall(Installer.CurrentDir);
                    break; // end uninstall

                case "update":
                    Logger.Log("Starting update");
                    if (args.Length < 2) PrintHelp("Update command requires argument; supported arguments: fika, headless");
                    switch (args[1].ToLower())
                    {
                        case "fika":
                            Logger.Log("Updating: fika");
                            UI.Pages.Methods.UpdateFika(Installer.CurrentDir);
                            break;
                        case "headless":
                            Logger.Log("Updating: headless");
                            UI.Pages.Methods.UpdateHeadless(Installer.CurrentDir);
                            break;
                        default:
                            PrintHelp("Update command requires argument; supported arguments: fika, headless");
                            break;
                    }
                    break; // end update

                default:
                    PrintHelp($"Unknown command-line argument: {args[0]}");
                    break;
            }
            Terminate();
        }

        public static void Terminate(int code = 0)
        {
            /*
             * This is important:
             * Since this process is self-elevating (because of firewall rules),
             * if it is called by any other process, that process *can not* know
             * when fika-installer has finished, unless it is polling the log file.
             * 
             * Because of this, as a temporary workaround, we log a default "Closing" message
             * both on errors as well as on normal termination, so that external
             * processes can monitor the log file for this message to detect termination.
             * 
             * Ideally, we'd only elevate when needed, meaning when we create firewall rules.
             * This could be done like this:
             * - keep track of whether firewall rules have been created (fika-installer.json with {"firewallRulesCreated": true} ?)
             * - when firewall rules should be created, check for that flag first and if needed:
             * - re-launch self with elevation:
                var psi = new ProcessStartInfo
                {
                  FileName = Application.ExecutablePath,
                  Arguments = "create-firewall-rules",
                  Verb = "runas",
                  UseShellExecute = true
                };
                Process.Start(psi);
             * - create-firewall-rules is a new CLI argument that only creates the firewall rules and then exits
             * 
             * This preserves the functionality without forcing elevation all the time,
             * which is a better user experience, generally better practice and allows fika-installer to be
             * used programmatically without as much headache.
             */
            Logger.Log("Closing");
            Environment.Exit(code);
        }
    }
}
