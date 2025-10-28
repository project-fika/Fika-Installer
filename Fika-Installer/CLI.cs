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
            Environment.Exit(0);
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

                // internal use only
                case "create-firewall-rules":
                    if (args.Length < 2) PrintHelp("create-firewall-rules command requires install directory argument");
                    Console.WriteLine("Creating firewall rules..."); // No Logger here, as this is the helper process
                    Utils.FwUtils.ElevatedSetRules(args[1]);
                    break;

                default:
                    PrintHelp($"Unknown command-line argument: {args[0]}");
                    break;
            }
            Environment.Exit(0);
        }
    }
}
