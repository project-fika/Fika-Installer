namespace Fika_Installer.UI
{
    public class ProgressBar
    {
        private string _message;
        private int _barWidth;
        private int _messageCursorTopPos;
        private int _progressBarCursorTopPos;
        private bool _isDisposed = false;

        public ProgressBar(string message = "", int barWidth = 50)
        {
            _message = message;
            _barWidth = barWidth;

            _messageCursorTopPos = Console.GetCursorPosition().Top;
            _progressBarCursorTopPos = Console.GetCursorPosition().Top + 1;

            Console.WriteLine(_message);
            Draw(0);

            Console.CursorVisible = false;
        }

        public void Draw(double ratio)
        {
            int completedWidth = (int)Math.Round(ratio * _barWidth);

            string barProgress = new('#', completedWidth);
            string barRemaining = new('-', _barWidth - completedWidth);

            int percent = (int)Math.Round(ratio * 100);

            string progressBar = $"[{barProgress}{barRemaining}] {percent}%";

            Console.Write($"\r{progressBar}");
        }

        public void Draw(string message, double ratio)
        {
            int consoleWidth = Console.BufferWidth;

            if (message.Length >= consoleWidth)
            {
                message = message.Substring(0, consoleWidth - 1);
            }

            int oldMessageLength = _message.Length;
            int newMessageLength = message.Length;
            int lengthToRemove = oldMessageLength - newMessageLength;

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
            int consoleWidth = Console.BufferWidth;

            // Prevent setting cursor outside of bounds
            if (left >= consoleWidth)
                return;

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

            int messageLength = _message.Length; // Message + space
            int barWidth = _barWidth + 2; // Progress bar + [ and ]
            int percentageLength = 5; // Space + 100%
            int progressBarTotalLength = messageLength + barWidth + percentageLength;

            Erase(0, _progressBarCursorTopPos, progressBarTotalLength);

            Console.SetCursorPosition(0, _messageCursorTopPos);
            Console.CursorVisible = true;

            _isDisposed = true;
        }
    }
}
