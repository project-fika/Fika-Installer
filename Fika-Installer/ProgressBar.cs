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
        int MessageCursorPos;
        int ProgressBarCursorPos;

        public ProgressBar(string message, int barWidth = 50)
        {
            Message = message;
            BarWidth = barWidth;

            MessageCursorPos = Console.GetCursorPosition().Top;
            ProgressBarCursorPos = Console.GetCursorPosition().Top + 1;

            Console.WriteLine(Message);
            Draw(0);

            Console.CursorVisible = false;
        }

        public void Draw(double ratio)
        {
            int completedWidth = (int)Math.Round(ratio * BarWidth);
            string visualProgress = new('#', completedWidth);
            string visualRemaining = new('-', BarWidth - completedWidth);

            int percent = (int)Math.Round(ratio * 100);

            string progressBar = $"[{visualProgress}{visualRemaining}] {percent}%";

            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(progressBar);
        }

        public void Draw(string message, double ratio)
        {
            Erase(MessageCursorPos, Message.Length);
       
            Message = message;
            Console.Write(message);

            Console.SetCursorPosition(0, MessageCursorPos + 1);
            Draw(ratio);
        }
        public void Dispose()
        {
            Erase(MessageCursorPos, Message.Length);

            int messageLength = Message.Length; // Message + space
            int barWidth = BarWidth + 2; // Progress bar + [ and ]
            int percentageLength = 5; // Space + 100%
            int progressBarTotalLength = messageLength + barWidth + percentageLength;

            Erase(ProgressBarCursorPos, progressBarTotalLength);

            Console.CursorVisible = true;
        }

        private void Erase(int pos, int length)
        {
            Console.SetCursorPosition(0, pos);
            string empty = new(' ', length);
            Console.Write(empty);
            Console.SetCursorPosition(0, Console.CursorTop);
        }
    }
}
