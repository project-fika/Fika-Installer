using Fika_Installer.Models;
using Fika_Installer.Utils;
using SharpHDiffPatch.Core;
using System.Runtime.InteropServices;

namespace Fika_Installer.Spt
{
    public class SptInstaller(string sptDir)
    {
        public bool InstallSpt(string installDir, InstallMethod installType, bool headless = false)
        {
            List<string> excludeFiles =
            [
                "FikaInstallerTemp",
                "Fika-Installer.exe",
                "fika-installer.log",
                "SPTInstaller.exe",
                "Logs",
            ];

            if (headless)
            {
                excludeFiles.AddRange(
                [
                    "SPT",
                    "SPT.Launcher.lnk",
                    "SPT.Server.lnk"
                ]);
            }

            if (installType == InstallMethod.Symlink)
            {
                string eftDataFolderName = "EscapeFromTarkov_Data";

                excludeFiles.Add(eftDataFolderName);

                string escapeFromTarkovDataPath = Path.Combine(sptDir, eftDataFolderName);
                string escapeFromTarkovDataFikaPath = Path.Combine(installDir, eftDataFolderName);

                Logger.Log("Creating symlink...");

                if (!FileUtils.CreateFolderSymlink(escapeFromTarkovDataPath, escapeFromTarkovDataFikaPath))
                {
                    Logger.Error($"An error occurred when creating the symlink.", true);
                    return false;
                }
            }

            Logger.Log("Copying client folder...");

            if (!FileUtils.CopyFolderWithProgress(sptDir, installDir, excludeFiles))
            {
                Logger.Error("An error occurred when copying the client files.", true);
                return false;
            }

            if (headless)
            {
                string sptPatchesDir = Path.Combine(sptDir, @"SPT\SPT_Data\Launcher\Patches");
                string sptPatchesDirDest = Path.Combine(installDir, @"SPT\SPT_Data\Launcher\Patches");

                if (!FileUtils.CopyFolderWithProgress(sptPatchesDir, sptPatchesDirDest, []))
                {
                    return false;
                }
            }

            return true;
        }

        public bool InstallSptRequirements(string installDir)
        {
            Logger.Log("Applying SPT patches...");

            if (!ApplyPatches(installDir))
            {
                Logger.Error("An error occurred when applying SPT patches. Please verify your SPT installation.", true);
                return false;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Logger.Log("Generating SPT shortcuts...");

                if (!GenerateSptShortcuts(installDir))
                {
                    Logger.Error("An error occurred when creating SPT shortcuts.", true);
                    return false;
                }
            }

            Logger.Log("Cleaning up files...");

            if (!CleanupEftFiles(installDir))
            {
                Logger.Error("An error occurred when cleaning up files.", true);
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
            string sptPatchesDir = Path.Combine(installDir, @"SPT\SPT_Data\Launcher\Patches");

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

                        if (!File.Exists(target.FullName))
                        {
                            Logger.Warning($"Patch failed because target file does not exist: {target.Name}.");
                            continue;
                        }

                        if (!ApplyPatch(target.FullName, file.FullName))
                        {
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
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
                Logger.Error($"Failed to patch: {targetFile}!");
                return false;
            }

            return true;
        }

        public bool GenerateSptShortcuts(string installDir)
        {
            try
            {
                string sptFolder = Path.Combine(installDir, "SPT");

                string sptLauncherPath = Path.Combine(sptFolder, "SPT.Launcher.exe");

                if (File.Exists(sptLauncherPath))
                {
                    string sptLauncherShortcutPath = Path.Combine(installDir, "SPT.Launcher.lnk");

                    if (File.Exists(sptLauncherShortcutPath))
                    {
                        File.Delete(sptLauncherShortcutPath);
                    }

                    FileUtils.CreateShortcut(sptLauncherShortcutPath, sptLauncherPath, sptFolder, sptLauncherPath, "SPT Launcher");
                }

                string sptServerPath = Path.Combine(sptFolder, "SPT.Server.exe");

                if (File.Exists(sptServerPath))
                {
                    string sptServerShortcutPath = Path.Combine(installDir, "SPT.Server.lnk");

                    if (File.Exists(sptServerShortcutPath))
                    {
                        File.Delete(sptServerShortcutPath);
                    }

                    FileUtils.CreateShortcut(sptServerShortcutPath, sptServerPath, sptFolder, sptServerPath, "SPT Server");
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);

                return false;
            }
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
                Logger.Error(ex.Message);

                return false;
            }

            return true;
        }
    }
}
