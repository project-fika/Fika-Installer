using Fika_Installer.Models.Spt;
using Fika_Installer.Utils;
using SharpHDiffPatch.Core;
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

        public static string? BrowseAndValidateSptDir(CompositeLogger? logger = null)
        {
            logger?.Confirm("SPT not detected. Press ENTER to browse for your SPT folder.");

            string sptDir = FileUtils.BrowseFolder("Please select your SPT installation folder.");

            if (string.IsNullOrEmpty(sptDir))
            {
                return null;
            }

            if (!IsSptInstalled(sptDir))
            {
                logger?.Error("The selected folder does not contain a valid SPT installation.", true);
                return null;
            }

            return sptDir;
        }

        public static SptProfile? GetProfileFromJson(string sptProfilePath, CompositeLogger? logger)
        {
            if (File.Exists(sptProfilePath))
            {
                try
                {
                    JsonObject? profile = JsonUtils.DeserializeFromFile(sptProfilePath, logger);

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
                catch (Exception ex)
                {
                    logger?.Error($"Failed to read profile: {sptProfilePath}. {ex.Message}");
                }
            }

            return null;
        }

        /* License: NCSA Open Source License
         * 
         * Copyright: SPT
         * AUTHORS:
         * waffle.lord
         */
        public static bool ApplyPatch(string targetFile, string patchFile, CompositeLogger? logger)
        {
            var backupFile = $"{targetFile}.spt-bak";

            if (!File.Exists(backupFile))
            {
                File.Copy(targetFile, backupFile);
            }

            try
            {
                HDiffPatch patcher = new();
                HDiffPatch.LogVerbosity = Verbosity.Quiet;

                patcher.Initialize(patchFile);
                patcher.Patch(backupFile, targetFile, false, default, false, false);
            }
            catch (Exception ex)
            {
                logger?.Error($"Failed to patch: {targetFile}!");
                return false;
            }

            return true;
        }
    }
}
