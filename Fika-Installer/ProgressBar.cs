namespace Fika_Installer
{
    public class ProgressBar
    {
        private string _message;
        private int _barWidth;
        private int _messageCursorTopPos;
        private int _progressBarCursorTopPos;

        public ProgressBar(string message, int barWidth = 50)
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
            int oldMessageLength = _message.Length;
            int newMessageLength = message.Length;
            int lengthToRemove = oldMessageLength - newMessageLength;

            if (lengthToRemove > 0)
            {
                Erase(newMessageLength, _messageCursorTopPos, lengthToRemove);
            }
            
            Console.SetCursorPosition(0, _messageCursorTopPos);
            Console.Write($"\r{message}");
            Console.SetCursorPosition(0, _progressBarCursorTopPos);

            _message = message;

            Draw(ratio);
        }

        public void Dispose()
        {
            Erase(0, _messageCursorTopPos, _message.Length);

            int messageLength = _message.Length; // Message + space
            int barWidth = _barWidth + 2; // Progress bar + [ and ]
            int percentageLength = 5; // Space + 100%
            int progressBarTotalLength = messageLength + barWidth + percentageLength;

            Erase(0, _progressBarCursorTopPos, progressBarTotalLength);

            Console.CursorVisible = true;
        }

        private void Erase(int left, int top, int length)
        {
            Console.SetCursorPosition(left, top);
            string empty = new(' ', length);
            Console.Write(empty);
            Console.SetCursorPosition(0, Console.CursorTop);
        }
    }
}
