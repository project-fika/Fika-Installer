using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fika_Installer
{
    public class ProgressBar
    {
        string Message { get; set; }
        int BarWidth { get; set; }

        public ProgressBar(string message, int barWidth = 50)
        {
            Message = message;
            BarWidth = barWidth;

            Console.Write(Message);
            Draw(0, 1);

            Console.CursorVisible = false;
        }

        public void Draw(int progress, int total)
        {
            double percent = (double)progress / total;
            int completedWidth = (int)(percent * BarWidth);

            string visualProgress = new('#', completedWidth);
            string visualRemaining = new('-', BarWidth - completedWidth);

            string progressBar = $"[{visualProgress}{visualRemaining}] {Math.Round(percent * 100)}%";

            Console.SetCursorPosition(Message.Length + 1, Console.CursorTop);
            Console.Write(progressBar);
        }

        public void Dispose()
        {
            int messageLength = Message.Length + 1; // Message + space
            int barWidth = BarWidth + 2; // Progress bar + [ and ]
            int percentageLength = 5; // Space + 100%
            int progressBarTotalLength = messageLength + barWidth + percentageLength;

            string empty = new(' ', progressBarTotalLength);

            Console.Write($"\r{empty}\r");

            Console.CursorVisible = true;
        }
    }
}
