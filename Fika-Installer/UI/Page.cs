namespace Fika_Installer.UI;

public abstract class Page
{
    private int _pageStartPos;

    public void Show()
    {
        _pageStartPos = Console.CursorTop;

        OnShow();
        Dispose();
    }

    public abstract void OnShow();

    public void Dispose()
    {
        var currentPos = Console.CursorTop;

        for (var i = currentPos; i >= _pageStartPos; i--)
        {
            Console.SetCursorPosition(0, i);
            Console.Write(new string(' ', Console.WindowWidth));
        }

        Console.SetCursorPosition(0, _pageStartPos);
    }
}