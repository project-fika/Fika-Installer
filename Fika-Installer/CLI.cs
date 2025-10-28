using Fika_Installer.Models.Enums;
using Fika_Installer.Utils;

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
            Console.WriteLine();

            Console.WriteLine("Supported arguments:");
            Console.WriteLine("  install fika [--path <spt_path>] [--method <HardCopy> <Symlink>]");
            Console.WriteLine("  install headless [--path <spt_path>] [--method <HardCopy> <Symlink>] [--profileId <headless_profile_id>]");
            Console.WriteLine("  uninstall");
            Console.WriteLine("  update fika");
            Console.WriteLine("  update headless");
            Console.WriteLine();

            Console.WriteLine("Optional arguments:");
            Console.WriteLine("  --path is the EFT/SPT folder path to copy the files from to create a duplicate instance.");
            Console.WriteLine("  --method is the install method (HardCopy or Symlink). Only works if -path is defined.");
            Console.WriteLine("  --profileId is the headless profile id. If not specified, a new headless profile will be created.");

            if (message != null)
            {
                Console.WriteLine("");
                Console.WriteLine(message);
            }

            Environment.Exit(0);
        }

        public static void Parse(string[] args)
        {
            switch (args[0].ToLower())
            {
                case "install":
                    if (args.Length < 2)
                    {
                        PrintHelp("Install command requires argument. Supported arguments: fika, headless");
                    }

                    string sptFolder = Installer.CurrentDir;
                    InstallMethod installMethod = InstallMethod.HardCopy;
                    string? headlessProfileId = null;

                    for (int i = 0; i < args.Length; i++)
                    {
                        string param = args[i].ToLower();
                        string? paramValue = null;
                        
                        if (i < args.Length - 1)
                        {
                            paramValue = args[i + 1];
                        }

                        switch (param)
                        {
                            case "--path":
                                if (paramValue != null && Directory.Exists(paramValue)) 
                                {
                                    sptFolder = paramValue;
                                } 

                                i++;
                                break;
                            case "--method":
                                if (paramValue != null && !Enum.TryParse(paramValue, out installMethod))
                                {
                                    Logger.Error("Invalid install method argument. Supported arguments: HardCopy, Symlink");
                                    break;
                                }

                                i++;
                                break;
                            case "--profileid":
                                if (paramValue != null && paramValue.Length == 24)
                                {
                                    headlessProfileId = paramValue;
                                }

                                i++;
                                break;
                        }
                    }

                    switch (args[1].ToLower())
                    {
                        case "fika":
                            if (Installer.CurrentDir == sptFolder)
                            {
                                UI.Pages.PageFunctions.InstallFika(Installer.CurrentDir);
                            }
                            else
                            {
                                UI.Pages.PageFunctions.InstallFikaCurrentDir(Installer.CurrentDir, sptFolder, installMethod);
                            }
                            break;
                        case "headless":
                            UI.Pages.PageFunctions.InstallHeadless(Installer.CurrentDir, sptFolder, headlessProfileId, installMethod);
                            break;
                        default:
                            PrintHelp("Invalid install argument. Supported arguments: fika, headless");
                            break;
                    }
                    break; // end install

                case "uninstall":
                    UI.Pages.PageFunctions.UninstallFika(Installer.CurrentDir);
                    break; // end uninstall

                case "update":
                    if (args.Length < 2)
                    {
                        PrintHelp("Update command requires argument. Supported arguments: fika, headless");
                    }
                    switch (args[1].ToLower())
                    {
                        case "fika":
                            UI.Pages.PageFunctions.UpdateFika(Installer.CurrentDir);
                            break;
                        case "headless":
                            UI.Pages.PageFunctions.UpdateHeadless(Installer.CurrentDir);
                            break;
                        default:
                            PrintHelp("Update command requires argument. Supported arguments: fika, headless");
                            break;
                    }
                    break; // end update

                // internal use only
                case "create-firewall-rule":
                    if (args.Length < 2)
                    {
                        PrintHelp("create-firewall-rule command requires install directory argument.");
                    }

                    Console.WriteLine("Creating firewall rule...");
                    FwUtils.CreateFirewallRule(args[1], args[2], args[3], args[4], args[5]);
                    break;

                case "help":
                    PrintHelp();
                    break;

                default:
                    PrintHelp($"Unknown command-line argument: {args[0]}");
                    break;
            }

            Environment.Exit(0);
        }
    }
}
