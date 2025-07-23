using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fika_Installer
{
    public static class Installer
    {
        public static bool InstallFika(string sptFolder, string fikaFolder)
        {
            // Initial checks
            string sptServerPath = Path.Combine(sptFolder, "SPT.Server.exe");
            string sptLauncherPath = Path.Combine(sptFolder, "SPT.Launcher.exe");

            if (!File.Exists(sptServerPath) || !File.Exists(sptLauncherPath))
            {
                Console.WriteLine("The selected folder does not contain a valid SPT installation.");
                return false;
            }

            string sptAssemblyCSharpBak = Path.Combine(sptFolder, @"EscapeFromTarkov_Data\Managed\Assembly-CSharp.dll.spt-bak");

            if (!File.Exists(sptAssemblyCSharpBak))
            {
                Console.WriteLine("You must run SPT.Launcher.exe and start the game at least once before you attempt to install Fika using the selected SPT folder.");
                return false;
            }

            string fikaPath = Path.Combine(sptFolder, @"BepInEx\plugins\Fika.Core.dll");
            string fikaHeadlessPath = Path.Combine(sptFolder, @"BepInEx\plugins\Fika.Headless.dll");

            if (File.Exists(fikaPath) || File.Exists(fikaHeadlessPath))
            {
                Console.WriteLine("The selected folder already contains Fika and/or Fika headless. Please select a fresh SPT install folder.");
                return false;
            }

            return true;
        }
    }
}
