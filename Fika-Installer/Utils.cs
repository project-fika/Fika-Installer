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
                    double ratio = (double)filesCopied / totalFiles;

                    progressBar.Draw(message, ratio);

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

        public static void WriteLineConfirmation(string message)
        {
            Console.WriteLine(message);
            Console.ReadKey(true);
        }
    }
}
