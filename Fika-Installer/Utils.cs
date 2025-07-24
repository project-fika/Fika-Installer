using Fika_Installer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fika_Installer
{
    public static class Utils
    {        
        public static string BrowseFolder(string description)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = description;

                DialogResult result = dialog.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {
                    return dialog.SelectedPath;
                }
            }

            return string.Empty;
        }

        public static SptValidationResult ValidateSptFolder(string sptFolder)
        {
            // Initial checks
            string sptServerPath = Path.Combine(sptFolder, "SPT.Server.exe");
            string sptLauncherPath = Path.Combine(sptFolder, "SPT.Launcher.exe");

            if (!File.Exists(sptServerPath) || !File.Exists(sptLauncherPath))
            {
                Console.WriteLine("The selected folder does not contain a valid SPT installation.");
                return SptValidationResult.INVALID_SPT_FOLDER;
            }

            string sptAssemblyCSharpBak = Path.Combine(sptFolder, @"EscapeFromTarkov_Data\Managed\Assembly-CSharp.dll.spt-bak");

            if (!File.Exists(sptAssemblyCSharpBak))
            {
                Console.WriteLine("You must run SPT.Launcher.exe and start the game at least once before you attempt to install Fika using the selected SPT folder.");
                return SptValidationResult.ASSEMBLY_CSHARP_NOT_DEOBFUSCATED;
            }

            string fikaPath = Path.Combine(sptFolder, @"BepInEx\plugins\Fika.Core.dll");
            string fikaHeadlessPath = Path.Combine(sptFolder, @"BepInEx\plugins\Fika.Headless.dll");

            if (File.Exists(fikaPath) || File.Exists(fikaHeadlessPath))
            {
                Console.WriteLine("The selected folder already contains Fika and/or Fika headless. Please select a fresh SPT install folder.");
                return SptValidationResult.MODS_DETECTED;
            }

            return SptValidationResult.OK;
        }

        public static bool CopyFolderWithProgress(string sourcePath, string destinationPath)
        {
            bool result = false;

            string[] allFiles = Directory.GetFiles(sourcePath, "*", SearchOption.AllDirectories);
            int totalFiles = allFiles.Length;
            int filesCopied = 0;

            ProgressBar progressBar = new("Copying...");

            try
            {
                foreach (string filePath in allFiles)
                {
                    string relativePath = Path.GetRelativePath(sourcePath, filePath);
                    string destFile = Path.Combine(destinationPath, relativePath);
                    string destDir = Path.GetDirectoryName(destFile);

                    if (!Directory.Exists(destDir))
                    {
                        Directory.CreateDirectory(destDir);
                    }

                    File.Copy(filePath, destFile, overwrite: true);
                    filesCopied++;

                    string fileName = Path.GetFileName(filePath);
                    
                    string message = $"Copying: {fileName}";
                    double ratio = (double)filesCopied / totalFiles;

                    progressBar.Draw(message, ratio);
                }

                result = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                progressBar.Dispose();
            }

            return result;
        }

        public static void WriteLineConfirmation(string message)
        {
            Console.WriteLine(message);
            Console.ReadKey(true);
        }
    }
}
