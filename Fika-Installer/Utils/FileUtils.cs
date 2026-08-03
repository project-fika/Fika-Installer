using System.Diagnostics;
using System.IO.Compression;
using IWshRuntimeLibrary;
using File = System.IO.File;
using ProgressBar = Fika_Installer.UI.ProgressBar;

namespace Fika_Installer.Utils;

public static class FileUtils
{
    public static string BrowseFolder(string description)
    {
        using (var dialog = new FolderBrowserDialog())
        {
            dialog.Description = description;

            var result = dialog.ShowDialog();

            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
            {
                return dialog.SelectedPath;
            }
        }

        return string.Empty;
    }

    public static bool CopyFolder(string sourcePath, string destinationPath, List<string> exclusions, bool showProgress = false)
    {
        var result = false;

        List<string> allFiles = Directory.GetFiles(sourcePath, "*", SearchOption.AllDirectories)
            .Where(file =>
            {
                var relativePath = Path.GetRelativePath(sourcePath, file);

                return !exclusions.Any(ex =>
                    relativePath == ex ||
                    relativePath.StartsWith(ex + Path.DirectorySeparatorChar));
            })
            .ToList();

        var totalFiles = allFiles.Count;
        var filesCopied = 0;

        ProgressBar? progressBar = null;

        if (showProgress)
        {
            progressBar = new();
        }

        try
        {
            foreach (var filePath in allFiles)
            {
                var relativePath = Path.GetRelativePath(sourcePath, filePath);
                var fileName = Path.GetFileName(filePath);
                var destFile = Path.Combine(destinationPath, relativePath);
                var destDir = Path.GetDirectoryName(destFile);

                if (string.IsNullOrWhiteSpace(destDir))
                {
                    continue;
                }

                if (showProgress)
                {
                    var message = $"Copying: {fileName}";
                    var progress = (double)filesCopied / totalFiles;
                    progressBar?.Draw(message, progress);
                }

                if (!Directory.Exists(destDir))
                {
                    Directory.CreateDirectory(destDir);
                }

                File.Copy(filePath, destFile, overwrite: true);
                filesCopied++;
            }

            result = true;
        }
        catch (Exception ex)
        {
            progressBar?.Dispose();
            Logger.Error(ex.Message);
        }

        progressBar?.Dispose();

        return result;
    }

    public static bool DownloadFile(string downloadUrl, string outputPath, bool showProgress = false)
    {
        var result = false;

        ProgressBar? progressBar = null;

        if (showProgress)
        {
            progressBar = new();
        }

        try
        {
            var directoryPath = Path.GetDirectoryName(outputPath);

            if (directoryPath == null)
            {
                return false;
            }

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            var fileName = Path.GetFileName(outputPath);

            using (HttpClient client = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(30)
            })
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd("FikaInstaller");

                using (var response = client.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead).Result)
                {
                    response.EnsureSuccessStatusCode();

                    var totalBytes = response.Content.Headers.ContentLength;

                    using (var contentStream = response.Content.ReadAsStreamAsync().Result)
                    {
                        using (FileStream fileStream = new(outputPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                        {
                            var buffer = new byte[8192];
                            long totalRead = 0;
                            int read;

                            while ((read = contentStream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                fileStream.Write(buffer, 0, read);
                                totalRead += read;

                                if (totalBytes.HasValue && showProgress)
                                {
                                    var progress = (double)totalRead / totalBytes.Value;
                                    progressBar?.Draw($"Downloading: {fileName}", progress);
                                }
                            }
                        }
                    }

                    result = true;
                }
            }
        }
        catch (Exception ex)
        {
            progressBar?.Dispose();
            Logger.Error(ex.Message);
        }

        progressBar?.Dispose();

        return result;
    }

    public static bool ExtractZip(string zipFilePath, string outputDirectory)
    {
        try
        {
            Directory.CreateDirectory(outputDirectory);
            ZipFile.ExtractToDirectory(zipFilePath, outputDirectory, overwriteFiles: true);

            return true;
        }
        catch (Exception ex)
        {
            Logger.Error(ex.Message);
            return false;
        }
    }

    public static bool CreateFolderSymlink(string fromPath, string toPath)
    {
        if (SecUtils.IsRunAsAdmin())
        {
            return CreateFolderSymlinkElevated(fromPath, toPath);
        }
        else
        {
            var processElevated = ProcUtils.Execute(Application.ExecutablePath, $"create-symlink \"{fromPath}\" \"{toPath}\"", ProcessWindowStyle.Minimized, true);

            if (processElevated == null)
            {
                Logger.Error("Failed to run elevated process.");
                return false;
            }

            return processElevated.ExitCode == 0;
        }
    }

    public static bool CreateFolderSymlinkElevated(string fromPath, string toPath)
    {
        try
        {
            Directory.CreateSymbolicLink(toPath, fromPath);

            return true;
        }
        catch (Exception ex)
        {
            Logger.Error(ex.Message);

            return false;
        }
    }

    public static void CreateShortcut(string shortcutPath, string targetPath, string workingDir, string iconPath, string description)
    {
        WshShell shell = new();
        IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);

        shortcut.TargetPath = targetPath;
        shortcut.WorkingDirectory = workingDir;
        shortcut.IconLocation = iconPath;
        shortcut.Description = description;
        shortcut.Save();
    }
}
