using Fika_Installer.Models.Spt;
using Fika_Installer.Utils;
using System.Text.Json.Nodes;

namespace Fika_Installer.Spt
{
    public static class SptUtils
    {
        public static bool IsSptInstalled(string sptDir)
        {
            string sptServerPath = Path.Combine(sptDir, SptConstants.ServerExeName);
            string sptLauncherPath = Path.Combine(sptDir, SptConstants.LauncherExeName);

            bool sptServerFound = File.Exists(sptServerPath);
            bool sptLauncherFound = File.Exists(sptLauncherPath);

            return sptServerFound && sptLauncherFound;
        }

        public static string? BrowseAndValidateSptDir()
        {
            ConUtils.WriteConfirm("SPT not detected. Press ENTER to browse for your SPT folder.");

            string sptDir = FileUtils.BrowseFolder("Please select your SPT installation folder.");

            if (string.IsNullOrEmpty(sptDir))
            {
                return null;
            }

            bool sptValidationResult = ValidateSptFolder(sptDir);

            if (!sptValidationResult)
            {
                ConUtils.WriteError("An error occurred during validation of SPT folder.", true);
                return null;
            }

            return sptDir;
        }

        private static bool ValidateSptFolder(string sptDir)
        {
            if (!IsSptInstalled(sptDir))
            {
                ConUtils.WriteError("The selected folder does not contain a valid SPT installation.", true);
                return false;
            }

            string sptAssemblyCSharpBak = Path.Combine(sptDir, @"EscapeFromTarkov_Data\Managed\Assembly-CSharp.dll.spt-bak");

            if (!File.Exists(sptAssemblyCSharpBak))
            {
                ConUtils.WriteError("You must run SPT.Launcher.exe and start the game at least once before you attempt to install Fika using the selected SPT folder.", true);
                return false;
            }

            return true;
        }

        public static SptProfile? GetProfileFromJson(string sptProfilePath)
        {
            if (File.Exists(sptProfilePath))
            {
                try
                {
                    JsonObject? profile = JsonUtils.DeserializeFromFile(sptProfilePath);

                    if (profile != null)
                    {
                        bool headless = false;

                        string? id = profile["info"]?["id"]?.GetValue<string>();
                        string? username = profile["info"]?["username"]?.GetValue<string>();
                        string? password = profile["info"]?["password"]?.GetValue<string>();

                        if (id != null && username != null && password != null)
                        {
                            headless = password == "fika-headless";

                            SptProfile sptProfile = new(id, username, password, headless);

                            return sptProfile;
                        }
                    }
                }
                catch(Exception ex)
                {
                    ConUtils.WriteError($"Failed to read profile: {sptProfilePath}. {ex.Message}");
                }
            }

            return null;
        }
    }
}
