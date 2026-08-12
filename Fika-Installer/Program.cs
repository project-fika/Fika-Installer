using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Fika_Installer.UI;

namespace Fika_Installer;

[Flags]
public enum LoadLibraryFlags : uint
{
    None = 0x00000000,
    LoadLibrarySearchUserDirs = 0x00000400,
    LoadLibrarySearchSystem32 = 0x00000800,
    LoadLibrarySearchDefaultDirs = 0x00001000
}

// Jank fix to avoid using winhttp.dll from SPT directory which causes crashes for some people
// This causes winhttp.dll to get loaded twice in memory, but this one will be prioritized due to load order
static partial class StartupNative
{
    [LibraryImport("kernel32", EntryPoint = "LoadLibraryExW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    public static partial IntPtr LoadLibraryEx(string lpFileName, IntPtr hFile, LoadLibraryFlags dwFlags);

    [ModuleInitializer]
    public static void Init()
    {
        var h = LoadLibraryEx("winhttp.dll", IntPtr.Zero, LoadLibraryFlags.LoadLibrarySearchSystem32);
        if (h == IntPtr.Zero)
        {
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }
    }
}

public static class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        var logFilePath = Path.Combine(Installer.CurrentDir, "fika-installer.log");

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
            var mainMenu = menuFactory.CreateMainMenu();
            mainMenu.Show();
        }
    }
}