using Fika_Installer.Models;

namespace Fika_Installer
{
    public static class Headless
    {
        public static void SetupProfile(SptProfile sptProfile, string sptFolder)
        {
            string sptUserModsPath = Path.Combine(sptFolder, @"user\mods\");
            string fikaServerModPath = Path.Combine(sptUserModsPath, @"fika-server\");
            string fikaServerScriptsPath = Path.Combine(fikaServerModPath, @"assets\scripts\");
            string headlessProfileStartScript = $"Start_headless_{sptProfile.ProfileId}";

            string headlessProfileStartScriptPath = Path.Combine(fikaServerScriptsPath, headlessProfileStartScript);

            if (File.Exists(headlessProfileStartScriptPath))
            {
                string fikaFolder = Constants.FikaDirectory;
                File.Copy(headlessProfileStartScriptPath, fikaFolder, true);

                string headlessProfileStartScriptName = Path.GetFileName(headlessProfileStartScriptPath);

                Utils.WriteLineConfirm($"{headlessProfileStartScriptName} has been copied to the root of your Fika install!");
            }
        }

        public static void SetupNewProfile(string sptFolder)
        {

        }
    }
}
