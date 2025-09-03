using Fika_Installer.Models;
using Fika_Installer.Utils;

namespace Fika_Installer.Spt
{
    public class SptInstaller(string installDir, string sptDir)
    {
        private string _installDir = installDir;
        private string _sptDir = sptDir;
        private string _sptPatchesDir = Path.Combine(installDir, @"SPT_Data\Launcher\Patches");

        public bool InstallSpt(InstallMethod installType, bool headless = false)
        {
            List<string> excludeFiles =
            [   "FikaInstallerTemp",
                "Fika-Installer.exe",
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

                string escapeFromTarkovDataPath = Path.Combine(_sptDir, "EscapeFromTarkov_Data");
                string escapeFromTarkovDataFikaPath = Path.Combine(_installDir, "EscapeFromTarkov_Data");

                Console.WriteLine("Creating symlink...");

                if (!FileUtils.CreateFolderSymlink(escapeFromTarkovDataPath, escapeFromTarkovDataFikaPath))
                {
                    ConUtils.WriteError($"An error occurred when creating the symlink.", true);
                    return false;
                }
            }

            Console.WriteLine("Copying SPT folder...");

            if (!FileUtils.CopyFolderWithProgress(_sptDir, _installDir, excludeFiles))
            {
                return false;
            }

            return true;
        }

        public bool InstallSptRequirements()
        {
            Console.WriteLine("Cleaning up files...");

            if (!CleanupEftFiles())
            {
                ConUtils.WriteError("An error occurred when cleaning up files.");
                return false;
            }

            Console.WriteLine("Applying SPT patches...");

            if (!ApplyPatches())
            {
                ConUtils.WriteError("An error occurred when applying SPT patches. Please verify your SPT installation.", true);
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
        public bool ApplyPatches()
        {
            try
            {
                string[] patchesPath = Directory.GetDirectories(_sptPatchesDir);

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
                        target = new FileInfo(Path.Combine(_installDir, relativefile.Replace(".delta", "")));

                        if (!SptUtils.ApplyPatch(target.FullName, file.FullName))
                        {
                            return false;
                        }

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                ConUtils.WriteError(ex.Message);
                return false;
            }

            return true;
        }

        public bool CleanupEftFiles()
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
                    string filePath = Path.Combine(_installDir, file);

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
                ConUtils.WriteError(ex.Message);

                return false;
            }

            return true;
        }
    }
}
