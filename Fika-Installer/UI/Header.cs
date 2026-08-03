namespace Fika_Installer.UI;

public static class Header
{
    public static void Show()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.BackgroundColor = ConsoleColor.Blue;

        var fikaInstallerVersionString = Installer.VersionString;

        var margin = 5;
        var headerBackgroundLength = fikaInstallerVersionString.Length + margin * 2;

        string headerBackground = new(' ', headerBackgroundLength);
        string headerTextSpacer = new(' ', margin);

        var headerText = $"{headerTextSpacer}{fikaInstallerVersionString}{headerTextSpacer}";

        Console.WriteLine(headerBackground);
        Console.WriteLine(headerText);
        Console.WriteLine(headerBackground);

        Console.WriteLine();

        Console.ResetColor();
    }
}
