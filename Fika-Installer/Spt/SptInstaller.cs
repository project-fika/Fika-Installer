using Fika_Installer.Models;
using Fika_Installer.Utils;
using SharpHDiffPatch.Core;

namespace Fika_Installer.Spt
{
    public class SptInstaller(string sptDir, CompositeLogger? logger)
    {
        //private string _sptPatchesDir = Path.Combine(installDir, @"SPT_Data\Launcher\Patches");

        public bool InstallSpt(string installDir, InstallMethod installType, bool headless = false)
        {
            List<string> excludeFiles =
            [   "FikaInstallerTemp",
                "Fika-Installer.exe",
                "fika-installer.log",
                "SPTInstaller.exe",
                "Logs"
            ];

            if (headless)
            {
                excludeFiles.AddRange(
                [
                    SptConstants.ServerExeName,
                    SptConstants.LauncherExeName,
                    @"SPT_Data\Server",
                    @"SPT_Data\Launcher\Locales",
                    "user",
                ]);
            }

            if (installType == InstallMethod.Symlink)
            {
                excludeFiles.Add("EscapeFromTarkov_Data");

                string escapeFromTarkovDataPath = Path.Combine(sptDir, "EscapeFromTarkov_Data");
                string escapeFromTarkovDataFikaPath = Path.Combine(installDir, "EscapeFromTarkov_Data");

                logger?.Log("Creating symlink...");

                if (!FileUtils.CreateFolderSymlink(escapeFromTarkovDataPath, escapeFromTarkovDataFikaPath, logger))
                {
                    logger?.Error($"An error occurred when creating the symlink.", true);
                    return false;
                }
            }

            logger?.Log("Copying SPT folder...");

            if (!FileUtils.CopyFolderWithProgress(sptDir, installDir, excludeFiles, logger))
            {
                return false;
            }

            return true;
        }

        public bool InstallSptRequirements(string installDir)
        {
            logger?.Log("Applying SPT patches...");

            if (!ApplyPatches(installDir))
            {
                logger?.Error("An error occurred when applying SPT patches. Please verify your SPT installation.", true);
                return false;
            }

            logger?.Log("Cleaning up files...");

            if (!CleanupEftFiles(installDir))
            {
                logger?.Error("An error occurred when cleaning up files.", true);
                return false;
            }

            return true;
        }

        /* License: NCSA Open Source License
         * 
         * Copyright: SPT
         * AUTHORS:
         * waffle.lord
         */
        public bool ApplyPatches(string installDir)
        {
            string sptPatchesDir = Path.Combine(installDir, @"SPT_Data\Launcher\Patches");

            try
            {
                string[] patchesPath = Directory.GetDirectories(sptPatchesDir);

                foreach (string patchPath in patchesPath)
                {
                    DirectoryInfo patchPathDirectoryInfo = new(patchPath);

                    // get all patch files within patchpath and it's sub directories.
                    var patchfiles = patchPathDirectoryInfo.GetFiles("*.delta", SearchOption.AllDirectories);

                    foreach (FileInfo file in patchfiles)
                    {
                        FileInfo target;

                        // get the relative portion of the patch file that will be appended to targetpath in order to create an official target file.
                        var relativefile = file.FullName.Substring(patchPath.Length).TrimStart('\\', '/');

                        // create a target file from the relative patch file while utilizing targetpath as the root directory.
                        target = new FileInfo(Path.Combine(installDir, relativefile.Replace(".delta", "")));

                        if (!ApplyPatch(target.FullName, file.FullName))
                        {
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger?.Error(ex.Message);
                return false;
            }

            return true;
        }

        /* License: NCSA Open Source License
         * 
         * Copyright: SPT
         * AUTHORS:
         * waffle.lord
         */
        public bool ApplyPatch(string targetFile, string patchFile)
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
            catch
            {
                logger?.Error($"Failed to patch: {targetFile}!");
                return false;
            }

            return true;
        }

        public bool CleanupEftFiles(string installDir)
        {
            string[] filesToCleanup =
            [
                "BattlEye",
                "Logs",
                "ConsistencyInfo",
                "EscapeFromTarkov_BE.exe",
                "Uninstall.exe",
                "UnityCrashHandler64.exe",
                "WinPixEventRuntime.dll"
            ];

            try
            {
                foreach (string file in filesToCleanup)
                {
                    string filePath = Path.Combine(installDir, file);

                    if (Directory.Exists(filePath))
                    {
                        Directory.Delete(filePath, true);
                    }

                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                }
            }
            catch (Exception ex)
            {
                logger?.Error(ex.Message);

                return false;
            }

            return true;
        }
    }
}
