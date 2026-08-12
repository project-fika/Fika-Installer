namespace Fika_Installer.UI;

public class ProgressBar
{
    private string _message;
    private readonly int _barWidth;
    private readonly int _messageCursorTopPos;
    private readonly int _progressBarCursorTopPos;
    private bool _isDisposed;

    public ProgressBar(string message = "", int barWidth = 50)
    {
        _message = message;
        _barWidth = barWidth;

        _messageCursorTopPos = Console.GetCursorPosition().Top;
        _progressBarCursorTopPos = Console.GetCursorPosition().Top + 1;

        Console.WriteLine(_message);
        Draw(0);
    }

    public void Draw(double ratio)
    {
        var completedWidth = (int)Math.Round(ratio * _barWidth);

        string barProgress = new('#', completedWidth);
        string barRemaining = new('-', _barWidth - completedWidth);

        var percent = (int)Math.Round(ratio * 100);

        var progressBar = $"[{barProgress}{barRemaining}] {percent}%";

        Console.Write($"\r{progressBar}");
    }

    public void Draw(string message, double ratio)
    {
        var consoleWidth = Console.BufferWidth;

        if (message.Length >= consoleWidth)
        {
            message = message.Substring(0, consoleWidth - 1);
        }

        var oldMessageLength = _message.Length;
        var newMessageLength = message.Length;
        var lengthToRemove = oldMessageLength - newMessageLength;

        if (lengthToRemove > 0)
        {
            Erase(newMessageLength, _messageCursorTopPos, lengthToRemove);
        }

        Console.SetCursorPosition(0, _messageCursorTopPos);
        Console.Write(message);

        Console.SetCursorPosition(0, _progressBarCursorTopPos);

        _message = message;

        Draw(ratio);
    }

    private void Erase(int left, int top, int length)
    {
        // setting cursor position when not owning the console will break
        if (Logger.IsInteractive == false)
        {
            return;
        }

        var consoleWidth = Console.BufferWidth;

        // Prevent setting cursor outside of bounds
        if (left >= consoleWidth)
        {
            return;
        }

        length = Math.Min(length, consoleWidth - left);

        Console.SetCursorPosition(left, top);
        Console.Write(new string(' ', length));
        Console.SetCursorPosition(0, Console.CursorTop);
    }

    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        Erase(0, _messageCursorTopPos, _message.Length);

        var messageLength = _message.Length; // Message + space
        var barWidth = _barWidth + 2; // Progress bar + [ and ]
        const int percentageLength = 5; // Space + 100%
        var progressBarTotalLength = messageLength + barWidth + percentageLength;

        Erase(0, _progressBarCursorTopPos, progressBarTotalLength);

        Console.SetCursorPosition(0, _messageCursorTopPos);

        _isDisposed = true;
    }
}
