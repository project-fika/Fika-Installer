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

        public static bool CopyFolderWithProgress(string sourcePath, string destinationPath)
        {
            bool result = false;

            string[] allFiles = Directory.GetFiles(sourcePath, "*", SearchOption.AllDirectories);
            int totalFiles = allFiles.Length;
            int filesCopied = 0;

            ProgressBar progressBar = new();

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

                    string fileName = Path.GetFileName(filePath);

                    string message = $"Copying: {fileName}";
                    double progress = (double)filesCopied / totalFiles;

                    progressBar.Draw(message, progress);

                    File.Copy(filePath, destFile, overwrite: true);
                    filesCopied++;
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

        public static bool DownloadFileWithProgress(string downloadUrl, string outputPath)
        {
            bool result = false;

            ProgressBar progressBar = new();

            try
            {
                string directoryPath = Path.GetDirectoryName(outputPath);

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
                            using (FileStream fileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
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
