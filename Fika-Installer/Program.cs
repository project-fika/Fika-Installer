using Fika_Installer.UI;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Fika_Installer
{
    // Jank fix to avoid loading winhttp.dll from SPT directory which causes crashes for some people
    // This causes winhttp.dll to get loaded twice in memory, but this one will be prioritized due to load order
    static class StartupNative
    {
        const uint LOAD_LIBRARY_SEARCH_SYSTEM32 = 0x00000800;

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hFile, uint dwFlags);

        [ModuleInitializer]
        public static void Init()
        {
            IntPtr h = LoadLibraryEx("winhttp.dll", IntPtr.Zero, LOAD_LIBRARY_SEARCH_SYSTEM32);
            if (h == IntPtr.Zero)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }
    }

    public class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            string logFilePath = Path.Combine(Installer.CurrentDir, "fika-installer.log");

            FileLogger fileLogger = new(logFilePath);
            Logger.AddLogger(fileLogger);

            PageLogger pageLogger = new();
            Logger.AddLogger(pageLogger);

            if (args.Length > 0)
            {
                Logger.SetInteractive(false);
                CLI.Parse(args);
            }
            else
            {
                Logger.SetInteractive(true);
                InitUI();
            }
        }

        static void InitUI()
        {
            Console.Title = Installer.VersionString;
            Console.CursorVisible = false;

            Header.Show();

            MenuFactory menuFactory = new(Installer.CurrentDir);

            while (true)
            {
                Menu mainMenu = menuFactory.CreateMainMenu();
                mainMenu.Show();
            }
        }
    }
}