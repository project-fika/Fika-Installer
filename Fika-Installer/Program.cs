using Fika_Installer.Utils;

namespace Fika_Installer
{
    public class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                ProcessArgs(args);
            }

            Console.Title = Constants.FikaInstallerVersionString;

            AppController appController = new AppController();
            appController.Start();
        }

        static void ProcessArgs(string[] args)
        {
            if (args.Length == 3 && args[0] == "-symlink")
            {
                string fromPath = args[1];
                string toPath = args[2];

                bool createSymlinkResult = FileUtils.CreateFolderSymlink(fromPath, toPath);

                int exitCode = 0;

                if (!createSymlinkResult)
                {
                    exitCode = 1;
                }

                Environment.Exit(exitCode);
            }
        }
    }
}