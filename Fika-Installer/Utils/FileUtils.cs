using System.IO.Compression;
using ProgressBar = Fika_Installer.UI.ProgressBar;

namespace Fika_Installer.Utils
{
    public static class FileUtils
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

        public static bool CopyFolderWithProgress(string sourcePath, string destinationPath, List<string> exclusions, CompositeLogger? logger)
        {
            bool result = false;

            List<string> allFiles = Directory.GetFiles(sourcePath, "*", SearchOption.AllDirectories)
                .Where(file =>
                {
                    string relativePath = Path.GetRelativePath(sourcePath, file);

                    return !exclusions.Any(ex =>
                        relativePath == ex ||
                        relativePath.StartsWith(ex + Path.DirectorySeparatorChar));
                })
                .ToList();

            int totalFiles = allFiles.Count;
            int filesCopied = 0;

            ProgressBar progressBar = new();

            try
            {
                foreach (string filePath in allFiles)
                {
                    string relativePath = Path.GetRelativePath(sourcePath, filePath);
                    string fileName = Path.GetFileName(filePath);
                    string destFile = Path.Combine(destinationPath, relativePath);
                    string? destDir = Path.GetDirectoryName(destFile);

                    if (string.IsNullOrWhiteSpace(destDir))
                    {
                        continue;
                    }

                    string message = $"Copying: {fileName}";
                    double progress = (double)filesCopied / totalFiles;
                    progressBar.Draw(message, progress);

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
                progressBar.Dispose();
                logger?.Error($"An error occurred while copying the folder: {ex.Message}", true);
            }

            progressBar.Dispose();

            return result;
        }

        public static bool DownloadFileWithProgress(string downloadUrl, string outputPath, CompositeLogger? logger)
        {
            bool result = false;

            ProgressBar progressBar = new();

            try
            {
                string? directoryPath = Path.GetDirectoryName(outputPath);

                if (directoryPath == null)
                {
                    return false;
                }

                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                string fileName = Path.GetFileName(outputPath);

                using (HttpClient client = new HttpClient
                {
                    Timeout = TimeSpan.FromMinutes(30)
                })
                {
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("FikaInstaller");

                    using (HttpResponseMessage response = client.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead).Result)
                    {
                        response.EnsureSuccessStatusCode();

                        long? totalBytes = response.Content.Headers.ContentLength;

                        using (Stream contentStream = response.Content.ReadAsStreamAsync().Result)
                        {
                            using (FileStream fileStream = new(outputPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                            {
                                byte[] buffer = new byte[8192];
                                long totalRead = 0;
                                int read;

                                while ((read = contentStream.Read(buffer, 0, buffer.Length)) > 0)
                                {
                                    fileStream.Write(buffer, 0, read);
                                    totalRead += read;

                                    if (totalBytes.HasValue)
                                    {
                                        double progress = (double)totalRead / totalBytes.Value;
                                        progressBar.Draw($"Downloading: {fileName}", progress);
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
                progressBar.Dispose();
                logger?.Error(ex.Message);
            }

            progressBar.Dispose();

            return result;
        }

        public static void ExtractZip(string zipFilePath, string outputDirectory, CompositeLogger? logger)
        {
            try
            {
                Directory.CreateDirectory(outputDirectory);
                ZipFile.ExtractToDirectory(zipFilePath, outputDirectory, overwriteFiles: true);
            }
            catch (Exception ex)
            {
                logger?.Log(ex.Message);
            }
        }

        public static bool CreateFolderSymlink(string fromPath, string toPath, CompositeLogger? logger)
        {
            try
            {
                Directory.CreateSymbolicLink(toPath, fromPath);
            }
            catch (Exception ex)
            {
                logger?.Error($"An error occurred when creating the symlink: {ex.Message}");
                return false;
            }

            return true;
        }
    }
}
